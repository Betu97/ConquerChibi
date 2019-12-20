using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System.Collections;
using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.multiTest
{
    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        public GameObject uinteract;
        bool col = false;

        public float Health = 1f;
        public static GameObject LocalPlayerInstance;
        public AudioSource Attack1;
        public AudioSource HitAttack1;

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;

        [Tooltip("The Player's Fire Point")]
        [SerializeField]
        public GameObject FirePoint;
        private bool CanFire = true;
        public float Firerate = 2.0f;

        #region Private Fields

        //True, when the user is firing
        //bool IsFiring;
        #endregion

        #region Private Methods
            #if UNITY_5_4_OR_NEWER
                    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
                    {
                        this.CalledOnLevelWasLoaded(scene.buildIndex);
                    }
            #endif
        #endregion

        #region IPunObservable implementation


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                //stream.SendNext(IsFiring);
            }
            else
            {
                // Network player, receive data
                //this.IsFiring = (bool)stream.ReceiveNext();
            }

            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                //stream.SendNext(IsFiring);
                stream.SendNext(Health);
            }
            else
            {
                // Network player, receive data
                //this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }

        }


        #endregion


        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {

            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            uinteract.SetActive(false);
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();


            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }

            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            #endif
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        void Update()
        {    


            // we only process Inputs if we are the local player
            if (photonView.IsMine)
            {

                if (col == true)
                {
                    try
                    {
                        uinteract.SetActive(true);
                        if (Input.GetButtonDown("MapInteraction"))
                        {
                            photonView.RPC("rotate", RpcTarget.All);    
                        }
                    }
                    catch
                    {
                        Debug.Log("Error Mostrando la interaccion");
                    }
                }

                if (Input.GetButton("Fire1") && CanFire)
                {
                        photonView.RPC("shoot", RpcTarget.All, FirePoint.transform.position, FirePoint.transform.rotation);
                        StartCoroutine(StartCounting(Firerate));
                        CanFire = false;
                }

                ProcessInputs();
                
                if (Health <= 0f)
                {
                    GameManager.Instance.LeaveRoom();
                }
            }
        }

        [PunRPC]
        public void rotate()
        {
            LeverScript.Instance.rotatePlatform();
        }

        [PunRPC]
        public void shoot(Vector3 pos, Quaternion angle)
        {
            //GameManager.Instance.SpawnVFX(FirePoint.transform.position, FirePoint.transform.rotation);
            GameManager.Instance.SpawnVFX(pos, angle);
            Attack1.Play();
        }

        /// <summary>
        /// MonoBehaviour method called when the Collider 'other' enters the trigger.
        /// Affect Health of the Player if the collider is a beam
        /// Note: when jumping and firing at the same, you'll find that the player's own beam intersects with itself
        /// One could move the collider further away to prevent this or check if the beam belongs to the player.
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (other.name.Contains("Panel2"))
            {
                col = true;
            }
            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            //Debug.LogWarning("hello" + SceneManager.GetActiveScene().name, this);
            if (SceneManager.GetActiveScene().name.Equals("Lobby") || !other.name.Contains("Pinxo"))
            {
                return;
            }
            Health -= 0.1f;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (other.name.Contains("Panel2"))
            {
                col = false;
                uinteract.SetActive(false);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "Projectile")
            {
                HitAttack1.Play();
                Health -= 0.1f;
            }
        }

        /// <summary>
        /// MonoBehaviour method called once per frame for every Collider 'other' that is touching the trigger.
        /// We're going to affect health while the beams are touching the player
        /// </summary>
        /// <param name="other">Other.</param>
        void OnTriggerStay(Collider other)
        {
            // we dont' do anything if we are not the local player.
            if (!photonView.IsMine)
            {
                return;
            }
            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (other.gameObject.tag == "Pinxo")
            {
                Health -= 0.1f * Time.deltaTime;
            }
        }

            #if !UNITY_5_4_OR_NEWER
            void OnLevelWasLoaded(int level)
            {
                this.CalledOnLevelWasLoaded(level);
            }
            #endif


        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }


        #endregion

        #region Custom

        /// <summary>
        /// Processes the inputs. Maintain a flag representing when the user is pressing Fire.
        /// </summary>
        void ProcessInputs()
        {
            /*
            if (Input.GetButtonDown("Fire1"))
            {
                if (!IsFiring)
                {
                    IsFiring = true;
                    StartCoroutine(StartCounting(Firerate));
                    CanFire = false;
                }
            }
            if (Input.GetButtonUp("Fire1"))
            {
                if (IsFiring)
                {
                    IsFiring = false;
                }
            }*/
        }

        private IEnumerator StartCounting(float firerate)
        {
            float waitTime = 1 / Firerate;
            yield return new WaitForSeconds(waitTime);
            CanFire = true;
        }

#if UNITY_5_4_OR_NEWER
        public override void OnDisable()
                {
                    // Always call the base to remove callbacks
                    base.OnDisable();
                    UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
                }
        #endif

        #endregion
    }
}