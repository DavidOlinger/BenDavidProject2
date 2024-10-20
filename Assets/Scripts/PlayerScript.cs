using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    //Movement
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    
    public bool momentLock; 
    public bool cantMove;

    public float moveSpeed;
    public float jumpForce;

    public float wallJumpXForce;
    public float wallJumpYForce;


    
    public Vector2 moveDirection = Vector2.zero;

    private bool isGrounded = true;
    private bool isLeftWallTouching;
    private bool isRightWallTouching;
    private bool firstTouchWall;

    float groundingTimer;
    public float groundingCountDown;

    //Animation
    Animator animator;

    //RayCasting
    private float xRayDistance; 
    public LayerMask WallLayer;
    public LayerMask GroundLayer;
    public float lastGroundY;
    public Vector3 lastGroundPoint;
    Vector3 leftOffset;
    Vector3 rightOffset;
    public float groundCheckDist;
    
    //Vault Variables
    public float vaultRise;
    public float vaultDistance;
    public float vaultTime;

    //CAMERA
    GameObject cam;
    public bool updateCamera;
    public bool lookingRight;
    public float maxLookAhead;
    public float lookSpeed;
    public float lookTurnDampening;
    public Vector2 playerLookAhead;

    public float lookDown; //FOR TESTING


    //Slash
    public GameObject slashPrefab;
    public GameObject vaultPrefab; //idk if we are doing this right but it should work for now

    public float slashDuration = 0.3f;
    float slashCooldown;
    public float maxSlashCoolDown;

    Vector3 slashPosition;
    public float slashOffX;
    public float slashOffY;

    public float knockbackOnHit;

    //Health
    public float currHP;
    public float maxHP;
    public bool invincible;
    

   


    //General Starting and Upkeep
    void Start()
    {
        momentLock = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
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

        if (slashCooldown < maxSlashCoolDown)
        {
            slashCooldown += 0.02f; // fixed update is every 1/50th of a second
        }

        if (!cantMove || !momentLock)
        {
            Vector2 moveVector = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
            if (momentLock && moveVector.x != 0)
            {
                momentLock = false;
            }

            if (!momentLock)
            {
                rb.velocity = moveVector;
            }

            
            
        }

        if (moveDirection.x > 0)
        {
            sp.flipX = false;
            lookingRight = true;
        }
        else if (moveDirection.x < 0)
        {
            sp.flipX = true;
            lookingRight = false;
        }
        if (lookingRight)
        {
            slashPosition = new Vector3(slashOffX, 0, 0);
        }
        else
        {
            slashPosition = new Vector3(-slashOffX, 0, 0);

        }

        animator.SetFloat("speed", Mathf.Abs(moveDirection.x));
        if (animator.GetBool("ascending") && rb.velocity.y < 0)
        {
            animator.SetBool("ascending", false);
            animator.SetBool("descending", true);
        }
        if (animator.GetBool("grounded"))
        {
            animator.SetBool("ascending", false);
            animator.SetBool("descending", false);
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


        
        if (groundingTimer < groundingCountDown)
        {
            groundingTimer += 0.02f;
        }else
        {
            groundingTimer = 0;
            Invoke("SetGroundPoint", 0.5f);
        }

        //FOR TESTING
        if (moveDirection.y < 0)
        {
           // playerLookAhead.y = -lookDown;

            //need to be holding down for like a second, use a time counter for this
        }
        else
        {
           // playerLookAhead.y = 0f;
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
        slashCooldown = 0;
    }
    private void SpawnVault()
    {

        if (sp.flipX)
        {
            slashPosition = new Vector3(-0.6f, -0.7f, 0);
            GameObject slash = Instantiate(vaultPrefab, transform.position + slashPosition, Quaternion.identity, transform);
        }
        else
        { 
            slashPosition = new Vector3(0.6f, -0.7f, 0);
            GameObject slash = Instantiate(vaultPrefab, transform.position + slashPosition, Quaternion.identity, transform);
        }
    }
    public void VaultLaunch()
    {
        animator.SetBool("ascending", true);
        cantMove = true;
        momentLock = true;

        rb.velocity = Vector2.zero;

        Invoke("endCantMove", vaultTime);


        if (sp.flipX)
        {
            if (isLeftWallTouching)
            {
                rb.velocity = new Vector2(vaultDistance, vaultRise);
            }
            else
            {
                rb.velocity = new Vector2(-vaultDistance, vaultRise);
            }
        }
        else
        {
            if (isRightWallTouching)
            {
                rb.velocity = new Vector2(-vaultDistance, vaultRise);
            }
            else
            {
                rb.velocity = new Vector2(vaultDistance, vaultRise);
            }
        }
    }

    public void takeDamage(GameObject other)
    {
        currHP--;
        if (currHP <= 0)
        {
            Debug.Log("DEAD");
        }


        invincible = true;
        Invoke("endInvincible", 2f);
        cantMove = true;
        momentLock = true;
        Invoke("endCantMove", 0.5f);
        sp.color = new Color(1, 0.7f, 0.7f, 0.4f);


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
        momentLock = true;
        Invoke("endCantMove", 0.14f);

        rb.velocity = new Vector2(0, rb.velocity.y);

        if (hitLaunch > 0)
        {
            rb.AddForce(new Vector2(-1.8f, 0), ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(new Vector2(1.8f, 0), ForceMode2D.Impulse);
        }


    } //currently is never used

    private void endInvincible()
    {
        invincible = false;
        sp.color = Color.white; //just a thing for now to see when invincible

    }
    private void endCantMove()
    {
        cantMove = false;
    }


    //RAYCASTING STUFF

    private void CheckForGround()
    {
        RaycastHit2D rightSide = Physics2D.Raycast(transform.position + rightOffset, Vector2.down, groundCheckDist, GroundLayer);
        RaycastHit2D leftSide = Physics2D.Raycast(transform.position + leftOffset, Vector2.down, groundCheckDist, GroundLayer);

        if (rightSide.collider != null && rb.velocity.y == 0)
        {
            animator.SetBool("grounded", true);
            isGrounded = true;
            momentLock = false;
        }
        else if(leftSide.collider != null && rb.velocity.y == 0)
        {
            animator.SetBool("grounded", true);

            isGrounded = true;
            momentLock = false;
        }
        else
        {
            animator.SetBool("grounded", false);
            isGrounded = false;
        }
    }
    private void CheckForWalls()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, xRayDistance, WallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, xRayDistance, WallLayer);

        if (hitLeft.collider != null && moveDirection.x < 0 && !isGrounded && rb.velocity.y < 0)
        {
            if (firstTouchWall)
            {
                momentLock = false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                firstTouchWall = false;
            }
            rb.gravityScale = 1;
            isLeftWallTouching = true;
            isRightWallTouching = false;
        }
        else if (hitRight.collider != null && moveDirection.x > 0 && !isGrounded && rb.velocity.y < 0)
        {
            if (firstTouchWall)
            {   momentLock = false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                firstTouchWall = false;
            }
            rb.gravityScale = 1; // need to also adjust linear drag to some value that will cause no acceleration
            isRightWallTouching = true;
            isLeftWallTouching = false;
        }
        else
        {
            rb.gravityScale = 2.65f;
            isLeftWallTouching = false;
            isRightWallTouching = false;
            firstTouchWall = true;
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
            animator.SetBool("ascending", true);

        }
        else if(context.started && !isGrounded && isLeftWallTouching)
        {
            cantMove = true;
            momentLock = true;
            Invoke("endCantMove", 0.16f);
            rb.velocity = Vector2.zero;
            rb.velocity = new Vector2(wallJumpXForce, wallJumpYForce);
            animator.SetBool("ascending", true);

        }
        else if (context.started && !isGrounded && isRightWallTouching)
        {
            cantMove = true;
            momentLock = true;
            Invoke("endCantMove", 0.16f);
            rb.velocity = Vector2.zero;
            rb.velocity = new Vector2(-wallJumpXForce, wallJumpYForce);
            animator.SetBool("ascending", true);

        }
        else if (context.canceled && rb.velocity.y > 0 && !cantMove) // added the cantMove part to stop weird sudden velocity stopping
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }
    public void OnSlash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if(slashCooldown >= maxSlashCoolDown)
            {
                SpawnSlash();
            }
        }
        else if (context.canceled)
        {

        }
    }
    public void OnVault(InputAction.CallbackContext context)
    {
        if (context.started && !cantMove)
        {
            SpawnVault(); // need to replace this with something with its own script to check for a collision
        }

    }

    void SetGroundPoint()
    {
        if (isGrounded && !invincible) {lastGroundPoint = transform.position + new Vector3(0, 0.2f, 0); }
        
    }

    public void HitKillzone()
    {
        currHP--;
        sp.enabled = false;
        rb.velocity = Vector2.zero;
        Invoke("ResetPlayerPos", 0.1f);
        invincible = true;
        sp.color = new Color(1, 0.7f, 0.7f, 0.4f);
        Invoke("endInvincible", 1.5f);
        cam.GetComponent<CameraMovementScript>().fadeToBlack();

    }

    void ResetPlayerPos()
    {
        cantMove = true;
        transform.position = lastGroundPoint;
        Invoke("enableSprite", 0.5f);
        Invoke("endCantMove", 2f);
    }

    void enableSprite()
    {
        sp.enabled = true;
    }
}
