using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player_Transform;
    private Transform m_Transform;

    public float distance;

    // Start is called before the first frame update
    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();


    }

    // Update is called once per frame
    void Update()
    {
        m_Transform.position = player_Transform.position - distance * m_Transform.forward;
    }
}
