using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launchableScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public Vector2 slashPushSpeed;

    private int launchDirection = 1;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();

    }

    private MonsterLogicScript ms;

    private Coroutine CantMoveCoroutine;


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("HeavySlash"))
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerScript>().PlayHitSound();

            rb.velocity = Vector2.zero; //to make it launch the same every time

            if (collision.transform.position.x - gameObject.transform.position.x <= 0)
            {
                launchDirection = 1;
            }
            else
            {
                launchDirection = -1;
            }
            slashPushSpeed.x = slashPushSpeed.x * launchDirection;


            if (gameObject.CompareTag("Enemy"))
            {
                ms = gameObject.GetComponent<MonsterLogicScript>();

                ms.takeDmg(collision);

                if (CantMoveCoroutine != null)
                {
                    StopCoroutine(CantMoveCoroutine);
                }
                CantMoveCoroutine = StartCoroutine(EndCantMove(launchTime));
            }
            

            rb.AddForce(slashPushSpeed, ForceMode2D.Impulse);
        }
    }

    public float launchTime;

    //private bool isLaunched = false;
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {
    //        if (isLaunched)
    //        {
    //            ms.cantMove = false;
    //            isLaunched = false;
    //        }
    //    }
    //}

    private IEnumerator EndCantMove(float duration) //TODO: Update the other script to use this cantmove value
    {

        ms.cantMove = true;

        yield return new WaitForSeconds(duration);

        ms.cantMove = false;
        //rb.velocity = Vector2.zero;
    }
}
