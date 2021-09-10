using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMovement : MonoBehaviour
{
    private Vector2 StartPoint;
    private float HorizontalInput;
    private float VerticalInput;
    public GameObject InnerCircle;
    public GameObject LeftOuterCircle;
    public GameObject RightOuterCircle;
    public GameObject player;
    public float playerSpeed = 5.0f;
    private void Start()
    {
        StartPoint = transform.position;
    }
    void Update()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        #region Simulate Joystick
        if (HorizontalInput != 0.0f || VerticalInput != 0.0f)
        {
            InnerCircle.transform.position = StartPoint;
            InnerCircle.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            InnerCircle.GetComponent<SpriteRenderer>().enabled = false;
        }
        InnerCircle.transform.position = new Vector2(transform.position.x + HorizontalInput, InnerCircle.transform.position.y);
        InnerCircle.transform.position = new Vector2(InnerCircle.transform.position.x, transform.position.y + VerticalInput);
        #endregion
        #region PlayerMovement
        player.transform.position += new Vector3(HorizontalInput, VerticalInput).normalized * playerSpeed * Time.deltaTime;
        #endregion
    }
}
