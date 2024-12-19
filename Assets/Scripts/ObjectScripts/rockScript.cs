using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rockScript : MonoBehaviour
{

    private Rigidbody2D rb;
    public Vector2 slashPushSpeed;
    public Vector2 vaultPushSpeed;

    private int launchDirection = 1;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();

    }

    private void FixedUpdate()
    {
        if(rb.velocity.x > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x - 0.08f, rb.velocity.y);
        }
        else if (rb.velocity.x < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x + 0.08f, rb.velocity.y);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Slash"))
        {
            if(collision.transform.position.x - gameObject.transform.position.x <= 0)
            {
                launchDirection = 1;
            }
            else
            {
                launchDirection = -1;
            }
            rb.AddForce(slashPushSpeed * launchDirection, ForceMode2D.Impulse) ;
        }
        else if (collision.CompareTag("Vault"))
        {
            if (collision.transform.position.x - gameObject.transform.position.x <= 0)
            {
                launchDirection = 1;
            }
            else
            {
                launchDirection = -1;
            }
            vaultPushSpeed.x = vaultPushSpeed.x * launchDirection;
            rb.AddForce(vaultPushSpeed, ForceMode2D.Impulse);
        }
    }
}
