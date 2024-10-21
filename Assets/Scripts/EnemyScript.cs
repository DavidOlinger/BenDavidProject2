using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    //Basic Variables
    private Rigidbody2D rb;
    private SpriteRenderer sp;

    PlayerScript playerScript;
    public int hpMax;
    public int hitCounter;

    //Movement public
    public float moveSpeed;
    public float fastMoveSpeed;
    public Vector2 activateMoveDistance;
    public Vector2 activateAttackDistance;


    //movement privates
    int directionMove = -1;
    float distanceToPlayerX = 0;
    float distanceToPlayerY = 0;
    int directionToPlayer = 0;
    private bool cantMove = false;

    //RayCasting
    private float xRayDistance;
    public LayerMask WallLayer;
    public LayerMask GroundLayer;
    Vector3 leftOffset;
    Vector3 rightOffset;

    //Enemy-type bools
    public bool stillGrounded;
    public bool stillFloating;
    public bool HoriMover;
    public bool standFollow;
    public bool floatFollow;
    public bool slasher;
    public bool shooter;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();

        xRayDistance = (transform.localScale.x / 2) + 0.05f;

        leftOffset = Vector3.zero;
        rightOffset = Vector3.zero;
        leftOffset.x = -(transform.localScale.x / 2);
        leftOffset.y = -(transform.localScale.y / 2);
        rightOffset.x = transform.localScale.x / 2;
        rightOffset.y = -(transform.localScale.y / 2);

        if(stillFloating || floatFollow)
        {
            rb.gravityScale = 0;
        }
    }

    private void FixedUpdate()
    {
        CheckForGround();
        CheckForWalls();

        distanceToPlayerX = playerScript.transform.position.x - transform.position.x;
        distanceToPlayerY = playerScript.transform.position.y - transform.position.y;

        if(distanceToPlayerX < 0)
        {
            directionToPlayer = -1;
        }
        else
        {
            directionToPlayer = 1;
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
        

    }


    private void Attack()
    {
        // idk just spawn a hurtbox in the direction the player is, make speed 0 while doing so
        // might need like a bool to keep it from triggering multiple times?
    }

    private void CycleMove(bool isClose)
    {
        if (!cantMove)
        {
            if (stillGrounded)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            if (stillFloating)
            {
                rb.velocity = Vector2.zero;
            }
            if (HoriMover)
            {
                rb.velocity = new Vector2(moveSpeed * directionMove, rb.velocity.y);
            }
            if (standFollow)
            {
                if (isClose)
                {
                    sp.color = Color.yellow;
                    rb.velocity = new Vector2(moveSpeed * directionToPlayer, rb.velocity.y);
                }
                else
                {
                    sp.color = Color.red;
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
            if (floatFollow)
            {
                if (isClose)
                {
                    sp.color = Color.yellow;
                    rb.velocity = new Vector2(moveSpeed * directionMove, moveSpeed * directionMove); // need to make a direction x and y
                }
                else
                {
                    sp.color = Color.red;
                    rb.velocity = Vector2.zero;
                }
            }
        }
    }




    //RAYCASTING STUFF
    private void CheckForGround()
    {
        RaycastHit2D rightSide = Physics2D.Raycast(transform.position + rightOffset, Vector2.down, .02f, GroundLayer);
        RaycastHit2D leftSide = Physics2D.Raycast(transform.position + leftOffset, Vector2.down, .02f, GroundLayer);

        if (rightSide.collider == null && rb.velocity.y == 0)
        {
            directionMove = -1;
        }
        else if (leftSide.collider == null && rb.velocity.y == 0)
        {
            directionMove = 1;
        }
        
    }
    private void CheckForWalls()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, xRayDistance, WallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, xRayDistance, WallLayer);

        if (hitLeft.collider != null)
        {
            directionMove = 1;
        }

        if (hitRight.collider != null)
        {
            directionMove = -1;
        }
    }


    //COMBAT
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slash"))
        {

            float hitLaunch = transform.position.x - collision.transform.position.x;

            //playerScript.slashKnockback(hitLaunch);

            hitCounter++;

            if (hitCounter >= hpMax)
            {
                Destroy(gameObject);
            }


            cantMove = true;
            Invoke("endCantMove", 0.06f);

            rb.velocity = new Vector2(0, rb.velocity.y);

            if(playerScript.moveDirection.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, playerScript.knockbackOnHit);
            }
            else if(playerScript.moveDirection.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, -playerScript.knockbackOnHit);
            }
            else if (playerScript.lookingRight)
            {
                rb.velocity = new Vector2(playerScript.knockbackOnHit, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(-(playerScript.knockbackOnHit), rb.velocity.y);
            }

        }

        if (collision.CompareTag("PlayerDmg") && !playerScript.invincible)
        {
            playerScript.takeDamage(gameObject);

        }

    }

    private void endCantMove()
    {
        cantMove = false;
        rb.velocity = Vector2.zero;
    }




}
