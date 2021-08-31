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
    
    //Animations
    //----------------------------
    private Animator anim;
    private STATES state = STATES.IDLE;
    private Vector2 dir = Vector2.up;
    private Color lockedColor = Color.red;
    private Color normalColor = new Color(0.3410021f, 0.60746f, 0.745283f);
    [HideInInspector] public bool isLocked = false;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public bool isDead = false;
    //----------------------------

    //Movement Parameters
    //----------------------------
    private Rigidbody2D rb;
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
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        dir = Quaternion.Euler(0, RotationDegree + RotationOffset, 0) * dir;
        RotationOffset = Random.Range(-RotationDegreeOffset, RotationDegreeOffset);
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            rb.velocity = Vector2.zero;
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
            rb.velocity = dir * wonderSpeed;
            //rb.position += dir * wonderSpeed * Time.deltaTime;
            transform.rotation = dir.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        }
    }

    public void Die()
    {
        spriteRenderer.color = normalColor;
        isDead = true;
        anim.SetBool("IsDead", true);
        Destroy(this, 0.5f);
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<Animator>(), 2.0f);
    }
}
