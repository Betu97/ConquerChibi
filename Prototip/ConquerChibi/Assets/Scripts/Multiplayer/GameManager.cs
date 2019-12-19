using System;
using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;


namespace Com.MyCompany.multiTest
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance;
        public GameObject playerPrefab;
        public Canvas canvas;
        public List<GameObject> vfx = new List<GameObject>();

        Boolean menu;
        private GameObject effectToSpawn;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            menu = false;
            effectToSpawn = vfx[0];

            Instance = this;
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
            
        }

        private void Update()
        {
            if (Input.GetKeyDown("escape"))
            {
                if(menu == false)
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


        public void SpawnVFX(Vector3 from, Quaternion to)
        {
            GameObject vfx;

            vfx = Instantiate(effectToSpawn, from, Quaternion.identity);
            vfx.transform.localRotation = to;
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
                PhotonNetwork.LoadLevel("Lobby");
            }
            else
            {
                PhotonNetwork.LoadLevel("AldeaYamikaze");
            }
            
        }


        #endregion

    }
}