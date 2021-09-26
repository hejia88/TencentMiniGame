using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

namespace Com.Tencent.DYYS {
    public class Billboard : MonoBehaviourPun
    {
        //public Camera theCam;

        public bool useStaticBillboard;
        public GameObject theCMvcam;
        private Camera cmVcam;
        private PlayerMovement playerMovement;
        public CinemachineVirtualCamera virtualCameraInstance;


        // Start is called before the first frame update
        void Start()
        {
            //theCam = Camera.main;
            playerMovement = gameObject.GetComponentInParent<PlayerMovement>();
            virtualCameraInstance = playerMovement.VirtualCameraInstance;
         }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.IsConnected == true && photonView.IsMine == false)
            {
                return;
            }
            if (!useStaticBillboard)
            {
                transform.LookAt(virtualCameraInstance.transform);
            }
            else
            {
                transform.rotation = virtualCameraInstance.transform.rotation;
            }

            transform.rotation = Quaternion.Euler(38f, (transform.rotation.eulerAngles.y * 0.01f), 0f);
        }
    }
}