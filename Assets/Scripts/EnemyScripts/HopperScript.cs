using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopperScript : MonoBehaviour
{
    private bool isHopping;
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
    public float jumpSpeed;
    public Vector2 activateMoveDistance;
    public Vector2 activateAttackDistance;


    //movement privates
    int directionMoveX;
   // int directionMoveY;

    float distanceToPlayerX;
    float distanceToPlayerY;
  //  int directionToPlayerX;
    //int directionToPlayerY;

    //RayCasting
    private float xRayDistance;
    public LayerMask WallLayer;
    public LayerMask GroundLayer;
    Vector3 leftOffset;
    Vector3 rightOffset;

    //Enemy-type bools
    public bool stillGrounded;

    private Coroutine hoppingCoroutine;
    private bool isGrounded;
    public float jumpDelay;

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
        SetMovement();
    }

    #endregion

    //Movement
    #region

    private void CycleMove(bool isClose)
    {

        if (!monsterLogic.cantMove)
        {
            
            if (isClose)
            {
                
                if (!isHopping)
                {
                    hoppingCoroutine = StartCoroutine(HopRoutine());
                }
                else if (isGrounded)
                {
                    rb.velocity = Vector2.zero;
                }
                

            }
        } 
    }

    private void SetDirections()
    {
        distanceToPlayerX = playerScript.transform.position.x - transform.position.x;
        distanceToPlayerY = playerScript.transform.position.y - transform.position.y;

        //if (distanceToPlayerX < 0)
        //{
        //    directionToPlayerX = -1;
        //}
        //else
        //{
        //    directionToPlayerX = 1;
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
    }
    #endregion



    private void CheckForGround()
    {
        RaycastHit2D rightSide = Physics2D.Raycast(transform.position + rightOffset, Vector2.down, .1f, GroundLayer);
        RaycastHit2D leftSide = Physics2D.Raycast(transform.position + leftOffset, Vector2.down, .1f, GroundLayer); // the offsets next to GroundLayer are how long the rays are

        if (rightSide.collider == null && leftSide.collider == null)
        {
            animator.SetBool("grounded", false);
            isGrounded = false;
        }
        else
        {
            animator.SetBool("grounded", true);
            isGrounded = false;
        }

    }


    //Other stuff COROUTINES
    #region
    private IEnumerator HopRoutine()
    {
        isHopping = true;

        Invoke("JumpLaunch", .1f);
        yield return new WaitForSeconds(jumpDelay);

        isHopping = false;
    }

    private void JumpLaunch()
    {
        //START JUMPING ANIMATION
        rb.velocity = new Vector2(moveSpeed * distanceToPlayerX, jumpSpeed);   
        animator.SetTrigger("jump");
        animator.SetBool("hopperGrounded", false);
        isGrounded = false;
    }

    private void PlayDamagedAnim(float duration)
    {
        animator.SetTrigger("hurt");

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
            animator.SetBool("hopperGrounded", true);
            rb.velocity = Vector2.zero;
        }
    }

}