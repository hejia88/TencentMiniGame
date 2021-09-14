using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

namespace Com.Tencent.DYYS
{
    public class PlayerHealth : MonoBehaviourPun
    {
        public int MaxHealth = 1;
        public GameObject Player;
        //  public List<GameObject>  Prefab_PlayerList;
        [HideInInspector] public int CurrentHealth;
        private PlayerItem Item;
        public Text HealthText;

        private void Awake()
        {
            CurrentHealth = MaxHealth;
        }

        void Start()
        {
            Item = GetComponent<PlayerItem>();
            //txt_Lives.text = string.Format("{0}", manager_PlayerHealth.CurrentLives);
            HealthText.text = string.Format("{0}", CurrentHealth);
        }

        void Update()
        {
            if (PhotonNetwork.IsConnected == true && photonView.IsMine == false)
            {
                return;
            }
        }

        public void TakeDamage(int DamageAmount)
        {
            if (CurrentHealth > 0)
            {
                if (Item.itemName == ItemName.GoldenSilk)
                {
                    Item.PassiveItemUse();
                    return;
                }
                CurrentHealth -= DamageAmount;
                if (CurrentHealth <= 0)
                {
                    CurrentHealth = 0;
                    Die();
                }
                HealthText.text = string.Format("{0}", CurrentHealth);
            }
        }

        //  public void Spawn()
        //  {
        //       CurrentHealth = MaxHealth;
        //       int ItemIndex = Random.Range(0, Prefab_PlayerList.Count);
        //  }

        private void Die()
        {
            //TODO: Add Animation
            //TODO: Call GameManager function to spawn new player prefab
        }
    }
}
