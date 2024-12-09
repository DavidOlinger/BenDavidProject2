using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChargerScript : MonoBehaviour
{
    #region
    //Basic Variables
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    Animator animator;

    PlayerScript playerScript;
    GameObject player;
    MonsterLogicScript monsterLogic;

    //publics
    public float moveSpeed;
    public bool stillGrounded;
    public bool aggroOnPlayer;
    public float aggroRadius;
    public float deAggroRadius;
    public float attackRange;
    public float chargeSpeed;
    public float attackDuration;
    public float attackCooldown;
    public float chargeWarningWindow;
    public float feetLevel;
    public float wallClimbSpeed;
    public float unclimbableHeight;

    [SerializeField] ParticleSystem chargeReadyParticles;
    [SerializeField] ParticleSystem chargeParticles;


    //movement privates
    public int directionMoveX;
    // int directionMoveY;

    float distanceToPlayerX;

    int directionToPlayerX;
    //int directionToPlayerY;

    //RayCasting
    private float xRayDistance;
    public LayerMask WallLayer;
    public LayerMask GroundLayer;
    Vector3 leftOffset;
    Vector3 rightOffset;

    //Enemy-type bools
    public bool isAttacking = false;



    AudioSource audioSource; //in case movement sound effects are necessary


    #endregion

    //start + update
    #region
    private void OnEnable()
    {
        isAttacking = false;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
        audioSource = GetComponent<AudioSource>();
        monsterLogic = GetComponent<MonsterLogicScript>();

        xRayDistance = (transform.localScale.x / 2) + 0.5f; // this is the variable used for the laser distance to check for walls

        leftOffset = Vector3.zero;
        rightOffset = Vector3.zero;
        leftOffset.x = -(transform.localScale.x / 2);
        leftOffset.y = -(transform.localScale.y / 2);
        rightOffset.x = transform.localScale.x / 2;
        rightOffset.y = -(transform.localScale.y / 2);
    }

    //private void Start()
    //{
    //    animator = GetComponent<Animator>();
    //    rb = GetComponent<Rigidbody2D>();
    //    sp = GetComponent<SpriteRenderer>();
    //    player = GameObject.FindGameObjectWithTag("Player");
    //    playerScript = player.GetComponent<PlayerScript>();
    //    audioSource = GetComponent<AudioSource>();
    //    monsterLogic = GetComponent<MonsterLogicScript>();

    //    xRayDistance = (transform.localScale.x / 2) + 0.5f; // this is the variable used for the laser distance to check for walls

    //    leftOffset = Vector3.zero;
    //    rightOffset = Vector3.zero;
    //    leftOffset.x = -(transform.localScale.x / 2);
    //    leftOffset.y = -(transform.localScale.y / 2);
    //    rightOffset.x = transform.localScale.x / 2;
    //    rightOffset.y = -(transform.localScale.y / 2);
    //}

    private void FixedUpdate()
    {
        SetDirections();
        //CheckForGround();
        //CheckForWalls();
        Movement();
    }

    #endregion

    //Movement
    #region
    private void Movement()
    {
         animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        if (!monsterLogic.cantMove && !isAttacking) //if able to move
        {
            monsterLogic.ignoreStun = false;


            rb.velocity = new Vector2(0, rb.velocity.y);
            float distToPlayer = Vector2.Distance(transform.position, player.transform.position);


            if (distToPlayer < aggroRadius) //if close enough to see the player
            {
                aggroOnPlayer = true;
                //Debug.Log("AGGRO");

            }
            else if (distToPlayer > deAggroRadius) //if the player is far enough to de aggro
            {
                aggroOnPlayer = false; //de aggro
            }


            if (aggroOnPlayer)
            {
                if (!isAttacking)
                {
                    if (directionToPlayerX > 0) //aggro and face them
                    {
                        sp.flipX = false;
                    }
                    else
                    {
                        sp.flipX = true;
                    }
                }

                if (!isAttacking && distToPlayer < attackRange) //if close enough to attack
                {
                    //Debug.Log("ATTACKING");
                    isAttacking = true;
                    monsterLogic.ignoreStun = true;

                    StartCoroutine(AttackCoroutine(attackDuration)); //then attack
                }
                else //otherwise, chase
                {
                    Chase();
                }
            } else //if not aggro-ed, pace.
            {
                Pace();
            }
            
        }
    }

    private IEnumerator AttackCoroutine(float duration)
    {
        //change animation to readying charger
        rb.velocity = new Vector2(0, rb.velocity.y);

        Debug.Log("startingCharge");
        Destroy(Instantiate(chargeReadyParticles, transform.position, Quaternion.identity), 1); //play warning effect

        yield return new WaitForSeconds(chargeWarningWindow);

        //change animation to charging

        //charge frame 1
        animator.SetTrigger("charge1");
        Destroy(Instantiate(chargeParticles, transform.position, Quaternion.identity), 1);


        rb.velocity = new Vector2(moveSpeed * directionToPlayerX * chargeSpeed, rb.velocity.y);

        yield return new WaitForSeconds(duration / 2);

        //charge frame 2
        animator.SetTrigger("charge2");

        yield return new WaitForSeconds(duration / 2);

        //Not charging
        monsterLogic.ignoreStun = false;


        Debug.Log("endingCharge");
        animator.SetTrigger("notCharging");
        rb.velocity = new Vector2(0, rb.velocity.y);

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;

        yield return null;
    }




    private void SetDirections()
    {
        distanceToPlayerX = playerScript.transform.position.x - transform.position.x;
        if (distanceToPlayerX < 0)
        {
            directionToPlayerX = -1;
        }
        else
        {
            directionToPlayerX = 1;
        }
    }

    #endregion


    private void Chase()
    {
        Boolean[] groundChecks = ChaseGroundCheck();
        ChaseWallCheck();
        groundChecks = new Boolean[] { false, false}; //IGNORE EDGES
        
            
        if (directionToPlayerX > 0)
        {
            if (!(groundChecks[1]))
            {
                directionMoveX = 1;
            }
            else { directionMoveX = 0; }
        }
        else
        {
            if (!(groundChecks[0])){
                directionMoveX = -1;
            } else { directionMoveX = 0; }
                
        }
        rb.velocity = new Vector2(moveSpeed*directionMoveX, rb.velocity.y);
        

        
    }

    private void Pace()
    {
        
        CheckForGround();
        CheckForWalls();
        rb.velocity = new Vector2(moveSpeed * directionMoveX, rb.velocity.y);
    }
    

    //Raycasting
    #region
    private void CheckForGround()
    {
        RaycastHit2D rightSide = Physics2D.Raycast(transform.position + rightOffset, Vector2.down, transform.localScale.y / 2, GroundLayer);
        RaycastHit2D leftSide = Physics2D.Raycast(transform.position + leftOffset, Vector2.down, transform.localScale.y / 2, GroundLayer); // the offsets next to GroundLayer are how long the rays are

        if (rightSide.collider == null && leftSide.collider == null)
        {
            animator.SetBool("grounded", false);
        }
        else
        {
            animator.SetBool("grounded", true);
        }

        if (rightSide.collider == null && rb.velocity.y == 0)
        {
            Debug.Log("Right Edge");
            directionMoveX = -1;
            sp.flipX = true;
        }
        else if (leftSide.collider == null && rb.velocity.y == 0)
        {
            Debug.Log("Left Edge");

            directionMoveX = 1;
            sp.flipX = false;
        }

    }
    private void CheckForWalls()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position + new Vector3(0, feetLevel, 0), Vector2.left, xRayDistance, WallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + new Vector3(0, feetLevel, 0), Vector2.right, xRayDistance, WallLayer);
        Debug.DrawRay(transform.position + new Vector3(0, feetLevel, 0), Vector2.left * xRayDistance, Color.red);
        Debug.DrawRay(transform.position + new Vector3(0, feetLevel, 0), Vector2.right * xRayDistance, Color.green);

        if (hitLeft.collider != null)
        {   
            Debug.Log("LeftWall");
            directionMoveX = 1;
            sp.flipX = false;

        } else if (hitRight.collider != null)
        {
            Debug.Log("Right WALL");
            directionMoveX = -1;
            sp.flipX = true;
        }
    }

    private void ChaseWallCheck()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position + new Vector3(0, unclimbableHeight, 0), Vector2.left, xRayDistance, WallLayer);
        RaycastHit2D hitLeftLow = Physics2D.Raycast(transform.position + new Vector3(0, feetLevel, 0), Vector2.left, xRayDistance, WallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + new Vector3(0, unclimbableHeight, 0), Vector2.right, xRayDistance, WallLayer);
        RaycastHit2D hitRightLow = Physics2D.Raycast(transform.position + new Vector3(0, feetLevel, 0), Vector2.right, xRayDistance, WallLayer);


        if (hitLeftLow.collider != null && hitLeft.collider == null) //if hit a wall that i can climb
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y+wallClimbSpeed);
        }

        if (hitRightLow.collider != null && hitRight.collider == null) //if hit a wall i can climb
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + wallClimbSpeed);
        }
    }

    private Boolean[] ChaseGroundCheck()
    {
        Boolean rightEdge = false;
        Boolean leftEdge = false; 
        RaycastHit2D rightSide = Physics2D.Raycast(transform.position + rightOffset, Vector2.down, transform.localScale.y / 2, GroundLayer);
        RaycastHit2D leftSide = Physics2D.Raycast(transform.position + leftOffset, Vector2.down, transform.localScale.y / 2, GroundLayer); // the offsets next to GroundLayer are how long the rays are

        if (rightSide.collider == null && leftSide.collider == null)
        {
            animator.SetBool("grounded", false);
        }
        else
        {
            animator.SetBool("grounded", true);
        }

        if (rightSide.collider == null && rb.velocity.y == 0)
        {
            rightEdge = true;
        }
        else if (leftSide.collider == null && rb.velocity.y == 0)
        {
            leftEdge = true;
        }
        Boolean[] arr = {leftEdge, rightEdge};
        return arr;
    }
    #endregion



    //Other stuff COROUTINES
    #region

    private void PlayDamagedAnim(float duration)
    {
        animator.SetBool("hurt", true);
        Invoke("CancelDamageAnim", duration);

    }

    private void CancelDamageAnim()
    {
        animator.SetBool("hurt", false);
    }

    #endregion


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //nothing for now
        }
    }
}
