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
    public float WonderTimeOffset = 0.5f;
    private float WonderOffset = 0.5f;
    public float IdleTime = 1.0f;
    private float IdleTimer = 0.0f;
    public float IdleTimeOffset = 0.5f;
    private float IdleOffset = 0.5f;
    private STATES state = STATES.IDLE;
    private Vector3 dir = Vector2.up;
    public float RotationDegree = 30.0f;
    public float RotationDegreeOffset = 10.0f;
    private float RotationOffset = 10.0f;
    public float wonderSpeed = 2.5f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dir = Quaternion.Euler(0, RotationDegree + RotationOffset, 0) * dir;
        RotationOffset = Random.Range(-RotationDegreeOffset, RotationDegreeOffset);
    }

    void Update()
    {
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
            transform.position += dir * wonderSpeed * Time.deltaTime;
        }
    }
}
