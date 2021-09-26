using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.Tencent.DYYS
{
    public class ItemManager : MonoBehaviourPun
    {
        public List<GameObject> prefab_FreshPointList;
        public List<GameObject> Prefab_ItemList;

        public int MaxRefreshNum;
        public float RefreshCD;

        private float currentTime;

        private List<int> list_FreshPointState;
        private Dictionary<GameObject, int> dict_Item2PointIndex;


        void Awake()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            PlayerItem.manager_Item = this;
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                return;
            }
            InitItemManager();
        }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                return;
            }
            currentTime += Time.deltaTime;
            if (currentTime > RefreshCD)
            {
                currentTime = 0;
                RefreshPoints();
            }
        }

        void RefreshPoints()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                return;
            }
            if (dict_Item2PointIndex.Count < MaxRefreshNum)
            {
                int PointIndex = Random.Range(0, prefab_FreshPointList.Count);
                if (list_FreshPointState[PointIndex] == 0)
                {
                    list_FreshPointState[PointIndex] = 1;
                    int ItemIndex = Random.Range(0, Prefab_ItemList.Count);
                    Transform pointTransform = prefab_FreshPointList[PointIndex].GetComponent<Transform>();
                    GameObject go = null;
                    if (PhotonNetwork.IsConnected)
                    {
                        go = PhotonNetwork.Instantiate(Prefab_ItemList[ItemIndex].name, pointTransform.position, pointTransform.rotation, 0);
                    }
                    else
                    {
                        go = GameObject.Instantiate(Prefab_ItemList[ItemIndex], pointTransform);
                    }
                    //将item的管理接口挂过去方便销毁时调用接口
                    ItemPrefab item = go.GetComponent<ItemPrefab>();
                    item.Manager_Item = this;

                    dict_Item2PointIndex.Add(go, PointIndex);
                }
            }
        }

        [PunRPC]
        public void ItemDestroyRPC(int pvid)
        {
            PhotonView goPV = PhotonView.Find(pvid);
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                return;
            }
            GameObject go = goPV.gameObject;
            MasterDestroyItem(go);
        }

        private void MasterDestroyItem(GameObject go)
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                return;
            }
            int PointIndex = dict_Item2PointIndex[go];
            list_FreshPointState[PointIndex] = 0;
            dict_Item2PointIndex.Remove(go);
            Debug.Log(dict_Item2PointIndex);
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Destroy(go);
            }
            else
            {
                GameObject.Destroy(go);
            }
        }

        public void ItemDestroy(GameObject go)
        {
            if (go)
            {
                PhotonView m_PhotonView = PhotonView.Get(this);

                if (PhotonNetwork.IsConnected && m_PhotonView != null && !PhotonNetwork.IsMasterClient)
                {
                    PhotonView goPvid = go.GetPhotonView();
                    this.photonView.RPC("ItemDestroyRPC", RpcTarget.MasterClient, goPvid.ViewID);
                }
                else
                {
                    MasterDestroyItem(go);
                }

            }
        }
        public void InitItemManager()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                return;
            }
            if (dict_Item2PointIndex != null)
            {
                foreach (GameObject item in dict_Item2PointIndex.Keys)
                {
                    if (PhotonNetwork.IsConnected && item)
                    {
                        PhotonNetwork.Destroy(item);
                    }
                    else
                    {
                        GameObject.Destroy(item);
                    }
                }
            }
            currentTime = 0;
            if (dict_Item2PointIndex == null)
            {
                list_FreshPointState = new List<int>();
                dict_Item2PointIndex = new Dictionary<GameObject, int>();
            }
            else
            {
                list_FreshPointState.Clear();
                dict_Item2PointIndex.Clear();
            }
            for (int i = 0; i < prefab_FreshPointList.Count; i++)
            {
                //-1表示数据配置有误，prefab是null
                list_FreshPointState.Add(-1);
                if (prefab_FreshPointList[i])
                {
                    //0表示该刷新点没有东西
                    list_FreshPointState[i] = 0;
                }
            }
        }
    }
}
