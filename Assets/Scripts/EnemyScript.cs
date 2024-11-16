using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    //Variables

    private bool isHopping = false;


    #region
    //Basic Variables
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    Animator animator;

    PlayerScript playerScript;
    GameObject player;
    public int hpMax;
    public int hitCounter;

    //publics
    public float moveSpeed;
    public float fastMoveSpeed;
    public Vector2 activateMoveDistance;
    public Vector2 activateAttackDistance;
    public float dmgHitStun;


    //movement privates
    int directionMoveX = -1;
    int directionMoveY = 0;
    float distanceToPlayerX = 0;
    float distanceToPlayerY = 0;
    int directionToPlayerX = 0;
    //int directionToPlayerY= 0;
    public bool cantMove = false;

    //RayCasting
    private float xRayDistance;
    public LayerMask WallLayer;
    public LayerMask GroundLayer;
    Vector3 leftOffset;
    Vector3 rightOffset;

    //Enemy-type bools
    public bool stillGrounded;
    public bool isStump;
    public bool isBird;
    public bool isHopper;

    //other
    private Coroutine CantMoveCoroutine;

    private Coroutine hoppingCoroutine;

    [SerializeField] ParticleSystem killParticles;
    [SerializeField] ParticleSystem hitParticles;
    [SerializeField] ParticleSystem moneyParticles;
    [SerializeField] int moneyOnKill;

    AudioSource audioSource;
    [SerializeField] AudioClip deathSound;

    #endregion

    //start + update
    #region
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
        audioSource = GetComponent<AudioSource>();

        xRayDistance = (transform.localScale.x / 2) + 0.3f; // this is the variable used for the laser distance to check for walls

        leftOffset = Vector3.zero;
        rightOffset = Vector3.zero;
        leftOffset.x = -(transform.localScale.x / 2);
        leftOffset.y = -(transform.localScale.y / 2);
        rightOffset.x = transform.localScale.x / 2;
        rightOffset.y = -(transform.localScale.y / 2);

        if(isBird)
        {
            rb.gravityScale = 0;
        }
    }

    private void FixedUpdate()
    {
        

        distanceToPlayerX = playerScript.transform.position.x - transform.position.x;
        distanceToPlayerY = playerScript.transform.position.y - transform.position.y;

        if(distanceToPlayerX < 0)
        {
            directionToPlayerX = -1;
        }
        else
        {
            directionToPlayerX = 1;
        }

        if(distanceToPlayerY < 0)
        {
            directionMoveY = -1;
        }
        else
        {
            directionMoveY = 1;
        }



        if (!isBird)
        {
            CheckForGround();
            CheckForWalls();
        }
        else
        {
            directionMoveX = directionToPlayerX;
        }


        if (Mathf.Abs(distanceToPlayerX) < activateMoveDistance.x && Mathf.Abs(distanceToPlayerY) < activateMoveDistance.y)
        {
            CycleMove(true); // true = the player is close
        }
        else
        {
            CycleMove(false); // false = the player is not close, (still activates move cycle, but only does the idle move cycle)
        }



        if (Mathf.Abs(distanceToPlayerX) < activateAttackDistance.x && Mathf.Abs(distanceToPlayerY) < activateAttackDistance.y)
        {
            Attack();
        }
        
        if (Mathf.Abs(rb.velocity.x) > 0){
            animator.SetBool("walking", true);
        } else
        {
            animator.SetBool("walking", false);

        }

    }

    #endregion

    //Movement
    #region
   

    private void CycleMove(bool isClose)
    {
        
        if (!cantMove)
        {
            if (stillGrounded)
            {
                
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            if (isStump)
            {
                if (isClose)
                {
                    
                    rb.velocity = new Vector2(moveSpeed * directionMoveX, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
            if (isBird)
            {
                if (isClose)
                {
                    rb.velocity = new Vector2(moveSpeed * directionMoveX, (moveSpeed / 2) * directionMoveY); // need to make a direction x and y
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
            }
            if (isHopper)
            {
                if (isClose)
                {
                    if (!isHopping)
                    {
                        hoppingCoroutine = StartCoroutine(HopRoutine());
                    }
                    else if(isGrounded)
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
            }
        }
    }

    private bool isGrounded;
    #endregion


    //RAYCASTING STUFF
    #region
    private void CheckForGround()
    {
        RaycastHit2D rightSide = Physics2D.Raycast(transform.position + rightOffset, Vector2.down, .1f, GroundLayer);
        RaycastHit2D leftSide = Physics2D.Raycast(transform.position + leftOffset, Vector2.down, .1f, GroundLayer); // the offsets next to GroundLayer are how long the rays are

        if (rightSide.collider == null &&  leftSide.collider == null)
        {
            animator.SetBool("grounded", false);
        } else
        {
            animator.SetBool("grounded", true);
        }

        if (rightSide.collider == null && rb.velocity.y == 0)
        {
            directionMoveX = -1;
            sp.flipX = true;
        }
        else if (leftSide.collider == null && rb.velocity.y == 0)
        {
            directionMoveX = 1;
            sp.flipX = false;
        }
        
    }
    private void CheckForWalls()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, xRayDistance, WallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, xRayDistance, WallLayer);

        if (hitLeft.collider != null)
        {
            directionMoveX = 1;
            sp.flipX = false;

        }

        if (hitRight.collider != null)
        {
            directionMoveX = -1;
            sp.flipX = true;

        }
    }

    #endregion




    //COMBAT
    #region
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slash") || collision.CompareTag("Vault"))
        {

            //float hitLaunch = transform.position.x - collision.transform.position.x;
            playerScript.PlayHitSound();
            //playerScript.slashKnockback(hitLaunch);
            if (collision.CompareTag("Slash"))
            {
                
                playerScript.hitStop(0.08f, 0.01f);
            } else
            {
                playerScript.hitStop(0.02f, 0.01f); // this part needs to be made so it actually just checks for vault
            }

            hitCounter++;
            if (hitParticles != null) 
            { 
            ParticleSystem onHitParticles = Instantiate(hitParticles, transform.position, Quaternion.identity);
            Destroy(onHitParticles.gameObject, 3);
            }
            
            PlayDamagedAnim(0.4f);
            if (hitCounter >= hpMax)
            {
                Kill();
                Debug.Log("Enemy Kill");
            }


            //rb.gravityScale = 0;
            rb.velocity = new Vector2(0, rb.velocity.y);

            sp.color = new Color(1, 1, 1, 0.5f); // to simulate the white flash for now lol


            if (CantMoveCoroutine != null)
            {
                StopCoroutine(CantMoveCoroutine);
            }
            CantMoveCoroutine = StartCoroutine(endCantMove(0.08f + playerScript.stunOnHit)); // ends cantMove after 4 frames + how long the hitStun will be



            // rb.velocity = new Vector2(0, rb.velocity.y);

            Invoke("dmgLaunch", playerScript.stunOnHit); // this should prob be a coroutine thing too lol
            //essentially activates the knockback after the stun time wears off
        }

        if (collision.CompareTag("PlayerDmg") && !playerScript.invincible)
        {
            playerScript.takeDamage(gameObject, dmgHitStun, 1);
            //Debug.Log("Time Should have stopped - enemy");
            //playerScript.hitStop(0.24f, 0.01f);

        }

    }

    

    private void dmgLaunch()
    {

        sp.color = Color.white;

        //rb.gravityScale = 1; 

        

        float PlayerToEnemyPosition = player.transform.position.x - transform.position.x;
        if (PlayerToEnemyPosition < 0)
        {
            rb.velocity = new Vector2(playerScript.knockbackOnHit, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-(playerScript.knockbackOnHit), rb.velocity.y);
        }
    }  //Applies the knockback from the players slash












    private void Attack()
    {
        // idk just spawn a hurtbox in the direction the player is, make speed 0 while doing so
        // might need like a bool to keep it from triggering multiple times?
    }
    #endregion

    //Other stuff COROUTINES
    #region

    private IEnumerator endCantMove(float duration)
    {

        cantMove = true;

        yield return new WaitForSeconds(duration);

        cantMove = false;
        rb.velocity = Vector2.zero;
        //rb.velocity = Vector2.zero; //i forget why this is here but it was before so i kept it
    }


    private IEnumerator HopRoutine()
    {
        //START DOWN ANIMATION IF WE HAVE ONE I FORGET
        isHopping = true;
        
        Invoke("JumpLaunch", .8f);
        yield return new WaitForSeconds(3.5f);

        isHopping = false;
    }

    private void JumpLaunch()
    {
        //START JUMPING ANIMATION
        rb.velocity = new Vector2(moveSpeed * distanceToPlayerX, jumpSpeed);
        if (isHopper) //only hopper can do this
        {
            animator.SetTrigger("jump");
            animator.SetBool("hopperGrounded", false);
            
        }
        
        isGrounded = false;
        

    }
    public float jumpSpeed;

    private void PlayDamagedAnim(float duration)
    {
        if(isHopper || isBird) //these enemies use triggers
        {
            animator.SetTrigger("hurt");
        }
        else
        {
            animator.SetBool("hurt", true);
            Invoke("CancelDamageAnim", duration);
        }
        
    }

    private void CancelDamageAnim()
    {
        animator.SetBool("hurt", false);
    }

    public void Kill()
    {

        audioSource.PlayOneShot(deathSound);

        // play death sound
        if (killParticles != null)
        {
            ParticleSystem onKillParticles = Instantiate(killParticles, transform.position, Quaternion.identity);
            Destroy(onKillParticles.gameObject, 5f);
            if (isHopper) // hoppers need more death particles
            {
                ParticleSystem splatter2 = Instantiate(killParticles, transform.position, Quaternion.identity);
                Destroy(splatter2.gameObject, 5f);
            }

        }

        if (moneyParticles != null)
        {
            ParticleSystem moneyDropParticles = Instantiate(moneyParticles, transform.position, Quaternion.identity);

            // Modify the burst count
            var emission = moneyDropParticles.emission;
            var burst = emission.GetBurst(0); 
            burst.count = moneyOnKill;        
            emission.SetBurst(0, burst);     
            moneyDropParticles.Play();

            Destroy(moneyDropParticles.gameObject, 30f);
            

        }

        Destroy(gameObject);
    }

    #endregion


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") && isHopper)
        {
            
            
            animator.SetBool("hopperGrounded", true);
            Debug.Log("groundedtrigger");
            
            rb.velocity = Vector2.zero;
            
        }
    }
}
