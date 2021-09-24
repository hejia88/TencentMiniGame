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
        private ItemManager manager_Item;
        public ItemState m_ItemState;
        public GameObject Player;
        public Button btn_Pick;
        public Button btn_Use;
        public Image img_ActivateItemNum;
        public Image img_BtnUseBG;
        public Text txt_ActivateItemNum;
        public Color pickHightLightColor;
        public Transform m_PanelTransform;

        public ItemState PickState
        {
            get { return m_ItemState; }
        }

        [HideInInspector] public bool isActivateItem;
        [HideInInspector] public int itemCount;
        [HideInInspector] public int itemIndex;
        [HideInInspector] public ItemName itemName;

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
            m_PanelTransform.gameObject.SetActive(false);
        }

        void Start()
        {
            if (PhotonNetwork.IsConnected == true && photonView.IsMine == true)
            {
                m_PanelTransform.gameObject.SetActive(true);
            }
            manager_Item = FindObjectOfType<ItemManager>();
            m_ItemState = ItemState.Nothing;
            isActivateItem = false;
            itemCount = 0;
            itemIndex = 0;
            itemName = ItemName.Others;
            btn_Pick.onClick.AddListener(OnBtnPickClick);
            btn_Use.onClick.AddListener(OnBtnUseClick);
            //NormalBtnPick();
        }

        void Update()
        {
            if (PhotonNetwork.IsConnected == true && photonView.IsMine == false)
            {
                return;
            }
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
                            ShowBtnUse(isActivateItem, itemIndex, itemCount);
                        }
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
                    ShowBtnPick();
                    m_ItemState = ItemState.Nothing;
                    if (itemName == ItemName.GoldenSilk)
                    {
                        if (Player)
                        {
                            Player.GetComponent<PlayerMovement>().PlayAudio(manager_Item.list_AudioClip[itemIndex]);
                        }
                    }
                    itemName = ItemName.Others;
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

                SetItemData(Item);
                m_ItemState = ItemState.Owning;

                NormalBtnPick();
                Debug.Log(string.Format("{0},{1},{2}", isActivateItem, itemIndex, itemCount));
                ShowBtnUse(isActivateItem, itemIndex, itemCount);
                manager_Item.ItemDestroy(go);
            }
        }

        private void SetItemData(ItemPrefab InItemBase)
        {
            isActivateItem = (InItemBase.Itemtype == ItemType.ActiveItem);
            itemCount = InItemBase.UsageCount;
            itemIndex = InItemBase.IndexResource;
            itemName = InItemBase.ItemName;
        }

        public void HighlightBtnPick()
        {
            btn_Pick.interactable = true;
            btn_Pick.GetComponent<Image>().color = pickHightLightColor;
        }

        public void NormalBtnPick()
        {
            Debug.Log("OnTriggerStay");
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

        public void ShowBtnUse(bool isActivateItem, int itemIndex, int itemCount)
        {
            btn_Pick.gameObject.SetActive(false);
            btn_Use.gameObject.SetActive(true);

            img_BtnUseBG.sprite = manager_Item.list_UITexture[itemIndex];
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