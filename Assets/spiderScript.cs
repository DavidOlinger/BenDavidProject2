using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderScript : MonoBehaviour
{
    #region
    //Basic Variables
    private Rigidbody2D rb;
   // private SpriteRenderer sp;
    Animator animator;

    PlayerScript playerScript;
    GameObject player;
    MonsterLogicScript monsterLogic;

    //publics
    public float moveSpeed;
    public Vector2 activateMoveDistance;
    public Vector2 activateAttackDistance;


    //movement privates
    //int directionMoveX;
    //int directionMoveY;

    float distanceToPlayerX;
    float distanceToPlayerY;
   // int directionToPlayerX;
    //int directionToPlayerY;

    //RayCasting
    private float xRayDistance;
    public float groundCheckDist;
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
       // sp = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
        audioSource = GetComponent<AudioSource>();
        monsterLogic = GetComponent<MonsterLogicScript>();

        xRayDistance = (transform.localScale.x / 2) + 0.3f; // this is the variable used for the laser distance to check for walls

        leftOffset = Vector3.zero;
        rightOffset = Vector3.zero;
        leftOffset.x = -(transform.localScale.x / 2);
        //leftOffset.y = -(transform.localScale.y / 2);
        rightOffset.x = transform.localScale.x / 2;
        // rightOffset.y = -(transform.localScale.y / 2);

        rb.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        SetDirections();
        SetMovement();
        CheckForGround();

    }

    #endregion

    //Movement
    #region


    public bool Diving = false;
    private void CycleMove(bool isClose)
    {
        if (!monsterLogic.cantMove)
        {
            if (rightHeight || Diving)
            {
                Diving = true;
                rb.velocity = new Vector2(0, -moveSpeed * 2);
            }
            else if (!rightHeight)
            {
                rb.velocity = new Vector2(0, moveSpeed);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

    }

    public bool rightHeight = false;
    public float stopBeforeGroundDist;
    private void CheckForGround()
    {

        Vector3 offset = new Vector3(1f, 0, 0);
        RaycastHit2D middle = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDist, GroundLayer);
        RaycastHit2D hitCeilingLeft = Physics2D.Raycast(transform.position - offset, Vector2.up, 0.6f, GroundLayer);
        RaycastHit2D hitCeilingRight = Physics2D.Raycast(transform.position + offset, Vector2.up, 0.6f, GroundLayer);
        RaycastHit2D hitGroundLeft = Physics2D.Raycast(transform.position - offset, Vector2.down, stopBeforeGroundDist, GroundLayer);
        RaycastHit2D hitGroundRight = Physics2D.Raycast(transform.position + offset, Vector2.down, stopBeforeGroundDist, GroundLayer);





        if (middle.collider == null)
        {
            rightHeight = true;
        }
        else if (hitCeilingLeft.collider != null)
        {
            rightHeight = true;
        }
        else if (hitCeilingRight.collider != null)
        {
            rightHeight = true;
        }
        else
        {
            rightHeight = false;
        }

        if (hitGroundLeft.collider != null || hitGroundRight.collider != null)
        {
            Diving = false;
            StartCoroutine(EndCantMove(0.8f));
            
        }
    }
    private IEnumerator EndCantMove(float duration) //TODO: Update the other script to use this cantmove value
    {

        monsterLogic.cantMove = true;

        yield return new WaitForSeconds(duration);

        monsterLogic.cantMove = false;
        rb.velocity = Vector2.zero;
    }


    private void SetDirections()
    {
        distanceToPlayerX = playerScript.transform.position.x - transform.position.x;
        distanceToPlayerY = playerScript.transform.position.y - transform.position.y;
        //directionMoveX = directionToPlayerX;
        //if (distanceToPlayerX < 0)
        //{
        //    directionToPlayerX = -1;
        //}
        //else
        //{
        //    directionToPlayerX = 1;
        //}

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


        //if (directly above the player)
        //{
        //    Attack(); //TODO: Dive down and attack the player before flying up again. (on a cooldown)
        //}


    }
    #endregion

    private void Attack() //TODO
    {
        // idk just spawn a hurtbox in the direction the player is, make speed 0 while doing so
        // might need like a bool to keep it from triggering multiple times?
    }

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
