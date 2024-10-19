using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    //Basic Variables
    Rigidbody2D rb;
    PlayerScript playerScript;
    public float hpMax;
    public float hitCounter;

    //Movement
    public float moveSpeed;
    int direction = -1;
    float distanceToPlayer = 0;

    //Enemy-type bools
    public bool mover;

    //RayCasting
    private float xRayDistance;
    public LayerMask WallLayer;
    public LayerMask GroundLayer;
    Vector3 leftOffset;
    Vector3 rightOffset;





    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();

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

        distanceToPlayer = playerScript.transform.position.x - transform.position.x;

        if(Mathf.Abs(distanceToPlayer) < 3)
        {
            moveSpeed = 3; //this is rly crude for now but we can adjust all this to make good enemy patterns
        }
        else
        {
            moveSpeed = 1; // again stupid for rn but there is an idea here lol
        }

        if(Mathf.Abs(distanceToPlayer) < 1)
        {
            Attack();
        }

        if (mover)
        {
            rb.velocity = new Vector2(moveSpeed * direction, rb.velocity.y);
        }

    }


    private void Attack()
    {
        // idk just spawn a hurtbox in the direction the player is, make speed 0 while doing so
        // might need like a bool to keep it from triggering multiple times?
    }




    //RAYCASTING STUFF
    private void CheckForGround()
    {
        RaycastHit2D rightSide = Physics2D.Raycast(transform.position + rightOffset, Vector2.down, .02f, GroundLayer);
        RaycastHit2D leftSide = Physics2D.Raycast(transform.position + leftOffset, Vector2.down, .02f, GroundLayer);

        if (rightSide.collider == null && rb.velocity.y == 0)
        {
            direction = -1;
        }
        else if (leftSide.collider == null && rb.velocity.y == 0)
        {
            direction = 1;
        }
        
    }
    private void CheckForWalls()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, xRayDistance, WallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, xRayDistance, WallLayer);

        if (hitLeft.collider != null)
        {
            direction = 1;
        }

        if (hitRight.collider != null)
        {
            direction = -1;
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slash"))
        {
            Debug.Log("hit detected");
            hitCounter++;
            if(hitCounter >= hpMax)
            {
                Destroy(gameObject);
            }
        }


        if (collision.CompareTag("PlayerDmg") && !playerScript.Invincible)
        {
            playerScript.takeDamage(gameObject);

        }


    }





}
