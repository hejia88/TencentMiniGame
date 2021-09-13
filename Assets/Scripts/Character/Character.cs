using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Character : MonoBehaviourPun
{
    public JobType m_JobType;

    private Transform m_Transform;
    private Rigidbody m_Rigidbody;

    public float moveSpeed;

    private PlayerItem manager_Item;
    private PlayerHealth manager_Health;

    public static GameObject LocalPlayerInstance;

    void Awake()
    {
        if (PhotonNetwork.IsConnected == true)
        {
            if (photonView.IsMine == true)
            {
                Character.LocalPlayerInstance = this.gameObject;
            }
            DontDestroyOnLoad(this.gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();

        manager_Item = GameObject.Find("PlayerManager").GetComponent<PlayerItem>();
        manager_Health = GameObject.Find("PlayerManager").GetComponent<PlayerHealth>();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected == true && photonView.IsMine == false)
        {
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 moveDirection = vertical * Vector3.forward + horizontal * Vector3.right;
            m_Rigidbody.MovePosition(m_Transform.position + moveSpeed * Time.fixedDeltaTime * moveDirection);
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            manager_Health.TakeDamage(1);
        }

    }


    private void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "Items")
        {
            manager_Item.OnPlayerLeaveItem();
        }
    }

    private void OnTriggerStay(Collider coll)
    {
        if (coll.tag == "Items")
        {
             manager_Item.OnPlayerStayItem(coll.gameObject);
        }
    }

    private void PlayerTakeDamege(int DamageAmount)
    {
        manager_Health.TakeDamage(DamageAmount);
    }

    public void PlayerDie()
    {
        Debug.Log("PlayerDie");
        GameObject.Destroy(gameObject);
        
    }
}
