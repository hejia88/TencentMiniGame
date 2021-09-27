using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Com.Tencent.DYYS
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {

        [Header("玩家预制体")]
        public GameObject playerPrefab;

        [Header("在场景中配置的重生点")]
        public GameObject[] RebornSpots;
        
        [Header("玩家总命数")]
        public int TotalHealth = 3;
        
        //保证每个客户端只生成自己的玩家实例
        void Start()
        {
            if (playerPrefab == null)
            {
                Debug.LogError(
                    "<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",
                    this);
            }
            else
            {
                if (PlayerMovement.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    Transform NewBornSpot = SelectRebornSpot();
                    PhotonNetwork.Instantiate(this.playerPrefab.name, 
                        NewBornSpot.position, 
                        NewBornSpot.rotation,
                        0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        Transform SelectRebornSpot()
        {
            Transform ResTranfrom = new RectTransform();
            if (RebornSpots.Length > 0)
            {
                int RandomIndex = Random.Range(0, RebornSpots.Length);
                Debug.LogFormat("Total Born points are {0}, Ready to Reborn at point {1}", RebornSpots.Length, RandomIndex);
                ResTranfrom = RebornSpots[RandomIndex].transform;
                return ResTranfrom;
            }
            else
            {
                ResTranfrom.position = new Vector3(.0f, 1.0f, 20.0f);
                ResTranfrom.rotation = new Quaternion(0, 1, 0, 0);
                return ResTranfrom;
            }
        }

        public void RebornPlayer()
        {
            if (PhotonNetwork.IsConnected)
            {
                TotalHealth--;
                if (TotalHealth > 0)
                {
                    Debug.LogFormat("{0} Ready Reborn", this.gameObject);
                    Transform NewBornSpot = SelectRebornSpot();
                    PhotonNetwork.Instantiate(this.playerPrefab.name, 
                        NewBornSpot.position, 
                        NewBornSpot.rotation,
                        0);
                }
            }
        }
    }
}
