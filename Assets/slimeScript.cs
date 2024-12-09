using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimeScript : MonoBehaviour
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
    public Vector2 activateMoveDistance;
    public Vector2 activateAttackDistance;


    //movement privates
    int directionMoveX;
    // int directionMoveY;

    float distanceToPlayerX;
    float distanceToPlayerY;
    int directionToPlayerX;
    //int directionToPlayerY;

    //RayCasting
    private float xRayDistance;
    public LayerMask WallLayer;
    public LayerMask GroundLayer;
    Vector3 leftOffset;
    Vector3 rightOffset;

    //Enemy-type bools
    public bool stillGrounded;


    AudioSource audioSource; //in case movement sound effects are necessary


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
        monsterLogic = GetComponent<MonsterLogicScript>();

        xRayDistance = (transform.localScale.x / 2) + 0.3f; // this is the variable used for the laser distance to check for walls

        leftOffset = Vector3.zero;
        rightOffset = Vector3.zero;
        leftOffset.x = -(transform.localScale.x / 2);
        leftOffset.y = -(transform.localScale.y / 2);
        rightOffset.x = transform.localScale.x / 2;
        rightOffset.y = -(transform.localScale.y / 2);
    }

    private void FixedUpdate()
    {
        SetDirections();
        CheckForGround();
        CheckForWalls();
        SetMovement();
    }

    #endregion

    //Movement
    #region

    private void CycleMove(bool isClose)
    {

        if (!monsterLogic.cantMove)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

            if (isClose && !isStarted)
            {
                move();
            }
        }
    }

    private bool isStarted;
    private void move()
    {
        isStarted = true;
        rb.velocity = new Vector2(moveSpeed * directionMoveX, rb.velocity.y);
        Invoke("stop", 1.5f);
    }

    private void stop()
    {
        Debug.Log("AHHH");
        rb.velocity = new Vector2(0, rb.velocity.y);
        Invoke("move", 1.2f);
        
            directionMoveX = -directionMoveX;
        sp.flipX = !sp.flipX;


    }

    private void SetDirections()
    {
        distanceToPlayerX = playerScript.transform.position.x - transform.position.x;
        distanceToPlayerY = playerScript.transform.position.y - transform.position.y;

        if (distanceToPlayerX < 0)
        {
            directionToPlayerX = -1;
        }
        else
        {
            directionToPlayerX = 1;
        }

        //if (distanceToPlayerY < 0)
        //{
        //    directionMoveY = -1;
        //}
        //else
        //{
        //    directionMoveY = 1;
        //}
    }


    private void SetMovement()
    {
        if (Mathf.Abs(distanceToPlayerX) < activateMoveDistance.x && Mathf.Abs(distanceToPlayerY) < activateMoveDistance.y)
        {
            CycleMove(true); // true = the player is close
        }
        else
        {
            CycleMove(false); // false = the player is not close, (still activates move cycle, but only does the idle move cycle)
        }


        //if (Mathf.Abs(distanceToPlayerX) < activateAttackDistance.x && Mathf.Abs(distanceToPlayerY) < activateAttackDistance.y)
        //{
        //    Attack();
        //}

        if (Mathf.Abs(rb.velocity.x) > 0)
        {
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);

        }
    }
    #endregion


    //Raycasting
    #region
    private void CheckForGround()
    {
        RaycastHit2D rightSide = Physics2D.Raycast(transform.position + rightOffset, Vector2.down, .1f, GroundLayer);
        RaycastHit2D leftSide = Physics2D.Raycast(transform.position + leftOffset, Vector2.down, .1f, GroundLayer); // the offsets next to GroundLayer are how long the rays are

        //if (rightSide.collider == null && leftSide.collider == null)
        //{
        //}
        //else
        //{
        //}

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



    private void Attack()
    {
        // idk just spawn a hurtbox in the direction the player is, make speed 0 while doing so
        // might need like a bool to keep it from triggering multiple times?
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
