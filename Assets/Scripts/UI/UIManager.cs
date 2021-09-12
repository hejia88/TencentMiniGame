using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Color pickHightLightColor;

    private Transform m_Transform;

    private Transform panel_ItemsTransform;
    private Transform panel_HealthTransform;

    private Button btn_Pick;
    private Button btn_Use;
    private Image img_ActivateItemNum;
    private Image img_BtnUseBG;
    private Text txt_ActivateItemNum;
    private Text txt_Health;
    private Text txt_Lives;

    private Character m_localPlayer;
    private ItemManager manager_ItemScene;
    private PlayerItem manager_ItemPlayer;
    private PlayerHealth manager_PlayerHealth;



    void Awake()
    {
        m_Transform = gameObject.GetComponent<Transform>();

        

        panel_ItemsTransform = m_Transform.Find("ItemsPanel");
        panel_HealthTransform = m_Transform.Find("HealthPanel");

        btn_Pick = panel_ItemsTransform.Find("btn_Pick").GetComponent<Button>();
        btn_Use = panel_ItemsTransform.Find("btn_Use").GetComponent<Button>();
        img_BtnUseBG = btn_Use.gameObject.GetComponent<Image>();
        img_ActivateItemNum = panel_ItemsTransform.Find("img_ItemNum").GetComponent<Image>();
        txt_ActivateItemNum = img_ActivateItemNum.transform.Find("Text").GetComponent<Text>();

        txt_Lives = panel_HealthTransform.Find("Lives").GetComponent<Text>();
        txt_Health = panel_HealthTransform.Find("Health").GetComponent<Text>();

        m_localPlayer = GameObject.Find("Character").GetComponent<Character>();
        manager_ItemScene = GameObject.Find("ItemManager").GetComponent<ItemManager>();
        manager_ItemPlayer = GameObject.Find("PlayerManager").GetComponent<PlayerItem>();
        manager_PlayerHealth = GameObject.Find("PlayerManager").GetComponent<PlayerHealth>();




        btn_Use.gameObject.SetActive(false);
        img_ActivateItemNum.GetComponent<Image>().color = pickHightLightColor;
        img_ActivateItemNum.gameObject.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {
        btn_Pick.onClick.AddListener(OnBtnPickClick);
        btn_Use.onClick.AddListener(OnBtnUseClick);

        txt_Lives.text = string.Format("{0}", manager_PlayerHealth.CurrentLives);
        txt_Health.text = string.Format("{0}", manager_PlayerHealth.CurrentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBtnPickClick()
    {
        manager_ItemPlayer.PickItem();
    }
    private void OnBtnUseClick()
    {
        manager_ItemPlayer.UseActivateItem();
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
    public void ShowBtnUse(bool isActivateItem,int itemIndex,int itemCount)
    {
        btn_Pick.gameObject.SetActive(false);
        btn_Use.gameObject.SetActive(true);

        img_BtnUseBG.sprite = manager_ItemScene.list_UITexture[itemIndex];
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
    public void UpdateHealth(int Health)
    {
        txt_Health.text = string.Format("{0}", manager_PlayerHealth.CurrentHealth);
    }

    public void UpdateLives(int lives)
    {
        txt_Lives.text = string.Format("{0}", manager_PlayerHealth.CurrentLives);
    }
}
