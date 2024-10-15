using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class controls : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;

    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Vector2 moveDirection = Vector2.zero;
    private bool isGrounded = true;

    public float rayDistance = 0.1f; 
    public LayerMask GroundLayer;

    public bool updateCamera;


    public GameObject slashPrefab;
    public float slashDuration = 0.3f;
    public float cooldownSlash;

    Vector3 slashPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        slashPosition = new Vector3(1, 0, 0);
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
       
    }


    public void Update()
    {
        CheckForGround();
        CheckForWalls();

        if(cooldownSlash < 0.5f)
        {
            cooldownSlash += Time.deltaTime;
        }

        if(moveDirection.x > 0)
        {
            sp.flipX = false;
            slashPosition.x = 1;

        }
        else if(moveDirection.x < 0)
        {
            sp.flipX = true;
            slashPosition.x = -1;
        }
    }



  

    private void SpawnSlash()
    {

        GameObject slash = Instantiate(slashPrefab, transform.position + slashPosition, Quaternion.identity, transform);
        cooldownSlash = 0;
        Destroy(slash, slashDuration);
    }



    //RAYCASTING STUFF

    private void CheckForGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, GroundLayer);
        if (hit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void CheckForWalls()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, rayDistance, GroundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, rayDistance, GroundLayer);

        if (hitLeft.collider != null)
        {
            Debug.Log("leftWall");
        }

        if (hitRight.collider != null)
        {
            Debug.Log("rightWall");
        }
    }





    //INPUT MANAGING

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            moveDirection = Vector2.zero;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }
        else if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            Debug.Log("Jump Released");
        }
    }

    public void OnSlash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Slash Started");
            if(cooldownSlash > 0.48f)
            {
                SpawnSlash();
            }
        }
        else if (context.canceled)
        {
            Debug.Log("Slash Released");
        }
    }


}
