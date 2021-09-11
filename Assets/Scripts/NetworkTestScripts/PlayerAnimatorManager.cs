using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerAnimatorManager : MonoBehaviourPun
{

    #region Private Fields

    [SerializeField]
    private float directionDampTime = 0.25f;

    private Animator animator;

    

    #endregion

    #region MonoBehaviour Callbacks
    
    void Awake()
    {
        
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        if (!animator)
        {
            Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
        }
    }

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        if (!animator)
        {
            return;
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (v < 0)
        {
            v = 0;
        }
        animator.SetFloat("Speed", h * h + v * v);
        animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
    }

    #endregion
}
