using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ItemState
{
    Nothing,
    WaitPick,
    Picking,
    Owning
}

public class Character : MonoBehaviour
{
    private Transform m_Transform;
    private Rigidbody m_Rigidbody;

    public float moveSpeed;

    private UIManager manager_UI;
    private ItemManager manager_Item;

    private ItemState m_ItemState;
    private ItemData m_Item;


    // Start is called before the first frame update
    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();

        manager_UI = GameObject.Find("UICanvas").GetComponent<UIManager>();
        manager_Item = GameObject.Find("ItemManager").GetComponent<ItemManager>();

        m_Item = new ItemData();

        m_ItemState = ItemState.Nothing;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if(horizontal!=0 || vertical!=0)
        {
            Vector3 moveDirection = vertical * Vector3.forward + horizontal * Vector3.right;
            m_Rigidbody.MovePosition(m_Transform.position + moveSpeed * Time.fixedDeltaTime * moveDirection);
        }
        
    }

    public void PickItem()
    {
        m_ItemState = ItemState.Picking;
    }
    public void UseItem()
    {
        if(m_ItemState==ItemState.Owning)
        {
            if(m_Item.IsActivateItem)
            {
                if(m_Item.ItemCount>0)
                {
                    m_Item.ItemCount -= 1;
                    if(m_Item.ItemCount == 0)
                    {
                        manager_UI.ShowBtnPick();
                        m_ItemState = ItemState.Nothing;
                    }
                    else
                    {
                        manager_UI.ShowBtnUse(m_Item);
                    }
                }
            }
        }
    }



    private void OnTriggerEnter(Collider coll)
    {
        if (m_ItemState != ItemState.Owning)
        {
            Debug.Log(coll.tag);
            if (coll.tag == "Items")
            {
                GameObject go = coll.gameObject;

                manager_UI.HighlightBtnPick();
            }
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "Items")
        {
            if(m_ItemState == ItemState.WaitPick)
            {
                manager_UI.NormalBtnPick();
                m_ItemState = ItemState.Nothing;
            }
        }
    }

    private void OnTriggerStay(Collider coll)
    {
        if (coll.tag == "Items")
        {
            if (m_ItemState == ItemState.Nothing)
            {
                manager_UI.HighlightBtnPick();
                m_ItemState = ItemState.WaitPick;
            }
            if (m_ItemState ==ItemState.Picking)
            {
                ItemPrefab Item =coll.gameObject.GetComponent<ItemPrefab>();
                m_Item.SetItemData(Item);
                m_ItemState = ItemState.Owning;

                manager_UI.NormalBtnPick();
                manager_UI.ShowBtnUse(m_Item);


                manager_Item.ItemDestroy(coll.gameObject);
            }
        }
    }
}
