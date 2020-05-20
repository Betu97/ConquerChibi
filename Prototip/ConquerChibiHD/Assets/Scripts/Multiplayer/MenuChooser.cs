using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System.Collections;
using Photon.Pun;
using Photon.Realtime;

using UnityEngine.UI;

namespace Com.MyCompany.multiTest
{
    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    /// 
    public class MenuChooser : MonoBehaviourPunCallbacks, IPunObservable
    {

        public float timeLeft = 20.0f;
        public static GameObject LocalPlayerInstance;
        #region Private Fields
        private int indexArena;
        private bool voted = false;
        private bool pressed = false;
        private int lastIndex = 0;
        private Text timer;
        private int indexPlayer;
        private bool votedPlayer = false;
        private bool pressedPlayer = false;
        private int lastIndexPlayer = 0;
        private Text timerPlayer;
        private bool arenaVoting = false;
        private GameObject PlayerControlPanel;
        private GameObject ArenaControlPanel;

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
            }
            else
            {
            }

            if (stream.IsWriting)
            {
            }
            else
            {
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
                MenuChooser.LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);

            try
            {
                timer = GameObject.Find("Counter").GetComponent<Text>();
                timerPlayer = GameObject.Find("CounterPlayer").GetComponent<Text>();
                ArenaControlPanel = GameObject.Find("ArenaControlPanel");
                ArenaControlPanel.SetActive(false);
                PlayerControlPanel = GameObject.Find("PlayerControlPanel");
            }
            catch
            {
                //Debug.LogError("error en inicialitzar en el MenuChooser");
            }
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            

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
            try
            {                
                timeLeft -= Time.deltaTime;
                if (arenaVoting)
                {
                    timer.text = Mathf.Round(timeLeft).ToString();
                    if (timeLeft < 0)
                    {
                        timer.text = "0";
                    }
                }
                else
                {
                    timerPlayer.text = Mathf.Round(timeLeft).ToString();
                    Debug.Log("Canvio text");
                    if (timeLeft < 0)
                    {
                        Debug.Log("Entro al time 0");
                        timerPlayer.text = "0";
                        arenaVoting = true;
                        ArenaControlPanel.SetActive(true);
                        PlayerControlPanel.SetActive(false);
                        timeLeft = 20.0f;
                    }
                }
            }
            catch
            {
                //Debug.LogError("Error en agafar el text de timer");
            }

            // we only process Inputs if we are the local player
            if (photonView.IsMine)
            {
                if (pressedPlayer == true)
                {
                    try
                    {
                        //si ja ha votat anteriorment, retirarem el vot anterior per fer el canvi
                        if (votedPlayer)
                        {
                            int punctuationPlayer = int.Parse(GameObject.Find("Player" + lastIndexPlayer + "Text").GetComponent<Text>().text);
                            punctuationPlayer--;
                            GameObject.Find("Player" + lastIndexPlayer + "Text").GetComponent<Text>().text = punctuationPlayer.ToString();
                        }
                        //ficarem a true el voted i afagirem la nova votació
                        votedPlayer = true;
                        lastIndexPlayer = indexPlayer;
                        Debug.Log("Adding 1 vote to " + indexPlayer + " Player");
                        int punctuationPlayermore = int.Parse(GameObject.Find("Player" + indexPlayer + "Text").GetComponent<Text>().text);
                        punctuationPlayermore++;
                        GameObject.Find("Player" + indexPlayer + "Text").GetComponent<Text>().text = punctuationPlayermore.ToString();
                        pressedPlayer = false;

                    }
                    catch
                    {
                        Debug.Log("Error Mostrando la interaccion");
                    }
                }
                if (pressed == true)
                {
                    try
                    {
                        //si ja ha votat anteriorment, retirarem el vot anterior per fer el canvi
                        if (voted)
                        {
                            photonView.RPC("unvote", RpcTarget.All, lastIndex);                            
                        }
                        //ficarem a true el voted i afagirem la nova votació
                        voted = true;
                        lastIndex = indexArena;
                        photonView.RPC("vote", RpcTarget.All, indexArena);
                        pressed = false;

                    }
                    catch
                    {
                        Debug.Log("Error Mostrando la interaccion");
                    }
                }
            }
        }

        [PunRPC]
        public void vote(int index)
        {
            Debug.Log("Adding 1 vote to " + index + " Arena");
            int punctuation = int.Parse(GameObject.Find("Arena" + index +"Text").GetComponent<Text>().text);
            punctuation++;
            GameObject.Find("Arena" + index +"Text").GetComponent<Text>().text = punctuation.ToString();
        }

        [PunRPC]
        public void unvote(int index)
        {
            int punctuation = int.Parse(GameObject.Find("Arena" + index + "Text").GetComponent<Text>().text);
            punctuation--;
            GameObject.Find("Arena" + index + "Text").GetComponent<Text>().text = punctuation.ToString();
        }


        #endregion

        #region Custom

        public void voteArena(int index)
        {
            indexArena = index;
            pressed = true;
        }

        public void votePlayer(int index)
        {
            indexPlayer = index;
            pressedPlayer = true;
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
