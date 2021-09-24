using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Random = UnityEngine.Random;

namespace Com.Tencent.DYYS
{
    public class AIMovement : MonoBehaviourPun
    {
        public enum PLAYER_DIRECTIONS
        {
            UP,
            UPRIGHT,
            RIGHT,
            BOTTOMRIGHT,
            BOTTOM,
            BOTTOMLEFT,
            LEFT,
            UPLEFT,
        }

        public enum STATES
        {
            IDLE,
            WALKING,
            RUNNING,
            DEATH,
        }

        //Animations
        //----------------------------
        private Animator anim;
        private STATES state = STATES.IDLE;
        private Vector3 dir = Vector3.right;
        private Color lockedColor = new Color(0.990566f, 0.2669989f, 0.4508103f);
        private Color normalColor = new Color(1.0f, 1.0f, 1.0f);
        [HideInInspector] public bool isLocked = false;
        private SpriteRenderer spriteRenderer;

        [HideInInspector] public bool isDead = false;
        //----------------------------

        //Movement Parameters
        //----------------------------
        private Rigidbody rb;
        public float WonderTime = 2.0f;
        private float WonderTimer = 0.0f;
        public float WonderTimeOffset = 0.5f;
        private float WonderOffset = 0.5f;
        public float IdleTime = 1.0f;
        private float IdleTimer = 0.0f;
        public float IdleTimeOffset = 0.5f;
        private float IdleOffset = 0.5f;
        public float RotationDegree = 30.0f;
        public float RotationDegreeOffset = 10.0f;
        private float RotationOffset = 10.0f;

        public float wonderSpeed = 2.5f;
        //----------------------------

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
            dir = Quaternion.Euler(0, RotationDegree + RotationOffset, 0) * dir;
            RotationOffset = Random.Range(-RotationDegreeOffset, RotationDegreeOffset);
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PlayerMovement.AIs = FindObjectsOfType<AIMovement>();
                int cnt = 0;
                foreach (var ai in PlayerMovement.AIs)
                {
                    Debug.LogFormat("CurrAi {0} is {1}", ++ cnt, ai);
                }
            }
        }

        void Update()
        {
            if (isDead) return;

            if (isLocked && spriteRenderer.color != lockedColor)
            {
                spriteRenderer.color = lockedColor;
            }
            else if (!isLocked && spriteRenderer.color != normalColor)
            {
                spriteRenderer.color = normalColor;
            }

            //变色逻辑是单客户端的，移动逻辑是全局的，故在此判断 -shanexchen
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (state == STATES.IDLE)
            {
                IdleTimer += Time.deltaTime;
                if (IdleTimer >= IdleTime + IdleOffset)
                {
                    IdleTimer -= IdleTime;
                    state = STATES.WALKING;
                    IdleOffset = Random.Range(-IdleTimeOffset, IdleTimeOffset);
                    dir = Quaternion.AngleAxis(RotationDegree + RotationOffset, Vector3.up) * dir;
                    RotationOffset = Random.Range(-RotationDegreeOffset, RotationDegreeOffset);
                }
                //rb.velocity = Vector2.zero;
            }

            if (state == STATES.WALKING)
            {
                WonderTimer += Time.deltaTime;
                if (WonderTimer >= WonderTime + WonderOffset)
                {
                    WonderTimer -= WonderTime;
                    state = STATES.IDLE;
                    WonderOffset = Random.Range(-WonderTimeOffset, WonderTimeOffset);
                }

                //rb.velocity = dir * wonderSpeed;
                rb.MovePosition(transform.position + wonderSpeed * Time.fixedDeltaTime * dir);
                transform.rotation = dir.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            }
        }

        public void Die()
        {
            PhotonView m_PhotonView = PhotonView.Get(this);
            if (PhotonNetwork.IsConnected && m_PhotonView != null)
            {
                m_PhotonView.RPC("FinishedDie", RpcTarget.AllViaServer);
            }
            else
            {
                FinishedDie();
            }
        }

        [PunRPC]
        public void FinishedDie()
        {
            //TODO:等待动画接入后加入Animator
            spriteRenderer.color = normalColor;
            isDead = true;
            //anim.SetBool("IsDead", true);

            Destroy(this, 0.5f);
            Destroy(gameObject, 0.5f); //Temporary
            Destroy(GetComponent<Collider>());
            Destroy(GetComponent<Rigidbody>());

            //Destroy(GetComponent<Animator>(), 2.0f);
        }
    }
}