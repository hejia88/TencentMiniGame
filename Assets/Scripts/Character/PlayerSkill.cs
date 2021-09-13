using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum JobType
{
    StickMen,
    BusinessMen,
    PoliceMen,
    FortuneTeller
}

public class PlayerSkill : MonoBehaviourPun
{
    [HideInInspector] public JobType jobType;

    void Start()
    {
        
    }

    void Update()
    {
        if (PhotonNetwork.IsConnected == true && photonView.IsMine == false)
        {
            return;
        }
    }


    public void UseSkill()
    {
        Debug.Log("UseSkill");
    }
}
