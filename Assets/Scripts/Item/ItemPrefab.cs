using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum ItemType
{
    ActiveItem,
    PassiveItem
}
public enum ItemName
{
    Others,
    GoldenSilk
}

namespace Com.Tencent.DYYS
{
    public class ItemPrefab : MonoBehaviourPun
    {

        [Header("技能分类")]
        public ItemType Itemtype;

        [Header("主动道具使用次数")]
        [Tooltip("仅在m_Itemtype=ActiveItem时配置有效")]
        public int UsageCount;

        [Header("拾取半径")]
        public float PickDistance;

        [Header("UI资源")]
        public Sprite UItexture;

        [Header("声音资源")]
        public AudioClip AudioClip;

        public ItemName ItemName;

        private BoxCollider m_boxCollider;

        private ItemManager manager_Item;

        public ItemManager Manager_Item
        {
            set { manager_Item = value; }
        }

        // Start is called before the first frame update
        void Start()
        {
            m_boxCollider = gameObject.GetComponent<BoxCollider>();

            m_boxCollider.size = new Vector3(PickDistance, PickDistance, PickDistance);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
