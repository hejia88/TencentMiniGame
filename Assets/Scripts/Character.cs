using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;

public class Character : MonoBehaviourPun
{
    private Transform m_Transform;
    private Rigidbody m_Rigidbody;

    public float moveSpeed;
    public static GameObject LocalPlayerInstance;


    void Awake()
    {
        if (photonView.IsMine)
        {
            Character.LocalPlayerInstance = this.gameObject;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if(horizontal!=0 || vertical!=0)
        {
            Vector3 moveDirection = vertical * Vector3.forward + horizontal * Vector3.right;
            m_Rigidbody.MovePosition(m_Transform.position + moveSpeed * Time.fixedDeltaTime * moveDirection);
        }
        
    }
}
