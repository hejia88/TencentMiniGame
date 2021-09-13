using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform m_Transform;

    public float distance;

    private PlayerHealth manager_PlayerHealth;

    // Start is called before the first frame update
    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();

        manager_PlayerHealth = GameObject.Find("PlayerManager").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if(manager_PlayerHealth.Player)
        {
            Transform player_Transform = manager_PlayerHealth.Player.GetComponent<Transform>();
            m_Transform.position = player_Transform.position - distance * m_Transform.forward;
        }
        
    }
}
