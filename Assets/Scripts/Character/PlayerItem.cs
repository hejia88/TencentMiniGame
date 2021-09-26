using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public enum ItemState
{
    Nothing,
    WaitPick,
    Picking,
    Owning
}

namespace Com.Tencent.DYYS
{
    public class PlayerItem : MonoBehaviourPun, IPunObservable
    {
        public static ItemManager manager_Item;
        public ItemState m_ItemState;
        public GameObject Player;
        public Button btn_Pick;
        public Button btn_Use;
        public Image img_ActivateItemNum;
        public Image img_BtnUseBG;
        public Text txt_ActivateItemNum;
        public Color pickHightLightColor;
        
        private Sprite UITexture;
        private AudioClip ItemAudio;
        public Transform m_PanelTransform;
        public Transform m_ArrowTransform;

        private PlayerMovement m_PlayerMovement;
        private Transform m_Transform;
        private int targetPlayerIndex;
        private float timer_Assassination;

        public ItemState PickState
        {
            get { return m_ItemState; }
        }

        [HideInInspector] public bool isActivateItem;
        [HideInInspector] public int itemCount;
        [HideInInspector] public int itemIndex;
        [HideInInspector] public ItemName itemName;
        [HideInInspector] public float assassinationCDTime;

        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
        }
        #endregion

        void Awake()
        {
            btn_Use.gameObject.SetActive(false);
            img_ActivateItemNum.GetComponent<Image>().color = pickHightLightColor;
            img_ActivateItemNum.gameObject.SetActive(false);
        }

        void Start()
        {
            if (PhotonNetwork.IsConnected == true && photonView.IsMine == false)
            {
                m_PanelTransform.gameObject.SetActive(false);
            }
            manager_Item = FindObjectOfType<ItemManager>();
            
            m_ItemState = ItemState.Nothing;
            isActivateItem = false;
            itemCount = 0;
            itemIndex = 0;
            itemName = ItemName.Others;
            targetPlayerIndex = -1;
            timer_Assassination = 0;
            btn_Pick.onClick.AddListener(OnBtnPickClick);
            btn_Use.onClick.AddListener(OnBtnUseClick);
            m_PlayerMovement=gameObject.GetComponent<PlayerMovement>();
            m_Transform= gameObject.GetComponent<Transform>();
            m_ArrowTransform.gameObject.SetActive(false);

            //NormalBtnPick();
        }

        void Update()
        {
            if (PhotonNetwork.IsConnected == true && photonView.IsMine == false)
            {
                return;
            }
            UpdateArrow();
        }

        private void OnBtnPickClick()
        {
            PickItem();
        }

        private void OnBtnUseClick()
        {
            UseActivateItem();
        }

        public void PickItem()
        {
            m_ItemState = ItemState.Picking;
        }

        public void UseActivateItem()
        {
            if (m_ItemState == ItemState.Owning)
            {
                if (isActivateItem)
                {
                    if (itemCount > 0)
                    {
                        itemCount -= 1;
                        if (itemCount == 0)
                        {
                            ShowBtnPick();
                            m_ItemState = ItemState.Nothing;
                        }
                        else
                        {
                            ShowBtnUse(isActivateItem,  itemCount);
                        }
                        if (itemName == ItemName.Assassination)
                        {
                            UseAssassination();
                        }
                    }
                }
            }
        }

        private void UseAssassination()
        {
            if (Player)
            {
                int TotalNum = m_PlayerMovement.Players.Count;
                targetPlayerIndex = -1;
                timer_Assassination = assassinationCDTime;
                List<int> indexUsedList = new List<int>();
                for (int i = 0; i < m_PlayerMovement.Players.Count; i++)
                {
                    indexUsedList.Add(i);
                }
                for (int i = 0; i < m_PlayerMovement.Players.Count; i++)
                {
                    int index = Random.Range(0, indexUsedList.Count);
                    int PlayerIndex = indexUsedList[index];
                    indexUsedList.RemoveAt(index);

                    PlayerMovement pm = m_PlayerMovement.Players[PlayerIndex];
                    if (pm && pm != GetComponent<PlayerMovement>())
                    {
                        targetPlayerIndex = PlayerIndex;
                        break;
                    }
                }
                indexUsedList.Clear();

                Debug.Log("UseAssassination");
                Debug.Log(TotalNum);
                Debug.Log(targetPlayerIndex);
                if(targetPlayerIndex < 0)
                {
                    //本地
                    Player.GetComponent<SpriteRenderer>().color = pickHightLightColor;
                }
                else
                {
                    PlayerMovement pm = m_PlayerMovement.Players[targetPlayerIndex];
                    pm.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                }
                m_ArrowTransform.gameObject.SetActive(true);
            }
        }

