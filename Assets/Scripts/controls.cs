using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class controls : MonoBehaviour
{

    public float moveSpeed = 0;

    Rigidbody2D rb;

    Vector2 moveDirection = Vector2.zero;

    public float jumpForce;

    public bool canJump;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }


   
            //rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    


    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
    }


    void OnMOVE(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
    }

  
    void OnJUMP()
    {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }


    void OnSLASH()
    {
        Debug.Log("slash");
    }



}
