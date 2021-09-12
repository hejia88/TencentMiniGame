using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public int MaxHealth;
    public int MaxLives;
    public GameObject LocalPlayer;

    public List<GameObject>  Prefab_PlayerList;

    [HideInInspector] public int CurrentHealth;
    [HideInInspector] public int CurrentLives;

    private Transform m_Transform;

    private UIManager manager_UI;
    private PlayerItem manager_ItemPlayer;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        CurrentLives = MaxLives;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();

        manager_UI = GameObject.Find("UICanvas").GetComponent<UIManager>();
        manager_ItemPlayer = m_Transform.GetComponent<PlayerItem>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Spawn();
        }
    }

    public void TakeDamage(int DamageAmount)
    {
        if (CurrentHealth > 0)
        {
            if(manager_ItemPlayer.itemName==ItemName.GoldenSilk)
            {
                manager_ItemPlayer.PassiveItemUse();
                return;
            }
            CurrentHealth -= DamageAmount;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Die();
            }
            manager_UI.UpdateHealth(CurrentHealth);
        }
    }
    public void Spawn()
    {
        if (CurrentLives > 0)
        {
            CurrentHealth = MaxHealth;
            int ItemIndex = Random.Range(0, Prefab_PlayerList.Count);
            LocalPlayer = GameObject.Instantiate(Prefab_PlayerList[ItemIndex], m_Transform);
            manager_UI.UpdateHealth(CurrentHealth);
        }
    }

    private void Die()
    {
        if(CurrentLives>0)
        {
            CurrentLives -= 1;
            LocalPlayer.GetComponent<Character>().PlayerDie();
            LocalPlayer = null;
            manager_UI.UpdateLives(CurrentLives);
        }
    }
}
