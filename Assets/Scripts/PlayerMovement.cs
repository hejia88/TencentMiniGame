using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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
    public bool TouchStart = false;
    private Vector2 StartPoint;
    private Vector2 EndPoint;
    private float HorizontalInput;
    private float VerticalInput;
    public GameObject InnerCircle;
    public GameObject LeftOuterCircle;
    public GameObject RightOuterCircle;
    private Vector3 mousePos;
    private PLAYER_DIRECTIONS currentDir = 0;
    private bool isRunning = false;
    private float Direction;
    public GameObject player;
    public float playerSpeed = 5.0f;
    public float wonderSpeed = 2.5f;
    public float WonderTime = 2.0f;
    private float WonderTimer = 0.0f;
    public float IdleTime = 1.0f;
    private float IdleTimer = 0.0f;
    private float timerOffset = 0.0f;
    private STATES state = STATES.IDLE;
    private Vector3 destination = Vector3.zero;
    private Vector3 dir = Vector2.zero;
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        #region Joystick Position
        if (Input.GetMouseButtonDown(0) && mousePos.x > -9 && mousePos.x < 9 && mousePos.y > -9 && mousePos.y < 9) //鼠标按下时(屏幕内)
        {
            transform.position = new Vector2(mousePos.x, mousePos.y);
            StartPoint = new Vector2(mousePos.x, mousePos.y);
            InnerCircle.transform.position = StartPoint;
            InnerCircle.GetComponent<SpriteRenderer>().enabled = true;
            LeftOuterCircle.GetComponent<SpriteRenderer>().enabled = true;
            RightOuterCircle.GetComponent<SpriteRenderer>().enabled = true;
            TouchStart = true;
        }
        else if (Input.GetMouseButtonDown(0) && (mousePos.x > 9 || mousePos.x < -9 || mousePos.y > -9 || mousePos.y < 9)) //鼠标按下时(屏幕外)
        {
            transform.position = new Vector2(Mathf.Clamp(mousePos.x, -9, 9), Mathf.Clamp(mousePos.y, -9, 9));
            StartPoint = new Vector2(mousePos.x, mousePos.y);
            InnerCircle.transform.position = StartPoint;
            InnerCircle.GetComponent<SpriteRenderer>().enabled = true;
            LeftOuterCircle.GetComponent<SpriteRenderer>().enabled = true;
            RightOuterCircle.GetComponent<SpriteRenderer>().enabled = true;
            TouchStart = true;
        }
        if (Input.GetMouseButton(0) && TouchStart)
        {
            EndPoint = new Vector2(mousePos.x, mousePos.y);
        }
        if (Input.GetMouseButtonUp(0) && TouchStart)
        {
            TouchStart = false;
            HorizontalInput = 0.0f;
            VerticalInput = 0.0f;
            state = STATES.IDLE;
            IdleTimer = 0.0f;
            WonderTimer = 0.0f;
        }
        #endregion
        if (TouchStart)
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
            Vector2 dir = Vector2.ClampMagnitude(offset, 1.75f);
            InnerCircle.transform.position = new Vector2(StartPoint.x + dir.x, StartPoint.y + dir.y);
        }
        else
        {
            InnerCircle.GetComponent<SpriteRenderer>().enabled = false;
            LeftOuterCircle.GetComponent<SpriteRenderer>().enabled = false;
            RightOuterCircle.GetComponent<SpriteRenderer>().enabled = false;
        }
        #region PlayerMovement
        if (HorizontalInput != 0.0f || VerticalInput != 0.0f)
        {
            player.transform.position += new Vector3(HorizontalInput, VerticalInput).normalized * playerSpeed * Time.deltaTime;
        }
        else
        {
            //AI移动逻辑
            if (state == STATES.IDLE)
            {
                IdleTimer += Time.deltaTime;
                if (IdleTimer >= IdleTime + timerOffset)
                {
                    IdleTimer -= IdleTime;
                    state = STATES.WALKING;
                    destination = new Vector2(Random.Range(-6, 6), Random.Range(-6, 6));
                    dir = (destination - transform.position).normalized;
                    timerOffset = Random.Range(-0.5f, 0.5f);
                }
            }
            if (state == STATES.WALKING)
            {
                WonderTimer += Time.deltaTime;
                if (WonderTimer >= WonderTime + timerOffset)
                {
                    WonderTimer -= WonderTime;
                    state = STATES.IDLE;
                    timerOffset = Random.Range(-0.5f, 0.5f);
                }
                player.transform.position += dir * wonderSpeed * Time.deltaTime;
            }

        }
        #endregion
    }
}
