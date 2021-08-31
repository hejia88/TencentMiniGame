using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
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
    private Rigidbody2D rb;
    public float WonderTime = 2.0f;
    private float WonderTimer = 0.0f;
    public float IdleTime = 1.0f;
    private float IdleTimer = 0.0f;
    private float timerOffset = 0.0f;
    private STATES state = STATES.IDLE;
    private Vector3 destination = Vector3.zero;
    private Vector3 dir = Vector2.zero;
    public float wonderSpeed = 2.5f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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
            transform.position += dir * wonderSpeed * Time.deltaTime;
        }
    }
}
