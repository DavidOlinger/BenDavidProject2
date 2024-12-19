using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{

    #region
    //Basic Variables
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    Animator animator;

    PlayerScript playerScript;
    GameObject player;
    MonsterLogicScript monsterLogic;
    public float activateMoveDistance;

    //publics
    public float moveSpeed;

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
        rb.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        SetMovement();
    }

    #endregion

    //Movement
    #region

    private void CycleMove(bool close)
    {
        if (close)
        {
            Vector2 velocity = player.transform.position- transform.position;
            if (velocity.x < 0)
            {
                sp.flipX = true;
            } else
            {
                sp.flipX= false;
            }
            rb.velocity = velocity.normalized*moveSpeed;
        }
    }

    private void SetMovement()
    {

        if (Mathf.Abs(Vector2.Distance(transform.position, player.transform.position)) < activateMoveDistance)
        {
            CycleMove(true); // true = the player is close
        }
        else
        {
            CycleMove(false); // false = the player is not close, (still activates move cycle, but only does the idle move cycle)
        }

    }
    #endregion



}




