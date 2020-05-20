using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


using Photon.Pun;
using Photon.Realtime;



namespace Com.MyCompany.multiTest
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance;
        public GameObject playerPrefab1;
        public GameObject playerPrefab2;
        public GameObject menuPrefab;
        public Canvas canvas;
        public List<GameObject> vfx = new List<GameObject>();
        public float menuTime = 40.0f;

        Boolean menu;
        private GameObject effectToSpawn;
        private GameObject effectToSpawnBB;
        private GameObject menuVotation;
        bool startedGame;

        void Start()
        {
            if (InGame())
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                menu = false;
                effectToSpawn = vfx[0];
                effectToSpawnBB = vfx[1];

                Instance = this;
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    int choosenHero = PlayerPrefs.GetInt("hero", 1);
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    if(choosenHero == 2){
                        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                        PhotonNetwork.Instantiate(this.playerPrefab2.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                    }
                    else
                    {
                        PhotonNetwork.Instantiate(this.playerPrefab1.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                    }                  

                    
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
            else if(SceneManager.GetActiveScene().name == "ArenaSelection")
            {
                Instance = this;
                StartCoroutine(MenuCountdown(menuTime));
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalMenu from {0}", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    menuVotation = PhotonNetwork.Instantiate(this.menuPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
            
        }

        private void Update()
        {
            if (Input.GetKeyDown("escape") && InGame())
            {
                if(menu == false && canvas != null)
                {
                    canvas.gameObject.SetActive(true);
                    menu = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    canvas.gameObject.SetActive(false);
                    menu = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                
            }
        }

        #region Photon Callbacks


        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
            try
            {
                if (SceneManager.GetActiveScene().name == "ArenaSelection")
                {
                    GameObject[] clones = GameObject.FindGameObjectsWithTag("menuPrefab");
                    foreach (GameObject clone in clones)
                    {
                        GameObject.Destroy(clone);
                    }
                }
            }
            catch
            {
                Debug.LogError("Could not destroy player instances in arena selection");
            }


                if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(0);
            try
            {
                if (SceneManager.GetActiveScene().name == "ArenaSelection")
                {
                    GameObject[] clones = GameObject.FindGameObjectsWithTag("menuPrefab");
                    foreach (GameObject clone in clones)
                    {
                        GameObject.Destroy(clone);
                    }
                }
            }
            catch
            {
                Debug.LogError("Could not destroy player instances in arena selection");
            }
        }

        private void OnApplicationQuit()
        {
            LeaveRoom();
        }


        #endregion


        #region Public Methods


        public void LeaveRoom()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PhotonNetwork.LeaveRoom();
        }

        public void Vote(int index)
        {
            try
            {
                if (menuVotation == null)
                {
                    Debug.Log("No l'he trobat");
                }
                else
                {
                    menuVotation.GetComponent<MenuChooser>().voteArena(index);
                }
            }
            catch
            {
                Debug.LogError("Ha habido un error con el prefab del menu");
            }
        }

        public void VotePlayer(int index)
        {
            try
            {
                if (menuVotation == null)
                {
                    Debug.Log("No l'he trobat");
                }
                else
                {
                    menuVotation.GetComponent<MenuChooser>().votePlayer(index);
                }
            }
            catch
            {
                Debug.LogError("Ha habido un error con el prefab del menu");
            }
        }

        private IEnumerator MenuCountdown(float menuTime)
        {
            float normalizedTime = 0;
            while (normalizedTime <= 1f)
            {
                normalizedTime += Time.deltaTime / menuTime;
                yield return null;
            }
            WinningArena();
        }

        private void WinningArena()
        {
            try
            {
                PlayerPrefs.SetInt("hero", menuVotation.GetComponent<MenuChooser>().getIndexPlayer);
                int[] punctuation = new int[6];
                punctuation[0] = int.Parse(GameObject.Find("Arena1Text").GetComponent<Text>().text);
                punctuation[1] = int.Parse(GameObject.Find("Arena2Text").GetComponent<Text>().text);
                punctuation[2] = int.Parse(GameObject.Find("Arena3Text").GetComponent<Text>().text);
                punctuation[3] = int.Parse(GameObject.Find("Arena4Text").GetComponent<Text>().text);
                punctuation[4] = int.Parse(GameObject.Find("Arena5Text").GetComponent<Text>().text);
                punctuation[5] = int.Parse(GameObject.Find("Arena6Text").GetComponent<Text>().text);
                int maxIndex = punctuation.ToList().IndexOf(punctuation.Max());
                Debug.Log("Arena guanyadora a l'index" + maxIndex);
                if (!PhotonNetwork.IsMasterClient)
                {
                    Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                }
                Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
                if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
                {
                    switch (maxIndex)
                    {
                        case 0:
                            Debug.Log("Loading Arena 0");
                            PhotonNetwork.LoadLevel("AldeaYamikaze");
                            break;
                        case 1:
                            Debug.Log("Loading Arena 1");
                            PhotonNetwork.LoadLevel("Evig Herlighed");
                            break;
                        case 2:
                            Debug.Log("Loading Arena 2");
                            PhotonNetwork.LoadLevel("K’áax Chakhole’en");
                            break;
                        case 3:
                            Debug.Log("Loading Arena 3");
                            PhotonNetwork.LoadLevel("Pfarrei des Hungerus");
                            break;
                        case 4:
                            Debug.Log("Loading Arena 4");
                            PhotonNetwork.LoadLevel("Pansi Zehm");
                            break;
                        case 5:
                            Debug.Log("Loading Arena 5");
                            PhotonNetwork.LoadLevel("Siku Angisooq");
                            break;
                        default:
                            Debug.Log("DefaultLevelPrinted");
                            PhotonNetwork.LoadLevel("AldeaYamikaze");
                            break;
                    }


                }
            }
            catch
            {
                Debug.LogError("Fallo en la elección de arena final");
            }
        }

        public void SpawnVFX(Vector3 from, Quaternion to, int effect)
        {
            GameObject vfx;
            if(effect == 2) {
                vfx = Instantiate(effectToSpawnBB, from, Quaternion.identity);
                vfx.transform.localRotation = to;
            }
            else
            {
                vfx = Instantiate(effectToSpawn, from, Quaternion.identity);
                vfx.transform.localRotation = to;
            }
            
        }
    


    #endregion

    #region Private Methods


    void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                if (SceneManager.GetActiveScene().name == "ArenaSelection")
                {
                    PhotonNetwork.LoadLevel("WaitingPlayers");
                }
                else
                {
                    LeaveRoom();
                }
                
            }
            else
            {
                if (startedGame || SceneManager.GetActiveScene().name == "Lobby")
                {
                    PhotonNetwork.LoadLevel("AldeaYamikaze");
                }
                else
                {
                    PhotonNetwork.LoadLevel("ArenaSelection");
                }
            }
        }


        private bool InGame()
        {
            if (SceneManager.GetActiveScene().name != "ArenaSelection" && SceneManager.GetActiveScene().name != "WaitingPlayers")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}