using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    //Movement
    public float moveSpeed;
    public float jumpForce;

    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Vector2 moveDirection = Vector2.zero;
    private bool isGrounded = true;


    //RayCasting
    private float xRayDistance; 
    public LayerMask WallLayer;
    public LayerMask GroundLayer;
    public float lastGroundY;
    Vector3 leftOffset;
    Vector3 rightOffset;
    

    //CAMERA
    public bool updateCamera;
    public bool lookingRight;
    public float maxLookAhead;
    public float lookSpeed;
    public float lookTurnDampening;
    public Vector2 playerLookAhead;

    public float lookDown; //FOR TESTING


    //Slash
    public GameObject slashPrefab;
    public float slashDuration = 0.3f;
    float cooldownSlash;
    public float coolDown;
    Vector3 slashPosition;
    public float slashOffX;
    public float slashOffY;

    //Health
    public float currHP;
    public float maxHP;
    public bool Invincible;
    public bool cantMove;


    //General Starting an Upkeep
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        slashPosition = new Vector3(1, 0, 0);
        playerLookAhead = Vector2.zero;
        xRayDistance = (transform.localScale.x / 2) + 0.05f;

        leftOffset = Vector3.zero;
        rightOffset = Vector3.zero;
        leftOffset.x = -(transform.localScale.x / 2);
        leftOffset.y = -(transform.localScale.y / 2);
        rightOffset.x = transform.localScale.x / 2;
        rightOffset.y = -(transform.localScale.y / 2);
    }

    private void FixedUpdate()
    {
        CheckForGround();
        CheckForWalls();

        if (cooldownSlash < 0.5f)
        {
            cooldownSlash += 0.02f; // fixed update is every 1/50th of a second
        }

        if (!cantMove)
        {
            Vector2 moveVector = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
            rb.velocity = moveVector;
        }

        
       


        if (moveDirection.x > 0)
        {
            sp.flipX = false;
            lookingRight = true;
        } else if (moveDirection.x < 0)
        {
            sp.flipX = true;
            lookingRight = false;
        }

        if (lookingRight)
        {
            slashPosition = new Vector3(slashOffX, 0, 0);
        } else
        {
            slashPosition = new Vector3(-slashOffX, 0, 0);

        }


        if (lookingRight)
        {
            if (Mathf.Abs(playerLookAhead.x) < maxLookAhead) { playerLookAhead.x += lookSpeed; } // for camera movement
            if (playerLookAhead.x < 0) { playerLookAhead.x *= lookTurnDampening; }
        }
        else if (!lookingRight)
        {
            if (Mathf.Abs(playerLookAhead.x) < maxLookAhead) { playerLookAhead.x -= lookSpeed; }
            if (playerLookAhead.x > 0) { playerLookAhead.x *= lookTurnDampening; }
        }



        

        //FOR TESTING
        if (moveDirection.y < 0)
        {
            playerLookAhead.y = -lookDown;
        }
        else
        {
            playerLookAhead.y = 0f;
        }
    }


    //Combat
    private void SpawnSlash()
    {

        
        if (moveDirection.y > 0) { 
            slashPosition = new Vector3(0, slashOffY, 0); 
        } else if (moveDirection.y < 0)
        {
            slashPosition = new Vector3(0, -slashOffY, 0);

        }

        GameObject slash = Instantiate(slashPrefab, transform.position + slashPosition, Quaternion.identity, transform);
        slash.GetComponent<SlashScript>().slashPosition = slashPosition;
        cooldownSlash = 0;
    }

    public void takeDamage(GameObject other)
    {
        currHP--;
        if (currHP <= 0)
        {
            Debug.Log("DEAD");
        }


        Invincible = true;
        Invoke("endInvincible", 2f);
        cantMove = true;
        Invoke("endCantMove", 0.5f);
        sp.color = Color.magenta;


        rb.velocity = Vector2.zero;  //COULD MAKE THIS A HIT STOP BY HAVING THE FORCES BE A DELAYED METHOD CALL AND THE ZERO LAST LONGER? (would also have to stop gravity?)


        float hitLaunch = transform.position.x - other.transform.position.x;
        if (hitLaunch > 0)
        {
            rb.AddForce(new Vector2(5, 10), ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(new Vector2(-5, 10), ForceMode2D.Impulse);
        }
    }

    public void slashKnockback(float hitLaunch)
    {
        cantMove = true;
        Invoke("endCantMove", 0.08f);

        rb.velocity = Vector2.zero;

        if (hitLaunch > 0)
        {
            rb.AddForce(new Vector2(-1, 0), ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(new Vector2(1, 0), ForceMode2D.Impulse);
        }


    }

    private void endInvincible()
    {
        Invincible = false;
        sp.color = Color.white; //just a thing for now to see when invincible

    }
    private void endCantMove()
    {
        cantMove = false;
    }


    //RAYCASTING STUFF

    private void CheckForGround()
    {
        RaycastHit2D rightSide = Physics2D.Raycast(transform.position + rightOffset, Vector2.down, .02f, GroundLayer);
        RaycastHit2D leftSide = Physics2D.Raycast(transform.position + leftOffset, Vector2.down, .02f, GroundLayer);

        if (rightSide.collider != null && rb.velocity.y == 0)
        {
            isGrounded = true;
            lastGroundY = transform.position.y;
        }
        else if(leftSide.collider != null && rb.velocity.y == 0)
        {
            isGrounded = true;
            lastGroundY = transform.position.y;
        }
        else
        {
            isGrounded = false;
        }
    }
    private void CheckForWalls()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, xRayDistance, WallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, xRayDistance, WallLayer);

        if (hitLeft.collider != null)
        {
            //Debug.Log("leftWall");
        }

        if (hitRight.collider != null)
        {
            //Debug.Log("rightWall");
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
        }
    }
    public void OnSlash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if(cooldownSlash > coolDown)
            {
                SpawnSlash();
            }
        }
        else if (context.canceled)
        {

        }
    }


}
