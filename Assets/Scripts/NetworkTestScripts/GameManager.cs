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
        public List<PlayerMovement> Players;
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
                    var player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(.0f, .0f, .0f), new Quaternion(0, 1, 0, 0), 0);
                    Players.Add(player.GetComponent<PlayerMovement>());
                    foreach (PlayerMovement pm in Players)
                    {
                        pm.LinkPlayers();
                    }
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
            PlayerMovement.AIs = FindObjectsOfType<AIMovement>();
            ItemManagerInstance.InitItemManager();
            Debug.LogFormat("PhotonNetwork : Loading Level : Art3DScene");
            PhotonNetwork.LoadLevel("Art3DScene");
        }

        #endregion
    }
}