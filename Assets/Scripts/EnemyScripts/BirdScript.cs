using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : MonoBehaviour
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
    int directionMoveY;

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

        rb.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        SetDirections();
        SetMovement();
        
    }

    #endregion

    //Movement
    #region

    private void CycleMove(bool isClose)
    {

        if (!monsterLogic.cantMove)
        {
            rb.velocity = Vector2.zero;

            if (isClose)
            {
                rb.velocity = new Vector2(moveSpeed * directionMoveX, (moveSpeed / 2) * directionMoveY); // need to make a direction x and y
            }

        }
    }

    private void SetDirections()
    {
        distanceToPlayerX = playerScript.transform.position.x - transform.position.x;
        distanceToPlayerY = playerScript.transform.position.y - transform.position.y;
        directionMoveX = directionToPlayerX;
        if (distanceToPlayerX < 0)
        {
            directionToPlayerX = -1;
        }
        else
        {
            directionToPlayerX = 1;
        }

        if (distanceToPlayerY < 0)
        {
            directionMoveY = -1;
        }
        else
        {
            directionMoveY = 1;
        }

        //TODO: make the bird fly to a space above the player, so that it can dive down and hit it
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
    



