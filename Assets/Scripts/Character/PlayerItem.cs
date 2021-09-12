using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState
{
    Nothing,
    WaitPick,
    Picking,
    Owning
}


public class PlayerItem : MonoBehaviour
{
    private UIManager manager_UI;
    private ItemManager manager_Item;
    private PlayerHealth manager_PlayerHealth;

    private ItemState m_ItemState;

    public ItemState PickState
    {
        get { return m_ItemState; }
    }

    [HideInInspector] public bool isActivateItem;
    [HideInInspector] public int itemCount;
    [HideInInspector] public int itemIndex;
    [HideInInspector] public ItemName itemName;

    // Start is called before the first frame update
    void Start()
    {
        manager_UI = GameObject.Find("UICanvas").GetComponent<UIManager>();
        manager_Item = GameObject.Find("ItemManager").GetComponent<ItemManager>();
        manager_PlayerHealth = GameObject.Find("PlayerManager").GetComponent<PlayerHealth>();

        m_ItemState = ItemState.Nothing;

        isActivateItem = false;
        itemCount = 0;
        itemIndex = 0;
        itemName = ItemName.Others;
    }

    // Update is called once per frame
    void Update()
    {
        
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
                        manager_UI.ShowBtnPick();
                        m_ItemState = ItemState.Nothing;
                    }
                    else
                    {
                        manager_UI.ShowBtnUse(isActivateItem,itemIndex,itemCount);
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
                manager_UI.ShowBtnPick();
                m_ItemState = ItemState.Nothing;
                if(itemName == ItemName.GoldenSilk)
                {
                    Character LocalPlayer = manager_PlayerHealth.LocalPlayer.GetComponent<Character>();
                    if(LocalPlayer)
                    {
                        LocalPlayer.PlayAudio(manager_Item.list_AudioClip[itemIndex]);
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
            manager_UI.NormalBtnPick();
            m_ItemState = ItemState.Nothing;
        }
    }

    public void OnPlayerStayItem(GameObject go)
    {
        if (m_ItemState == ItemState.Nothing)
        {
            manager_UI.HighlightBtnPick();
            m_ItemState = ItemState.WaitPick;
        }
        if (m_ItemState == ItemState.Picking)
        {
            ItemPrefab Item = go.GetComponent<ItemPrefab>();

            SetItemData(Item);
            m_ItemState = ItemState.Owning;

            manager_UI.NormalBtnPick();
            Debug.Log(string.Format("{0},{1},{2}", isActivateItem, itemIndex, itemCount));
            manager_UI.ShowBtnUse(isActivateItem,itemIndex,itemCount);


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
}
