using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Color pickHightLightColor;

    private Transform m_Transform;

    private Transform panel_ItemsTransform;

    private Button btn_Pick;
    private Button btn_Use;
    private Image img_ActivateItemNum;
    private Image img_BtnUseBG;
    private Text txt_ActivateItemNum;

    private Character m_localPlayer;
    private ItemManager manager_Item;



    void Awake()
    {
        m_Transform = gameObject.GetComponent<Transform>();

        

        panel_ItemsTransform = m_Transform.Find("ItemsPanel");

        btn_Pick = panel_ItemsTransform.Find("btn_Pick").GetComponent<Button>();
        btn_Use = panel_ItemsTransform.Find("btn_Use").GetComponent<Button>();
        img_BtnUseBG = btn_Use.gameObject.GetComponent<Image>();
        img_ActivateItemNum = panel_ItemsTransform.Find("img_ItemNum").GetComponent<Image>();
        txt_ActivateItemNum = img_ActivateItemNum.transform.Find("Text").GetComponent<Text>();

        m_localPlayer = GameObject.Find("Character").GetComponent<Character>();
        manager_Item = GameObject.Find("ItemManager").GetComponent<ItemManager>();

        


        btn_Use.gameObject.SetActive(false);
        img_ActivateItemNum.GetComponent<Image>().color = pickHightLightColor;
        img_ActivateItemNum.gameObject.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {
        btn_Pick.onClick.AddListener(OnBtnPickClick);
        btn_Use.onClick.AddListener(OnBtnUseClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBtnPickClick()
    {
        m_localPlayer.PickItem();
    }
    private void OnBtnUseClick()
    {
        m_localPlayer.UseItem();
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
    public void ShowBtnUse(ItemData InItemData)
    {
        btn_Pick.gameObject.SetActive(false);
        btn_Use.gameObject.SetActive(true);

        img_BtnUseBG.sprite = manager_Item.list_UITexture[InItemData.ItemIndex];
        if (InItemData.IsActivateItem)
        {
            btn_Use.interactable = true;
            img_ActivateItemNum.gameObject.SetActive(true);
            txt_ActivateItemNum.text = string.Format("{0}", InItemData.ItemCount);
            
            img_BtnUseBG.color = pickHightLightColor;
        }
        else
        {
            btn_Use.interactable = false;
            
            img_ActivateItemNum.gameObject.SetActive(false);
            img_BtnUseBG.color = Color.white;
        }
    }
}
