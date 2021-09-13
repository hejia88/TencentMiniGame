using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
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
    private PLAYER_DIRECTIONS currentDir = 0;
    private bool isRunning = false;
    private float Direction;
    //----------------------------

    //Joystick Controls
    //----------------------------
    private bool IsDragging = false;
    private Vector2 StartPoint;
    private Vector2 EndPoint;
    [HideInInspector] public float HorizontalInput;
    private float VerticalInput;
    public GameObject InnerCircle;
    public GameObject LeftOuterCircle;
    public GameObject RightOuterCircle;
    //----------------------------


    //Movement Parameters
    //----------------------------
    private Rigidbody rb;
    public GameObject player;
    private Vector3 dir = Vector2.up;
    public float playerSpeed = 5.0f;
    public float wonderSpeed = 2.5f;
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
    //----------------------------

    //Attack
    //----------------------------
    [HideInInspector] 
    public static AIMovement[] AIs;
    [HideInInspector] 
    public AIMovement nearestAI;
    private float distance = 100000;
    public float range = 3.0f;
    //----------------------------

    //For Test Only
    //----------------------------
    public Button attackButton;
    private Vector3 mousePos;
    //----------------------------

    //Audio
    //----------------------------
    private AudioSource m_AudioSource;
    //----------------------------
    
    //Back end Synchronize
    //----------------------------
    public static GameObject LocalPlayerInstance;
    //----------------------------

    void Awake()
    {
        if (PhotonNetwork.IsConnected == true)
        {
            if (photonView.IsMine == true)
            {
                PlayerMovement.LocalPlayerInstance = this.gameObject;
            }
            DontDestroyOnLoad(this.gameObject);
        }
        ShowJoyStick(false);
    }

    private void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        anim = player.GetComponent<Animator>();
        AIs = FindObjectsOfType<AIMovement>();
        m_AudioSource = player.GetComponent<AudioSource>();
    }
    void Update()
    {
        if (PhotonNetwork.IsConnected == true && photonView.IsMine == false)
        {
            return;
        }

        //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = Input.mousePosition;
        #region Joystick Position
        if (Input.GetMouseButtonDown(0)) //鼠标按下时
        {
            StartPoint = new Vector2(mousePos.x, mousePos.y);
            LeftOuterCircle.transform.position = StartPoint - new Vector2(75, 0);
            RightOuterCircle.transform.position = StartPoint + new Vector2(75, 0);
            InnerCircle.transform.position = StartPoint;
            ShowJoyStick(true);
            IsDragging = true;
        }
        if (Input.GetMouseButton(0) && IsDragging)
        {
            EndPoint = new Vector2(mousePos.x, mousePos.y);
        }
        if (Input.GetMouseButtonUp(0) && IsDragging)
        {
            IsDragging = false;
            HorizontalInput = 0.0f;
            VerticalInput = 0.0f;
            state = STATES.IDLE;
            IdleTimer = 0.0f;
            WonderTimer = 0.0f;
        }
        if (IsDragging)
        {
            Vector2 offset = EndPoint - StartPoint;
            isRunning = offset.magnitude > 0.35f ? true : false;  //当处于外轮盘时选择方向,当处于内轮盘时取消移动
            Direction = Quaternion.FromToRotation(offset, new Vector2(0, 1)).eulerAngles.z;
            if (Direction > 337.5f || Direction < 22.5f)//上
            {
                currentDir = PLAYER_DIRECTIONS.UP;
                HorizontalInput = 0.0f;
                VerticalInput = 1.0f;
            }
            else if (Direction > 22.5f && Direction < 67.5f)//右上
            {
                currentDir = PLAYER_DIRECTIONS.UPRIGHT;
                HorizontalInput = 1.0f;
                VerticalInput = 1.0f;
            }
            else if (Direction > 67.5f && Direction < 112.5f)//右
            {
                currentDir = PLAYER_DIRECTIONS.RIGHT;
                HorizontalInput = 1.0f;
                VerticalInput = 0.0f;
            }
            else if (Direction > 112.5f && Direction < 157.5f)//右下
            {
                currentDir = PLAYER_DIRECTIONS.BOTTOMRIGHT;
                HorizontalInput = 1.0f;
                VerticalInput = -1.0f;
            }
            else if (Direction > 157.5f && Direction < 202.5f)//下
            {
                currentDir = PLAYER_DIRECTIONS.BOTTOM;
                HorizontalInput = 0.0f;
                VerticalInput = -1.0f;
            }
            else if (Direction > 202.5f && Direction < 247.5f)//左下
            {
                currentDir = PLAYER_DIRECTIONS.BOTTOMLEFT;
                HorizontalInput = -1.0f;
                VerticalInput = -1.0f;
            }
            else if (Direction > 247.5f && Direction < 292.5f)//左
            {
                currentDir = PLAYER_DIRECTIONS.LEFT;
                HorizontalInput = -1.0f;
                VerticalInput = 0.0f;
            }
            else if (Direction > 292.5f && Direction < 337.5f)//左上
            {
                currentDir = PLAYER_DIRECTIONS.UPLEFT;
                HorizontalInput = -1.0f;
                VerticalInput = 1.0f;
            }
            Vector2 dir = Vector2.ClampMagnitude(offset, 150.0f);
            InnerCircle.transform.position = new Vector2(StartPoint.x + dir.x, StartPoint.y + dir.y);
        }
        else
        {
            ShowJoyStick(false);
        }
        #endregion
        #region PlayerMovement
        if (HorizontalInput != 0.0f || VerticalInput != 0.0f)
        {
            //rb.velocity = new Vector3(HorizontalInput, VerticalInput).normalized * playerSpeed;
            Vector3 moveDirection = VerticalInput * Vector3.forward + HorizontalInput * Vector3.right;
            rb.MovePosition(player.transform.position + playerSpeed * Time.fixedDeltaTime * moveDirection);

            //TODO:加入相机跟随逻辑
            //临时移除玩家转向
            //player.transform.rotation = HorizontalInput < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        }
        else
        {
            //AI移动逻辑
            if (state == STATES.IDLE)
            {
                IdleTimer += Time.deltaTime;
                if (IdleTimer >= IdleTime + IdleOffset)
                {
                    IdleTimer -= IdleTime;
                    state = STATES.WALKING;
                    IdleOffset = Random.Range(-IdleTimeOffset, IdleTimeOffset);
                    dir = Quaternion.AngleAxis(RotationDegree + RotationOffset, Vector3.forward) * dir;
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
                rb.MovePosition(player.transform.position + wonderSpeed * Time.fixedDeltaTime * dir);
                //临时移除玩家转向
                //player.transform.rotation = dir.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            }
        }
        #endregion
        #region PlayerAttack
        GetNearest();
        foreach (AIMovement ai in AIs)
        {
            if(ai == null)
            {
                continue;
            }
            if (ai == nearestAI)
            {
                ai.isLocked = true;
                continue;
            }
            ai.isLocked = false;
        }
//         if (nearestAI != null && Input.GetKeyDown(KeyCode.E))
//         {
//             anim.SetTrigger("Attack");
//             nearestAI.Die();
//             FadeToColor(attackButton.colors.pressedColor);
//         }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //TODO:等待动画接入后加入Animator
            //anim.SetTrigger("Attack");
            FadeToColor(attackButton.colors.pressedColor);
            if (nearestAI != null)
            {
                nearestAI.Die();
            }
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            FadeToColor(attackButton.colors.normalColor);
        }
        #endregion
    }

    void FadeToColor(Color color)
    {
        Graphic graphic = attackButton.GetComponent<Graphic>();
        graphic.CrossFadeColor(color, attackButton.colors.fadeDuration, true, true);
    }

    void GetNearest()
    {
        nearestAI = null;
        distance = 100000;
        if (AIs.Length == 0) return;
        foreach (AIMovement ai in AIs)
        {
            if(ai == null)
            {
                continue;
            }
            if (ai.isDead) continue;
            float dist = Vector2.Distance(ai.gameObject.transform.position, player.transform.position);
            if (dist > range) continue;
            bool flag = player.transform.rotation.y == 180;
            if (ai.gameObject.transform.position.x <= player.transform.position.x - 1 && !flag)
                continue;
            if (ai.gameObject.transform.position.x >= player.transform.position.x + 1 && flag)
                continue;
            if (dist <= distance)
            {
                distance = dist;
                nearestAI = ai;
            }
        }
    }

    public void PlayAudio(AudioClip audioClip)
    {
        if (audioClip)
        {
            m_AudioSource.clip = audioClip;
            m_AudioSource.Play();
        }
    }

    void ShowJoyStick(bool IsShow)
    {
        InnerCircle.GetComponent<Image>().enabled = IsShow;
        LeftOuterCircle.GetComponent<Image>().enabled = IsShow;
        RightOuterCircle.GetComponent<Image>().enabled = IsShow;
    }
}
