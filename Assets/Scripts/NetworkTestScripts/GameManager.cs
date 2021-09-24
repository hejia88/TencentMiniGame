using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;


namespace Com.Tencent.DYYS
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public PlayerMovement[] Players;
        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}",
                    PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
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

        
        public void RefreshPlayers()
        {
            PhotonView m_PhotonView = PhotonView.Get(this);
            if (PhotonNetwork.IsConnected && m_PhotonView != null)
            {
                m_PhotonView.RPC("MasterClient_RefreshPlayers", RpcTarget.MasterClient);
            }
        }

        [PunRPC]
        void MasterClient_RefreshPlayers()
        {
            Players = FindObjectsOfType<PlayerMovement>();
            int cnt = 0;
            foreach (var pm in Players)
            {
                Debug.LogFormat("CurrPlayer {0} is {1}", ++ cnt, pm.photonView);
            }
        }

        #endregion


        #region Public Methods

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        public GameObject ItemManagePrefab;

        public static GameManager GameManagerInstance;

        private ItemManager ItemManagerInstance;

        void Awake()
        {
        }

        void Start()
        {
            GameManagerInstance = this;

            if (ItemManagePrefab != null)
            {
                ItemManagerInstance = ItemManagePrefab.GetComponent<ItemManager>();
            }

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerMovement.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    var player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(.0f, 1.0f, 20.0f), new Quaternion(0, 1, 0, 0), 0);
                    /*Players.Add(player.GetComponent<PlayerMovement>());
                    foreach (PlayerMovement pm in Players)
                    {
                        pm.LinkPlayers();
                    }*/
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            ItemManagerInstance.InitItemManager();
            Debug.LogFormat("PhotonNetwork : Loading Level : ATS6_0915");
            PhotonNetwork.LoadLevel("ATS6_0915");
        }

        #endregion
    }
}