        private void UpdateArrow()
        {
            if (timer_Assassination>0)
            {
                timer_Assassination -= Time.deltaTime;
                Vector3 TargetPos = Vector3.zero;
                if (targetPlayerIndex>=0 && targetPlayerIndex< m_PlayerMovement.Players.Count )
                {
                    if(m_PlayerMovement.Players[targetPlayerIndex])
                    {
                        TargetPos = m_PlayerMovement.Players[targetPlayerIndex].gameObject.transform.position;
                        Debug.Log(targetPlayerIndex);
                        if (timer_Assassination <= 0)
                        {
                            PlayerMovement pm = m_PlayerMovement.Players[targetPlayerIndex];
                            pm.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                            
                        }
                    }
                    else
                    {
                        //如果人物中途死掉了
                        timer_Assassination = 0;
                    }
                }
                Vector3 Direction = TargetPos - m_Transform.position;
                m_ArrowTransform.right = Vector3.Normalize(new Vector3(Direction.x, 0, Direction.z));
                m_ArrowTransform.position = m_Transform.position + m_ArrowTransform.right;
                m_ArrowTransform.Rotate(new Vector3(90, 0, 0));
                Debug.Log(TargetPos);
                Debug.Log(m_Transform.position);


                if (timer_Assassination <= 0)
                {
                    m_ArrowTransform.gameObject.SetActive(false);
                    if(targetPlayerIndex == -1)
                    {
                        //本地
                        Player.GetComponent<SpriteRenderer>().color =Color.white;
                    }
                    else
                    {
                        targetPlayerIndex = -1;
                    }
                }
            }
        }

        public void PassiveItemUse()
        {
            if (m_ItemState == ItemState.Owning)
            {
                if (!isActivateItem)
                {
                    
                    m_ItemState = ItemState.Nothing;
                    if (itemName == ItemName.GoldenSilk)
                    {
                        if (Player)
                        {
                            Player.GetComponent<PlayerMovement>().PlayAudio(ItemAudio);
                            Debug.Log("金丝软甲破碎特效");
                        }
                    }
                    itemName = ItemName.Others;
                    ShowBtnPick();
                }
            }
        }

        public void OnPlayerLeaveItem()
        {
            if (m_ItemState == ItemState.WaitPick)
            {
                NormalBtnPick();
                m_ItemState = ItemState.Nothing;
            }
        }

        public void OnPlayerStayItem(GameObject go)
        {
            if (m_ItemState == ItemState.Nothing)
            {
                HighlightBtnPick();
                m_ItemState = ItemState.WaitPick;
            }
            if (m_ItemState == ItemState.Picking)
            {
                ItemPrefab Item = go.GetComponent<ItemPrefab>();
                if(PhotonNetwork.IsConnected == true)
                {
                    PhotonView m = go.GetPhotonView();
                    int pvid = m.ViewID;
                    PhotonView goPV = PhotonView.Find(pvid);
                    Item = goPV.gameObject.GetComponent<ItemPrefab>();
                }

                m_ItemState = ItemState.Owning;
                SetItemData(Item);
                NormalBtnPick();
                
                ShowBtnUse(isActivateItem,  itemCount);
                manager_Item.ItemDestroy(go);
            }
        }

        private void SetItemData(ItemPrefab InItemBase)
        {
            isActivateItem = (InItemBase.Itemtype == ItemType.ActiveItem);
            itemCount = InItemBase.UsageCount;
            itemName = InItemBase.ItemName;
            UITexture = InItemBase.UItexture;
            ItemAudio = InItemBase.AudioClip;
            assassinationCDTime = InItemBase.AssassinationTime;
        }

        public void HighlightBtnPick()
        {
            btn_Pick.interactable = true;
            btn_Pick.GetComponent<Image>().color = pickHightLightColor;
        }

        public void NormalBtnPick()
        {
            btn_Pick.interactable = false;
            btn_Pick.GetComponent<Image>().color = Color.white;
        }

        public void ShowBtnPick()
        {
            btn_Pick.interactable = false;
            btn_Pick.gameObject.SetActive(true);
            btn_Use.gameObject.SetActive(false);
            img_ActivateItemNum.gameObject.SetActive(false);
        }

        public void ShowBtnUse(bool isActivateItem,  int itemCount)
        {
            btn_Pick.gameObject.SetActive(false);
            btn_Use.gameObject.SetActive(true);

            img_BtnUseBG.sprite = UITexture;
            if (isActivateItem)
            {
                btn_Use.interactable = true;
                img_ActivateItemNum.gameObject.SetActive(true);
                txt_ActivateItemNum.text = string.Format("{0}", itemCount);

                img_BtnUseBG.color = pickHightLightColor;
            }
            else
            {
                btn_Use.interactable = false;

                img_ActivateItemNum.gameObject.SetActive(false);
                img_BtnUseBG.color = Color.white;
            }
        }

        private void OnTriggerExit(Collider coll)
        {
            
            if (PhotonNetwork.IsConnected == true && photonView.IsMine == false)
            {
                return;
            }
            if (coll.tag == "Items")
            {
                OnPlayerLeaveItem();
            }
        }

        private void OnTriggerStay(Collider coll)
        {
            if (PhotonNetwork.IsConnected == true && photonView.IsMine == false)
            {
                return;
            }
            if (coll.tag == "Items")
            {
                OnPlayerStayItem(coll.gameObject);
            }
        }
    }
}