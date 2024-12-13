using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
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
    }

    private void FixedUpdate()
    {
        CheckRollDirection();
        SetMovementAnimation();
    }

    #endregion

    //Movement
    #region



    private void SetMovementAnimation()
    {
       //TODO: Roll in current move direction 
        if (Mathf.Abs(rb.velocity.x) > 0)
        {
          //  animator.SetBool("rolling", true);
        }
        else
        {
           // animator.SetBool("rolling", false);

        }
    }
    #endregion

    public float groundCheckDist;
    public float offsetDistance;



    //Raycasting
    #region
    private void CheckRollDirection()
    {
        Vector3 Odown = new Vector3(offsetDistance, 0, 0);
        //Vector3 Odown2 = new Vector3(offsetDistance - 0.02f, 0, 0);
        Vector3 Oright= new Vector3(0, -offsetDistance, 0);
        Vector3 Oleft = new Vector3(0, offsetDistance, 0);
        Vector3 Oup = new Vector3(-offsetDistance, 0, 0);


        //TODO: Change Movedirection based on the status of the four raycasts (add clockwise/widdershins support)
        RaycastHit2D downHit1 = Physics2D.Raycast(transform.position + Odown, Vector2.down, groundCheckDist, GroundLayer);
        RaycastHit2D downHit2 = Physics2D.Raycast(transform.position - Odown, Vector2.down, groundCheckDist, GroundLayer);

        RaycastHit2D rightHit1 = Physics2D.Raycast(transform.position + Oright, Vector2.right, groundCheckDist, GroundLayer);
        RaycastHit2D rightHit2 = Physics2D.Raycast(transform.position - Oright, Vector2.right, groundCheckDist, GroundLayer);

        RaycastHit2D leftHit1 = Physics2D.Raycast(transform.position + Oleft, Vector2.left, groundCheckDist, GroundLayer);
        RaycastHit2D leftHit2 = Physics2D.Raycast(transform.position - Oleft, Vector2.left, groundCheckDist, GroundLayer);

        RaycastHit2D upHit1 = Physics2D.Raycast(transform.position + Oup, Vector2.up, groundCheckDist, GroundLayer);
        RaycastHit2D upHit2 = Physics2D.Raycast(transform.position - Oup, Vector2.up, groundCheckDist, GroundLayer);

       // Debug.DrawLine(transform.position, transform.position + rayDirection * rayLength, .collider ? Color.green : Color.red);


        if(downHit1.collider != null || downHit2.collider != null)
        {
            //Debug.Log("DOWNN");
            rb.velocity = new Vector2(-moveSpeed, 0);
        }
        else if (rightHit1.collider != null || rightHit2.collider != null)
        {
           // Debug.Log("AHHH");
            rb.velocity = new Vector2(0, -moveSpeed);
        }
        else if (upHit1.collider != null || upHit2.collider != null)
        {
            rb.velocity = new Vector2(moveSpeed, 0);
        }
        else if (leftHit1.collider != null || leftHit2.collider != null)
        {
            rb.velocity = new Vector2(0, moveSpeed);
        }

    }


    #endregion


    //Other stuff
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

}

