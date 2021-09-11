using System.Collections;
using System.Collections.Generic;

using Photon.Pun;

using UnityEngine;

public class NWT_AIController : MonoBehaviourPun
{
    private Transform m_Transform;
    private Rigidbody m_Rigidbody;
    private PhotonView m_PhotonView;

    public float moveSpeed;

    private float totTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        m_PhotonView = PhotonView.Get(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            totTime += Time.deltaTime;
            if (totTime >= 0.5f)
            {
                totTime = 0.0f;
                //m_PhotonView.RPC("UpdateAITransform", RpcTarget.AllViaServer);
                UpdateAITransform();
            }
        }
    }

    //[PunRPC]
    protected void UpdateAITransform()
    {
        float horizontal = Random.Range(-1.0f, 1.0f);
        float vertical = Random.Range(-1.0f, 1.0f);

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 moveDirection = vertical * Vector3.forward + horizontal * Vector3.right;
            m_Rigidbody.MovePosition(m_Transform.position + moveSpeed * Time.fixedDeltaTime * moveDirection);
        }
    }
}
