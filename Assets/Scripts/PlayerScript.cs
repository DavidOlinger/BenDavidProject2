using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    //use #region and #endregion to enclose things
    //VARIABLES
    #region

    //Movement
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    
    public bool momentLock; 
    public bool cantMove;

    public float moveSpeed;
    public float jumpForce;

    public float wallJumpXForce;
    public float wallJumpYForce;

    private Vector2 lastStoredVelocity;

    
    public Vector2 moveDirection = Vector2.zero;

    private bool isGrounded = true;
    private bool isLeftWallTouching;
    private bool isRightWallTouching;
    private bool firstTouchWall;

    float groundingTimer;
    public float groundingCountDown;

    //Animation
    Animator animator;
    public bool isAttacking;
    public bool isVaulting;
    public bool flipLock;

    //RayCasting
    private float xRayDistance; 
    public LayerMask WallLayer;
    public LayerMask GroundLayer;
    public float lastGroundY;
    public Vector3 lastGroundPoint;
    Vector3 leftOffset;
    Vector3 rightOffset;
    public float groundCheckDist;
    private float lastTimeGrounded;
    public float coyoteTime;
    
    //Vault Variables
    public float vaultRise;
    public float vaultDistance;
    public float vaultTime;
    public float vaultCooldown;
    public float maxVaultCooldown;


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
    public GameObject heavySlashPrefab;

    public float slashDuration = 0.3f;
    public float maxSlashCoolDown;

    public float heavySlashCharge;
    public float maxHeavySlashCharge;
    public bool isChargingSlash;
    public bool heavySlashCharged;
    public bool canChargeSlash;
    public bool isHeavySlashing;


    Vector3 slashPosition;
    public float slashOffX;
    public float slashOffY;

    public float knockbackOnHit;
    public float stunOnHit; // how much hitstun an enemy gets from an attack (not super fancy rn but can be improved)
    private float slashStun = 0.1f;
    //made it private cuz we will only change it inside here for different attacks

    //Health
    public float currHP;
    public float maxHP;
    public bool invincible;

    //Other
    public float hitLaunch = 0; // this is so we don't have to pass it through parameters
    public float grav;    // = 2.65f; // this is so we can just change this one variable once if we wanna update all the gravity scale stuff
    private Coroutine CantMoveCoroutine;
    private Coroutine PlayerAttackCoroutine;
    private Coroutine InvincibleCoroutine;
    private bool hitStopActive = false;


    #endregion

    //General Starting and Update
    #region
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        slashPosition = new Vector3(1, 0, 0);
        playerLookAhead = Vector2.zero;
        xRayDistance = (transform.localScale.x / 2) + 0.05f;

        leftOffset = Vector3.zero;
        rightOffset = Vector3.zero;
        lastStoredVelocity = Vector2.zero;
        leftOffset.x = -(transform.localScale.x / 2);
        leftOffset.y = -(transform.localScale.y / 2);
        rightOffset.x = transform.localScale.x / 2;
        rightOffset.y = -(transform.localScale.y / 2);
    }

    private void FixedUpdate()
    {
        CheckForGround();
        CheckForTouchingWalls(); // commented cuz no wall jump for rn

        if (isGrounded)
        {
            lastTimeGrounded = Time.time;
            animator.SetBool("injured", false);
        }


        if (!cantMove) // removed the || !momentLock
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
        
        #region

        if (moveDirection.x > 0 && !flipLock && !animator.GetBool("injured"))
        {
            sp.flipX = false;
            lookingRight = true;
        }
        else if (moveDirection.x < 0 && !flipLock && !animator.GetBool("injured"))
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
        }
        else
        {
            groundingTimer = 0;
            Invoke("SetGroundPoint", 0.5f);
        }

        if (vaultCooldown > 0) // add audio cue
        {
            vaultCooldown -= 0.02f;
        }

        if (isChargingSlash)
        {
            heavySlashCharge += 0.02f;
            if (heavySlashCharge > maxHeavySlashCharge)
            {
                animator.SetBool("heavySlashCharged", true);
                heavySlashCharged = true; //remove
            }
        }
        else
        {
            heavySlashCharge = 0;
            animator.SetBool("heavySlashCharged", false);
            heavySlashCharged = false; //remove

        }

        if (isChargingSlash) 
        {
            cantMove = true;
        }


        //FOR TESTING
        if (moveDirection.y < 0)
        {
           // playerLookAhead.y = -lookDown;

            //need to be holding down for like a second, use a time counter for this
        } // make this only work when isGrounded and direction is held for about a second
        else
        {
           // playerLookAhead.y = 0f;
        }

        #endregion

    }


    #endregion

    //SPAWNING
    #region
    private void SpawnSlash()
    {

        //slashStun = 0.1f;


        if (sp.enabled)
        {
            GameObject slash = Instantiate(slashPrefab, transform.position, Quaternion.identity, transform);
            if (sp.flipX)
            {
                slash.transform.Rotate(0, 180, 0); ;
            }
            slash.GetComponent<SlashScript>().slashPosition = slashPosition;
        }
        
    }
    private void SpawnHeavySlash()
    {
        //slashStun = 0.25f;


        if (sp.enabled)
        {
            GameObject hslash = Instantiate(heavySlashPrefab, transform.position, Quaternion.identity, transform);
            if (sp.flipX)
            {
                hslash.transform.Rotate(0, 180, 0); ;
            }
            hslash.GetComponent<HeavySlashScript>().slashPosition = slashPosition;
        }

    }
    private void SpawnVault()
    {
       // slashStun = 0.15f;


        if (sp.enabled)
        {
            GameObject vault = Instantiate(vaultPrefab, transform.position, Quaternion.identity, transform);
            if (sp.flipX)
            {
                vault.transform.Rotate(0, 180, 0); ;
            }
            vault.GetComponent<VaultScript>().vaultPosition = slashPosition;
        }
    }

    #endregion


    //COMBAT PHYSICS AND DAMAGE
    #region
    public void VaultLaunch()
    {
        animator.SetBool("ascending", true);

        CheckForNearWallsVault();
        

        rb.velocity = Vector2.zero;


        if (CantMoveCoroutine != null)
        {
            StopCoroutine(CantMoveCoroutine);
        }
        // Start a new coroutine to handle the timing
        CantMoveCoroutine = StartCoroutine(endCantMove(vaultTime));


        if (sp.flipX)
        {
            if (isLeftWallTouching || isNearWallVault)
            {
                rb.velocity = new Vector2(vaultDistance, vaultRise);
                sp.flipX = false;
            }
            else
            {
                rb.velocity = new Vector2(-vaultDistance, vaultRise);
            }
        }
        else
        {
            if (isRightWallTouching || isNearWallVault) // trying to make it always flip the launch
            {
                rb.velocity = new Vector2(-vaultDistance, vaultRise);
                sp.flipX = true;
            }
            else
            {
                rb.velocity = new Vector2(vaultDistance, vaultRise);
            }
        }
        momentLock = true;
        StartCoroutine(ApplyVaultVel(rb.velocity, 0.17f)); //testing
    }

    
    public void takeDamage(GameObject other, float duration)
    {
        currHP--;
        if (isChargingSlash)
        {
            StopChargingSlash();
        }
        if (currHP <= 0)
        {
            Debug.Log("DEAD");
        }

        

        if (CantMoveCoroutine != null)
        {
            StopCoroutine(CantMoveCoroutine);
        }
        CantMoveCoroutine = StartCoroutine(endCantMove(duration + 0.5f));


        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        hitStopActive = true;

        if (InvincibleCoroutine != null)
        {
            StopCoroutine(InvincibleCoroutine);
        }
        InvincibleCoroutine = StartCoroutine(endInvincible(duration + 1f));

        momentLock = true;

        sp.color = new Color(1, 0.7f, 0.7f, 0.4f);
        hitLaunch = transform.position.x - other.transform.position.x;


        Invoke("dmgLaunch", duration); // until this invoke resolves in "duration" time, the player is froze in place and should enter dmg animation
    }

    private void dmgLaunch()
    {
        animator.SetBool("ascending", true);
        animator.SetBool("injured", true);
        rb.gravityScale = grav;
        hitStopActive = false;
        if (hitLaunch > 0)
        {
            rb.AddForce(new Vector2(5, 10), ForceMode2D.Impulse); // prob want to replace with velocity!!!
        }
        else
        {
            rb.AddForce(new Vector2(-5, 10), ForceMode2D.Impulse);
        }
    }



    public void hitStop()
    {

        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        hitStopActive = true;


        if (CantMoveCoroutine != null)
        {
            StopCoroutine(CantMoveCoroutine);
        }
        CantMoveCoroutine = StartCoroutine(endCantMove(0.0f + slashStun)); // make 0.0 greater than 0 to have slashKnovkback actually activate


        Invoke("slashKnockback", slashStun);
    } // activates when you hit an enemy
    private void slashKnockback()
    {

        rb.gravityScale = grav;
        hitStopActive = false;

        if (hitLaunch > 0)
        {
            rb.velocity = new Vector2(-7, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(7, rb.velocity.y);
        }


    }

    #endregion

    //RAYCASTING
    #region


    private void CheckForGround()
    {
        RaycastHit2D rightSide = Physics2D.Raycast(transform.position + rightOffset, Vector2.down, groundCheckDist, GroundLayer);
        RaycastHit2D leftSide = Physics2D.Raycast(transform.position + leftOffset, Vector2.down, groundCheckDist, GroundLayer);
        RaycastHit2D middle = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDist + 0.5f, GroundLayer);


        if (rightSide.collider != null && rb.velocity.y == 0)
        {
            animator.SetBool("grounded", true);
            animator.SetBool("ascending", false);
            animator.SetBool("descending", false);

            isGrounded = true;
            momentLock = false;
        }
        else if (leftSide.collider != null && rb.velocity.y == 0)
        {
            animator.SetBool("grounded", true);
            animator.SetBool("ascending", false);
            animator.SetBool("descending", false);

            isGrounded = true;
            momentLock = false;
        }
        else if (middle.collider != null && rb.velocity.y == 0)  // this doesn't work for some reason
        {
            animator.SetBool("grounded", true);
            animator.SetBool("ascending", false);
            animator.SetBool("descending", false);

            Debug.Log("AHHH");
            isGrounded = true;
            momentLock = false;
        }
        else
        {
            animator.SetBool("grounded", false);
            isGrounded = false;
            if (rb.velocity.y < 0)
            {
                animator.SetBool("descending", true);

            }
        }
    }

    private void CheckForTouchingWalls()
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
            if (!hitStopActive)
            {
                rb.gravityScale = grav;
            }
            isLeftWallTouching = false;
            isRightWallTouching = false;
            firstTouchWall = true;
        }
    }


    public float wallVaultRaycastDistance;
    public bool isNearWallVault = false;
    private void CheckForNearWallsVault()
    {
        Vector3 yOffset = new Vector3(0, 0.15f, 0);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position + yOffset, Vector2.left, wallVaultRaycastDistance, WallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + yOffset, Vector2.right, wallVaultRaycastDistance, WallLayer);

        if (hitLeft.collider != null && !isGrounded)
        {
            isNearWallVault = true;
        }
        else if (hitRight.collider != null && !sp.flipX && !isGrounded)
        {
            isNearWallVault = true;        }
        else
        {
            isNearWallVault = false;
        }

    }


    #endregion

    //INPUTS
    #region
    //INPUTS

    private float timeSinceStartMoving = 0;
    public void OnMove(InputAction.CallbackContext context) // Might want to remove the Vector2.zero part from this and put it somewhere else, could cause problems maybe
    {
        if (context.performed)
        {
            if(timeSinceStartMoving < 0.1f)
            {
                moveSpeed = 6.5f - ((1 - timeSinceStartMoving * 10));
                timeSinceStartMoving += 0.02f;
            }
            else
            {
                moveSpeed = 6.5f;
            }

            moveDirection = context.ReadValue<Vector2>();

        }
        else if (context.canceled)
        {

            moveDirection = Vector2.zero;
            timeSinceStartMoving = 0;
        }
    }


    private bool wasGroundedCoyote()
    {
        return Time.time - lastTimeGrounded <= coyoteTime;
    }

    private bool wallJumping = false;
    private bool isJumping = false;
    private bool shortHop = false;
    private void jumpLaunch() // just separated to make timing easier by calling this with invoke for jumpsquat timing
    {
        
       
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isGrounded = false;
        isJumping = false;
        animator.SetBool("ascending", true);

        if (shortHop)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            shortHop = false;
        }

    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && (isGrounded  || (wasGroundedCoyote() && rb.velocity.y <= 0)) && !cantMove && !isJumping)
        {
            isJumping = true;
            //put the jump squat animation here !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Invoke("jumpLaunch", 0.06f);

        }
        //else if(context.started && !isGrounded && isLeftWallTouching && !cantMove)
        //{
        //    if (CantMoveCoroutine != null)
        //    {
        //        StopCoroutine(CantMoveCoroutine);
        //    }
        //    wallJumping = true;
        //    CantMoveCoroutine = StartCoroutine(endCantMove(0.14f));

        //    rb.velocity = Vector2.zero;
        //    rb.velocity = new Vector2(wallJumpXForce, wallJumpYForce);
        //    animator.SetBool("ascending", true);

        //}
        //else if (context.started && !isGrounded && isRightWallTouching && !cantMove)
        //{
        //    if (CantMoveCoroutine != null)
        //    {
        //        StopCoroutine(CantMoveCoroutine);
        //    }
        //    wallJumping = true;
        //    CantMoveCoroutine = StartCoroutine(endCantMove(0.14f));

        //    rb.velocity = Vector2.zero;
        //    rb.velocity = new Vector2(-wallJumpXForce, wallJumpYForce);
        //    animator.SetBool("ascending", true);

        //}
        else if (context.canceled)
        {
            if(rb.velocity.y > 0)
            {
                if (!cantMove || wallJumping)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.55f);
                }
            }
            else if (isJumping)
            {
                shortHop = true;
            }
        }
    }


    public void OnSlash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!isAttacking)
            {
                StartAttack();
                if (isGrounded)
                {
                   // canChargeSlash = true;
                }
                
            }
        }
        else if (context.canceled)
        {
           // canChargeSlash = false;
            if (heavySlashCharged)
            {
               // StartHeavySlash();
            }

           // StopChargingSlash();
            
        }
    }


    public void OnVault(InputAction.CallbackContext context)
    {
        
        if (context.started)
        {
            if (!isVaulting && vaultCooldown <= 0)
            {
                StartVault();
                vaultCooldown = maxVaultCooldown;
            }
        }

    }


    #endregion

    // RESPAWNING
    #region
    void SetGroundPoint()
    {
        if (isGrounded && !invincible) {lastGroundPoint = transform.position + new Vector3(0, 0.2f, 0); }
        
    }

    public void HitKillzone()
    {
        cam.GetComponent<CameraMovementScript>().fadeToBlack();


        if (CantMoveCoroutine != null)
        {
            StopCoroutine(CantMoveCoroutine);
        }
        CantMoveCoroutine = StartCoroutine(endCantMove(1.5f));
        vaultCooldown = 1.5f;

        currHP--;
        sp.enabled = false;
        rb.velocity = Vector2.zero;
        Invoke("ResetPlayerPos", 0.1f);
        sp.color = new Color(1, 0.65f, 0.65f, 0.7f);

        if (InvincibleCoroutine != null)
        {
            StopCoroutine(InvincibleCoroutine);
        }
        InvincibleCoroutine = StartCoroutine(endInvincible(2f));


    }

    void ResetPlayerPos()
    {
        cantMove = true;
        transform.position = lastGroundPoint;
        Invoke("enableSprite", 0.5f);
    }

    #endregion


    //COROUTINES
    #region
    private IEnumerator endInvincible(float duration)
    {
        invincible = true;

        yield return new WaitForSeconds(duration);


        invincible = false;
        sp.color = Color.white;
    }

    private IEnumerator endCantMove(float duration)
    {
        cantMove = true;

        //Debug.Log("Movement Disabled");

        yield return new WaitForSeconds(duration);

        cantMove = false;
        wallJumping = false; // this is my temp fix for wallJunmping

        //Debug.Log("Movement Enabled");
    }

    private IEnumerator ApplyVaultVel(Vector2 vel, float duration)
    {
        for (int i = 0; i < duration * 50; i++)
        {
            yield return new WaitForSeconds(0.02f);
            rb.velocity = new Vector2(vel.x, rb.velocity.y);
        }
    }

    #endregion


    //UTILITIES
    #region
    void enableSprite()
    {
        sp.enabled = true;
    }

    public void StartAttack()
    {
        if (!isAttacking)
        {
            Debug.Log("Attack Started");
            isAttacking = true;
            animator.SetBool("attacking", true);
        }
    }

    public void EndAttack()
    {
        Debug.Log("Attack Ended");
        isAttacking = false;
        animator.SetBool("attacking", false);
    }

    public void StartVault()
    {
        if (!isVaulting)
        {
            Debug.Log("Vault Started");
            isVaulting = true;
            animator.SetBool("vaulting", true);
        }
    }

    public void EndVault()
    {
        Debug.Log("Vault Ended");
        isVaulting = false;
        animator.SetBool("vaulting", false);
    }

    public void HaltVelocity()
    {
        rb.velocity = Vector2.zero;
    }

    public void SaveVelocity()
    {
        lastStoredVelocity = rb.velocity;
        HaltVelocity();
    }

    public void LoadVelocity()
    {
        rb.velocity = lastStoredVelocity;
    }

    public void AddSavedVelocity()
    {
        rb.velocity = rb.velocity + lastStoredVelocity;
    }




    public void StartChargingSlash()
    {
        if (canChargeSlash)
        {
            flipLock = true;
            cantMove = true;
            HaltVelocity();
            animator.SetBool("holdingSlash", true);
            isChargingSlash = true;
        }

    }

    public void StopChargingSlash()
    {
        flipLock = false;
        cantMove = false;
        animator.SetBool("holdingSlash", false);
        heavySlashCharge = 0f;
        heavySlashCharged = false;
        animator.SetBool("heavySlashCharged", false);
        isChargingSlash = false;
    }


    public void StartHeavySlash()
    {
        if (!isHeavySlashing)
        {
            Debug.Log("Heavy Slash Started");
            isHeavySlashing = true;
            animator.SetBool("heavySlashing", true);
        }
    }

    public void EndHeavySlash()
    {
        Debug.Log("Heavy Slash Ended");
        isHeavySlashing = false;
        animator.SetBool("heavySlashing", false);
    }


   public void InjuredStop()
    {
        animator.SetBool("injured", false );
    }

    #endregion
}
