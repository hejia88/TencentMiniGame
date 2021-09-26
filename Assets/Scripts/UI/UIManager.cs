using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Tencent.DYYS
{
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
        }

        // Start is called before the first frame update

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
        public void ShowBtnUse(bool isActivateItem, int itemIndex, int itemCount)
        {
            btn_Pick.gameObject.SetActive(false);
            btn_Use.gameObject.SetActive(true);

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
    }
}
