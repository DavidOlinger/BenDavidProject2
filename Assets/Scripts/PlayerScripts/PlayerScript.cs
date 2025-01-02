using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{


    //VARIABLES
    #region
    //Movement
    [Header("Movement")]
    [SerializeField] public bool momentLock;
    [SerializeField] public float moveSpeed = 5.85f;
    [SerializeField] public float normalMoveSpeed = 5.85f;
    [SerializeField] public float jumpForce;
    [SerializeField] public float wallJumpXForce;
    [SerializeField] public float wallJumpYForce;
    [SerializeField] public float groundingCountDown;

    private Vector2 lastStoredVelocity;
    private Vector2 moveDirection = Vector2.zero;
    private Vector2 moveVector;
    private bool isGrounded = true;
    private bool isLeftWallTouching;
    private bool isRightWallTouching;
    private bool firstTouchWall;
    private float groundingTimer;
    private SpriteRenderer sp;

    public Rigidbody2D rb;
    public bool cantMove;



    //Animation
    private Animator animator;
    [SerializeField] public bool isAttacking;
    [SerializeField] public bool isVaulting;
    [SerializeField] public bool flipLock;

    //RayCasting
    private float xRayDistance;
    [SerializeField] public LayerMask WallLayer;
    [SerializeField] public LayerMask GroundLayer;
    [SerializeField] public float lastGroundY;
    [SerializeField] public Vector3 lastGroundPoint;
    Vector3 leftOffset;
    Vector3 rightOffset;
    Vector3 middleOffset;
    [SerializeField] public float groundCheckDist;
    private float lastTimeGrounded;
    [SerializeField] public float coyoteTime;

    //Vault Variables
    [SerializeField] public float vaultRise;
    [SerializeField] public float vaultSpeed;
    [SerializeField] public float vaultTime;
    [SerializeField] public float vaultCooldown;
    [SerializeField] public float maxVaultCooldown;


    //CAMERA
    [Header("Camera")]
    [SerializeField] public GameObject cam;
    [SerializeField] public bool updateCamera;
    [SerializeField] public bool lookingRight;
    [SerializeField] public float maxLookAhead;
    [SerializeField] public float lookSpeed;
    [SerializeField] public float lookTurnDampening;
    [SerializeField] public Vector2 playerLookAhead;
    [SerializeField] public float lookDown; //FOR TESTING


    //Slash
    public GameObject downSlashPrefab;
    public GameObject upSlashPrefab;
    public GameObject vaultPrefab; //idk if we are doing this right but it should work for now
    public GameObject heavySlashPrefab;

    public float maxSlashCoolDown;
    public bool attackOnCooldown;

    public bool isHeavySlashing;
    public bool heavySlashOnCooldown;
    public float maxHeavySlashCooldown;

    public bool attackQueued;
    public bool nextSwingIsDownward = true;


    Vector3 slashPosition;
    public float slashOffX;
    public float slashOffY;

    public float knockbackOnHit;
    public float stunOnHit; // how much hitstun an enemy gets from an attack (not super fancy rn but can be improved)
    //made it private cuz we will only change it inside here for different attacks

    //Health
    public int currHP;
    public int maxHP; 
    public bool invincible;
    public float pickupRadius;

    public GameObject lastCampfire; //CAN BE NULL

    //Other
    public float hitLaunch = 0; // this is so we don't have to pass it through parameters
    public float grav;    // = 2.65f; // this is so we can just change this one variable once if we wanna update all the gravity scale stuff
    private Coroutine CantMoveCoroutine;
    private Coroutine PlayerAttackCoroutine;
    private Coroutine InvincibleCoroutine;
    //private bool hitMoveLock = false;
    public float maxGravSpeed;

    public float moveDecayCoef;

    public TimeManagerScript timeScript;
    public LogicScript logicScript;

    public ParticleSystem jumpDust;
    public ParticleSystem vaultDust;
    public ParticleSystem launchCloud;
    public ParticleSystem interactFlash;

    public bool isInteracting;

    //SOUND
    AudioSource audioSource;
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] AudioClip[] attackHitSounds;
    [SerializeField] AudioClip[] swingSounds;
    [SerializeField] AudioClip jumpLaunchSound;
    [SerializeField] AudioClip jumpLandSound;
    [SerializeField] AudioClip vaultSound;
    [SerializeField] AudioClip hpShatterSound;
    [SerializeField] AudioClip playerGotHitSound;
    [SerializeField] AudioClip saveSound;
    [SerializeField] AudioClip moneySound;






    private bool canBoomer = true;
    public GameObject boomerang;
    public float boomerangCooldown;

    public float wallVaultRaycastDistance;
    private bool RIGHTisNearWallVault = false;
    private bool LEFTisNearWallVault = false;

    [HideInInspector] public bool showMovementSettings = true;
    [HideInInspector] public bool showCameraSettings = true;
    [HideInInspector] public bool showSlashSettings = true;
    [HideInInspector] public bool showHealthSettings = true;
    [HideInInspector] public bool showSoundSettings = true;
    [HideInInspector] public bool showMiscellaneousSettings = true;
    #endregion




    //General Starting and Update
    #region
    void Start()
    {
        maxHP = PlayerPrefs.GetInt("MaxHP");
        currHP = PlayerPrefs.GetInt("CurrHP");

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        timeScript = GameObject.FindWithTag("TimeManager").GetComponent<TimeManagerScript>();
        logicScript = GameObject.FindWithTag("TimeManager").GetComponent<LogicScript>();
        audioSource = GetComponent<AudioSource>();
        slashPosition = new Vector3(1, 0, 0);
        playerLookAhead = Vector2.zero;
        xRayDistance = (transform.localScale.x / 2) + 0.05f;

        leftOffset = Vector3.zero;
        rightOffset = Vector3.zero;
        middleOffset = Vector3.zero;
        lastStoredVelocity = Vector2.zero;
        leftOffset.x = -(transform.localScale.x / 2);
        leftOffset.y = -(transform.localScale.y / 2);
        rightOffset.x = transform.localScale.x / 2;
        rightOffset.y = -(transform.localScale.y / 2);
        middleOffset.y = -(transform.localScale.y / 2);

        //vault dust particle init
        var vdmain = vaultDust.main;
        //vdmain.duration = vaultTime;   // removed for now cuz it caused an error in the console, idk why this is here
    }


    private float maxSpeedHorizontalAllowed = 5.85f;
    private void FixedUpdate()
    {
        if(PlayerPrefs.GetInt("Boon2") == 1)
        {
            moveSpeed = 6.8f;
            normalMoveSpeed = 6.8f;

            vaultSpeed = 13.5f;

        }
        

//if (Input.GetKeyDown(KeyCode.G))
//        {
//            PlayerPrefs.SetFloat("CanVault", 1);
//            PlayerPrefs.SetFloat("CanHeavySlash", 1);
//        }


        CheckForGround();
        CheckForTouchingWalls(); // commented cuz no wall jump for rn

        if (isGrounded)
        {
            lastTimeGrounded = Time.time;
            animator.SetBool("injured", false);
        }


        if (!cantMove) // removed the || !momentLock
        {
            moveVector = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);


            //experimental thing trying to make movement additive{{{

            //Vector2 moveVector = new Vector2(rb.velocity.x, rb.velocity.y);
            //if (rb.velocity.x < maxSpeedHorizontalAllowed)
            //{
            //    moveVector = new Vector2(rb.velocity.x + (moveDirection.x * moveSpeed), rb.velocity.y);

            //}
            //else
            //{
            //    rb.velocity.x = maxSpeedHorizontalAllowed;
            //}


            if (rb.velocity.x == 0 || isGrounded) //if grounded or not travelling in either direction
            {
                momentLock = false; //turn off moment lock
            }

            if (!momentLock)
            {
                rb.velocity = moveVector;
            } else
            {
                if (moveVector.x > 0 && rb.velocity.x < 0) //if holding right while moving left while momentlock is on
                {
                    
                    rb.velocity = new Vector2(rb.velocity.x + moveDecayCoef, rb.velocity.y); //decay leftward movement
                    if (rb.velocity.x > 0) //dont overshoot
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
                if (moveVector.x < 0 && rb.velocity.x > 0) //if holding leftt while moving right while momentlock is on
                {
                    rb.velocity = new Vector2(rb.velocity.x - moveDecayCoef, rb.velocity.y); //decay rightward movement
                    if (rb.velocity.x < 0) //dont overshoot
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
            }

        }

        #region

        if (moveDirection.x > 0 && !flipLock && !animator.GetBool("injured") && !cantMove)
        {
            sp.flipX = false;
            lookingRight = true;
        }
        else if (moveDirection.x < 0 && !flipLock && !animator.GetBool("injured") && !cantMove)
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

        if (!cantMove)
        {
            animator.SetFloat("speed", Mathf.Abs(moveDirection.x));

        }

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


        if (rb.velocity.y < -maxGravSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxGravSpeed);
        }



        

    }
    #endregion

    //SPAWNING
    #region
    private void SpawnDownSlash()
    {

        //slashStun = 0.1f;


        if (sp.enabled)
        {
            GameObject slash = Instantiate(downSlashPrefab, transform.position, Quaternion.identity, transform);
            if (sp.flipX)
            {
                slash.transform.Rotate(0, 180, 0); ;
            }
            slash.GetComponent<SlashScript>().slashPosition = slashPosition;
        }

    }

    private void SpawnUpSlash()
    {

        //slashStun = 0.1f;


        if (sp.enabled)
        {
            GameObject slash = Instantiate(upSlashPrefab, transform.position, Quaternion.identity, transform);
            if (sp.flipX)
            {
                slash.transform.Rotate(0, 180, 0); ;
            }
            slash.GetComponent<SlashScript>().slashPosition = slashPosition;
        }

    }
    private void SpawnHeavySlash()
    {

        if (sp.enabled)
        {
            GameObject hslash = Instantiate(heavySlashPrefab, transform.position, Quaternion.identity, transform);
            if (sp.flipX)
            {
                hslash.transform.Rotate(0, 180, 0); ;
            }
            hslash.GetComponent<SlashScript>().slashPosition = slashPosition;
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

    void CreateJumpDust()
    {
        PlayJumpSound();
        jumpDust.Play();
    }

    void CreateVaultDust()
    {
        launchCloud.Play();
        vaultDust.Play();
    }

    #endregion


    //COMBAT PHYSICS AND DAMAGE
    #region
    public void VaultLaunch()
    {
        animator.SetBool("ascending", true);

        CheckForNearWallsVault();

        flipLock = true; // to make it look cooler

        //maxSpeedHorizontalAllowed = vaultSpeed;


        rb.velocity = Vector2.zero;


        if (CantMoveCoroutine != null)
        {
            StopCoroutine(CantMoveCoroutine);
        }
        // Start a new coroutine to handle the timing
        CantMoveCoroutine = StartCoroutine(endCantMove(vaultTime));


        if (InvincibleCoroutine != null)
        {
            StopCoroutine(InvincibleCoroutine);
        }
        InvincibleCoroutine = StartCoroutine(endInvincible(0.08f));

        if (sp.flipX)
        {
            if (isLeftWallTouching || LEFTisNearWallVault)
            {
                rb.velocity = new Vector2(vaultSpeed, vaultRise);
                sp.flipX = false;
            }
            else
            {
                rb.velocity = new Vector2(-vaultSpeed, vaultRise);
            }
        }
        else
        {
            if (isRightWallTouching || RIGHTisNearWallVault) // trying to make it always flip the launch
            {
                rb.velocity = new Vector2(-vaultSpeed, vaultRise);
                sp.flipX = true;
            }
            else
            {
                rb.velocity = new Vector2(vaultSpeed, vaultRise);
            }
        }
        momentLock = true;
        StartCoroutine(ApplyVaultVel(rb.velocity, vaultTime)); //testing
        CreateVaultDust();
    }


    public void takeDamage(GameObject other, float duration, int damage, bool willLaunch)
    {
        if(PlayerPrefs.GetInt("Bless2") == 1)
        {
            if (Random.Range(0f, 1f) < 0.2f)
            {
                Debug.Log("HE WAS LUCKY!!!!");
                //PLAY A SOUND EFFECT HERE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            }
            else
            {
                if(currHP < 1.9f && Random.Range(0f, 1f) < 0.2f)
                {
                    Debug.Log("HE WAS LUCKY!!!!");

                }
                else
                {
                    currHP -= damage;
                    PlayHPLossSound();
                    logicScript.UpdateHealth();
                }
                
            }
        }
        else
        {
            currHP -= damage;
            PlayHPLossSound();
            logicScript.UpdateHealth();
        }
        
        if (currHP <= 0)
        {
            PlayerDeath(true);

        }
        cam.GetComponent<CameraMovementScript>().CameraShake(0.15f, 50f, 0.12f, 0.90f);
        hitStop(duration, 0.01f);

        //causing sliding..
        if (CantMoveCoroutine != null)
        {
            StopCoroutine(CantMoveCoroutine);
        }
        CantMoveCoroutine = StartCoroutine(endCantMove(0.5f));


        if (InvincibleCoroutine != null)
        {
            StopCoroutine(InvincibleCoroutine);
        }
        InvincibleCoroutine = StartCoroutine(endInvincible(1.5f));

        momentLock = true;

        sp.color = new Color(1, 0.7f, 0.7f, 0.4f);
        hitLaunch = transform.position.x - other.transform.position.x;

        rb.velocity = Vector2.zero;
        if (willLaunch)
        {
            dmgLaunch();
        }
        

        //Invoke("dmgLaunch", duration); // until this invoke resolves in "duration" time, the player is froze in place and should enter dmg animation
    }

    private void dmgLaunch()
    {
        animator.SetBool("ascending", true);
        animator.SetBool("injured", true);

        if (hitLaunch > 0)
        {
            rb.AddForce(new Vector2(5, 10), ForceMode2D.Impulse); // prob want to replace with velocity!!!
        }
        else
        {
            rb.AddForce(new Vector2(-5, 10), ForceMode2D.Impulse);
        }
    }



    public void hitStop(float duration, float delay)
    {
        
        timeScript.TimeStop(duration, delay);
    }
    public void slashKnockback()
    {

        //rb.gravityScale = grav;

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
        RaycastHit2D middle = Physics2D.Raycast(transform.position + middleOffset, Vector2.down, groundCheckDist, GroundLayer);


        if (rightSide.collider != null && rb.velocity.y == 0)
        {
            setGrounded();
        }
        else if (leftSide.collider != null && rb.velocity.y == 0)
        {
            setGrounded();
        }
        else if (middle.collider != null && rb.velocity.y == 0)  // this doesn't work for some reason
        {
            setGrounded();
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

    private void setGrounded()
    {
        animator.SetBool("grounded", true);
        animator.SetBool("ascending", false);
        animator.SetBool("descending", false);

        isGrounded = true;
        momentLock = false;
        wallJumping = false;
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
            animator.SetBool("touchingWall", true);
        }
        else if (hitRight.collider != null && moveDirection.x > 0 && !isGrounded && rb.velocity.y < 0)
        {
            if (firstTouchWall)
            { momentLock = false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                firstTouchWall = false;
            }
            rb.gravityScale = 1; // need to also adjust linear drag to some value that will cause no acceleration
            isRightWallTouching = true;
            isLeftWallTouching = false;
            animator.SetBool("touchingWall", true);

        }
        else
        {
            rb.gravityScale = grav;
            isLeftWallTouching = false;
            isRightWallTouching = false;
            firstTouchWall = true;
            animator.SetBool("touchingWall", false);

        }
    }


   

    private void CheckForNearWallsVault()
    {
        Vector3 yOffset = new Vector3(0, -0.1f, 0); // this is what is changing the height of where we check to bounce off walls
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position + yOffset, Vector2.left, wallVaultRaycastDistance, WallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + yOffset, Vector2.right, wallVaultRaycastDistance, WallLayer);

        if (hitLeft.collider != null && !isGrounded)
        {
            LEFTisNearWallVault = true;
        }
        else if (hitRight.collider != null && !sp.flipX && !isGrounded)
        {
            RIGHTisNearWallVault = true;
        }
        else
        {
            RIGHTisNearWallVault = false;
            LEFTisNearWallVault = false;
        }

    }


    #endregion


    //INPUTS
    #region
    //INPUTS

   
    public void OnBoomerang(InputAction.CallbackContext context) 
    {
        Debug.Log("AHHHHH BOOMERANG");

        if (context.performed && PlayerPrefs.GetInt("Bless1") == 1 && !cantMove)
        {
            //spawn a prefab
            Debug.Log("boomer is thrown");
            if(canBoomer)
            {
                canBoomer = false;
                GameObject boomer = Instantiate(boomerang, transform.position, Quaternion.identity);
                boomer.GetComponent<BoomerangScript>().isFacingRight = !sp.flipX;
                Invoke("resetBoomer", boomerangCooldown);
            }
        }
       
    }


    public void resetBoomer()
    {
        canBoomer = true;
    }

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

    //JUMPING
    #region

    private bool wallJumping = false;

    private bool isJumping = false;
    private bool shortHop = false;
    private Coroutine delayedJumpCoruotine;


    private bool wasGroundedCoyote()
    {
        return Time.time - lastTimeGrounded <= coyoteTime;
    }
    private IEnumerator delayedJump(float duration, InputAction.CallbackContext context)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            if ((isGrounded || (wasGroundedCoyote() && rb.velocity.y <= 0)) && !cantMove && !isJumping)
            {
                isJumping = true;
                Invoke("jumpLaunch", 0.06f);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }


    }

    private void jumpLaunch() // just separated to make timing easier by calling this with invoke for jumpsquat timing
    {


        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isGrounded = false;
        isJumping = false;
        animator.SetBool("ascending", true);
        CreateJumpDust();

        if (shortHop)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.45f);
            shortHop = false;
        }

    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && (isGrounded || (wasGroundedCoyote() && rb.velocity.y <= 0)) && !cantMove && !isJumping && !isHeavySlashing)
        {
            isJumping = true;
            shortHop = false;
            Invoke("jumpLaunch", 0.06f);

        }
        else if (context.started && !isGrounded && isLeftWallTouching && !cantMove)
        {
            if (wallJumping == false || PlayerPrefs.GetInt("WallJump") == 1)
            {
                if (CantMoveCoroutine != null)
                {
                    StopCoroutine(CantMoveCoroutine);
                }
                wallJumping = true;
                CantMoveCoroutine = StartCoroutine(endCantMove(0.14f));

                rb.velocity = Vector2.zero;
                rb.velocity = new Vector2(wallJumpXForce, wallJumpYForce);
                animator.SetBool("ascending", true);
                CreateJumpDust();
            }

        }
        else if (context.started && !isGrounded && isRightWallTouching && !cantMove)
        {
            if (wallJumping == false || PlayerPrefs.GetInt("WallJump") == 1)
            {
                if (CantMoveCoroutine != null)
                {
                    StopCoroutine(CantMoveCoroutine);
                }
                wallJumping = true;

                CantMoveCoroutine = StartCoroutine(endCantMove(0.14f));

                rb.velocity = Vector2.zero;
                rb.velocity = new Vector2(-wallJumpXForce, wallJumpYForce);
                animator.SetBool("ascending", true);
                CreateJumpDust();
            }

        }
        else if (context.started && !cantMove && !isJumping)
        {
            delayedJumpCoruotine = StartCoroutine(delayedJump(0.09f, context));
        }
        else if (context.canceled)
        {
            if (rb.velocity.y > 0)
            {
                if (!cantMove) //this is a bad fix rn but idk how to change it rn and it dont cause immediate problems
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.45f);
                }
            }
            else if (isJumping || delayedJumpCoruotine != null)
            {
                shortHop = true;
            }
        }
    }
    #endregion






    public void OnSlash(InputAction.CallbackContext context)
    {
        if (context.started && !logicScript.isPaused && !cantMove)
        {

            if (!attackOnCooldown)
            {
                StartAttack();
            } else
            {
                attackQueued = true;
            }

        }
        else if (context.canceled)
        {

        }
    }

    public void OnHeavySlash(InputAction.CallbackContext context)
    {
        
        if (context.started && !logicScript.isPaused && isGrounded && !isVaulting && !isJumping && PlayerPrefs.GetFloat("CanHeavySlash") == 1 && !heavySlashOnCooldown && !cantMove)
        {
            isAttacking = false;
            animator.SetBool("attacking", false);
            isVaulting = false;
            animator.SetBool("vaulting", false);
            attackQueued = false;
            Debug.Log("Heavy Slashing");
            StartHeavySlash();
            //HeavySlash
        }
    }

    public void OnVault(InputAction.CallbackContext context)
    {
        if (PlayerPrefs.HasKey("CanVault"))
        {
            if (PlayerPrefs.GetFloat("CanVault") == 1)
            {
                if (context.started && !logicScript.isPaused && !cantMove)
                {
                    if (!isVaulting && vaultCooldown <= 0)
                    {
                        StartVault();
                        vaultCooldown = maxVaultCooldown;
                    }
                }
            }
        }


    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!logicScript.isPaused)
            {
                logicScript.MenuPause();
            }
            else
            {
                logicScript.MenuUnPause();
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && rb.velocity.Equals(Vector2.zero))
        {

            // Directly modify the local position of the Particle System
            Vector3 newPosition = interactFlash.transform.localPosition;
            newPosition.x = sp.flipX ? -Mathf.Abs(newPosition.x) : Mathf.Abs(newPosition.x);
            interactFlash.transform.localPosition = newPosition;

            interactFlash.Play();

           // Debug.Log("Interacting");
            isInteracting = true;
            Invoke("StopInteracting", 0.1f);
        }
        //if (context.canceled)
        //{
        //    //nothing
        //}
    }

    //public void OnPro(InputAction.CallbackContext context)
    //{
    //    if (context.started)
    //    {
    //        Debug.Log("PRO CONTROLLER");

    //    }

    //}


    #endregion


    // RESPAWNING
    #region

    public void SetNewCampfire(GameObject newCampfire)
    {
  
        // Check if the last campfire exists before trying to put it out
        if (lastCampfire != null)
        {
            var lastCampfireScript = lastCampfire.GetComponent<CampfireParticleScript>();
            if (lastCampfireScript != null)
            {
                //lastCampfireScript.PutOut();
            }
        }

        lastCampfire = newCampfire;

        logicScript.savePoint(newCampfire);
        
    }

    void SetGroundPoint()
    {
        if (isGrounded && !invincible && !hasBeenSpiked) {
            if(!invincible && !hasBeenSpiked)
            {
                lastGroundPoint = transform.position + new Vector3(0, 0.2f, 0);
            }
        }

    }


    private bool hasBeenSpiked = false;
    public void PlayerDeath(bool trueDeath) //fades to black. 
                                            //on true death, reset hp and load the player back at a save point.
                                            // otherwise, just load them at the last safe point and subtract a hitpoint.
    {

        
        cam.GetComponent<CameraMovementScript>().fadeToBlack(0.15f);


        if (CantMoveCoroutine != null)
        {
            StopCoroutine(CantMoveCoroutine);
        }
        CantMoveCoroutine = StartCoroutine(endCantMove(1.5f));
        vaultCooldown = 1.5f;

        if (!trueDeath && currHP >= 1.5 && !hasBeenSpiked)
        {
            moveDirection = Vector2.zero;
            animator.SetFloat("speed", Mathf.Abs(moveDirection.x));

            if (!hasBeenSpiked)
            {
                currHP--;
            }
            hasBeenSpiked = true;

            logicScript.UpdateHealth();
            Invoke("ResetPlayerPos", 0.1f);
        } else if(!hasBeenSpiked)
        {
            Debug.Log("DEAD");
            currHP = maxHP;
            Invoke("LoadSavePoint", 0.15f);
        }

        sp.enabled = false;
        rb.velocity = Vector2.zero;

        sp.color = new Color(1, 0.65f, 0.65f, 0.7f);

        if (InvincibleCoroutine != null)
        {
            StopCoroutine(InvincibleCoroutine);
        }
        InvincibleCoroutine = StartCoroutine(endInvincible(1.5f));


    }

    void ResetPlayerPos()
    {
        cantMove = true;
        transform.position = lastGroundPoint;
        Invoke("EnableSprite", 0.5f);
    }

    #endregion


    //COROUTINES
    #region
    private IEnumerator endInvincible(float duration)
    {
        invincible = true;

        yield return new WaitForSeconds(duration);

        hasBeenSpiked = false;
        invincible = false;
        sp.color = Color.white;
    }

    private IEnumerator attackDelay(float duration)
    {
        attackOnCooldown = true;
        yield return new WaitForSeconds(duration);
        attackOnCooldown = false;
        if (attackQueued)
        {
            nextSwingIsDownward = !nextSwingIsDownward;
            StartAttack();
            attackQueued = false;
        } else
        {
            nextSwingIsDownward = true;
        }
    }

    private IEnumerator HeavySlashDelay(float duration)
    {
        heavySlashOnCooldown = true;
        yield return new WaitForSeconds(duration);
        heavySlashOnCooldown = false;
    }

    public IEnumerator endCantMove(float duration)
    {
        cantMove = true;

        yield return new WaitForSeconds(duration);

        cantMove = false;
        flipLock = false;

        maxSpeedHorizontalAllowed = normalMoveSpeed;

    }

    private IEnumerator ApplyVaultVel(Vector2 vel, float duration)
    {
        for (int i = 0; i < duration * 50; i++)
        {
            if (hasBeenSpiked)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.02f);
            if (vel.x > 0) // if travelling right
            {
                if (moveVector.x > 0) //if holding right
                {
                    rb.velocity = new Vector2(vel.x, rb.velocity.y); //keep applying velocity
                }
                else if (moveVector.x < 0) //if holding left
                {
                    break; //stop applying velocity
                }
            } else if (vel.x < 0) //if travelling left
            {
                if (moveVector.x < 0) //if holding left
                {
                    rb.velocity = new Vector2(vel.x, rb.velocity.y); //keep applying velocity
                }
                else if (moveVector.x > 0) //if holding right
                {
                    break; //stop applying velocity
                }
            }
            //EndVault();


        }
    }

    #endregion


    //UTILITIES
    #region
    void EnableSprite()
    {
        sp.enabled = true;
    }

    public void StartAttack()
    {
        if (!isAttacking && !isVaulting && !animator.GetBool("injured"))
        {  
            isAttacking = true;
            animator.SetBool("attacking", true);
            animator.SetBool("swingingDown", nextSwingIsDownward);
            StartCoroutine(attackDelay(maxSlashCoolDown));
        }
        
    }

    public void EndUpAttack()
    {
        //Debug.Log("Up Animation Played");
        isAttacking = false;
        animator.SetBool("attacking", false);

        //if (attackQueued)
        //{
        //    attackQueued = false;
        //    StartAttack();
        //}
    }
    public void EndDownAttack()
    {
        //Debug.Log("down Animation Played");
        isAttacking = false;
        animator.SetBool("attacking", false);

        //if (attackQueued)
        //{
        //    attackQueued = false;
        //    StartAttack();
        //}
    }

    public void StartVault()
    {
        if (!isAttacking && !isVaulting && !animator.GetBool("injured"))
        {
            animator.SetBool("attacking", false);
            isVaulting = true;
            animator.SetBool("vaulting", true);
        }
    }

    public void EndVault()
    {
        isVaulting = false;
        animator.SetBool("vaulting", false);
    }

    public void StopInteracting()
    {
        isInteracting = false;
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

    public void LoadSavePoint()
    {
        PlaySaveSound();
        //logicScript.loadSavePoint();
        logicScript.gameStartSceneLoad();
        sp.enabled = true;
    }



    public void StartHeavySlash()
    {
        cantMove = true;
        HaltVelocity();
        flipLock = true;
        if (!isHeavySlashing)
        {
            
            Debug.Log("Heavy Slash Started");
            isHeavySlashing = true;
            animator.SetBool("heavySlashing", true);
            StartCoroutine(HeavySlashDelay(maxHeavySlashCooldown));
        }
    }

    public void EndHeavySlash()
    {
        cantMove = false;
        flipLock = false;
        Debug.Log("Heavy Slash Ended");
        isHeavySlashing = false;
        animator.SetBool("heavySlashing", false);

    }


    public void InjuredStop()
    {
        animator.SetBool("injured", false);
    }

    #endregion



    //SOUNDS
    #region
    public void PlayFootstepSound()
    {
        int i = Random.Range(0, footstepSounds.Length);
        audioSource.PlayOneShot(footstepSounds[i]);
    }

    public void PlayHitSound()
    {
        int i = Random.Range(0, attackHitSounds.Length);
        audioSource.PlayOneShot(attackHitSounds[i]);
    }

    public void PlaySwingSound()
    {
        int i = Random.Range(0, swingSounds.Length);
        audioSource.PlayOneShot(swingSounds[i]);
    }

    public void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpLaunchSound);
    }

    //public void PlayLandSound()
    //{
    //    audioSource.PlayOneShot(jumpLandSound);
    //}

    public void PlayVaultSound()
    {
        audioSource.PlayOneShot(vaultSound);
    }

    public void PlayHPLossSound()
    {
        audioSource.PlayOneShot(playerGotHitSound);
        audioSource.PlayOneShot(hpShatterSound);
    }

    public void PlaySaveSound()
    {
        audioSource.PlayOneShot(saveSound);
    }


    #endregion

    //SAVING + Money
    #region


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Unnecessary due to campfire Changes
        //if (collision.gameObject.CompareTag("SavePoint"))
        //{
        //    logicScript.savePoint(collision.transform.position);
        //} else

        //Obsolete!
        //if (collision.gameObject.CompareTag("money"))
        //{
        //    logicScript.addMoney(1);
        //    Destroy(collision.gameObject);
        //}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("money"))
        {
            audioSource.PlayOneShot(moneySound);
        }
        if (collision.gameObject.CompareTag("HealthUp"))
        {
            PlayerPrefs.SetInt("MaxHP", PlayerPrefs.GetInt("MaxHP") + 1);
            Destroy(collision.gameObject);
        }
    }
    #endregion



}
