using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType
{
    ActiveItem,
    PassiveItem
}

public class ItemData
{
    private bool isActivateItem;
    private int itemCount;
    private int itemIndex;

    public bool IsActivateItem
    {
        get { return isActivateItem; }
    }

    public int ItemCount
    {
        get { return itemCount; }
        set { itemCount = value; }
    }

    public int ItemIndex
    {
        get { return itemIndex; }
    }

    public ItemData()
    {
        isActivateItem = false;
        itemCount = 0;
        itemIndex = 0;
    }
    public void SetItemData(ItemPrefab InItemBase)
    {
        isActivateItem = (InItemBase.Itemtype == ItemType.ActiveItem);
        itemCount = InItemBase.UsageCount;
        itemIndex = InItemBase.IndexResource;
    }

}
