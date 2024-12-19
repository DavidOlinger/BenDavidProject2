using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangScript : MonoBehaviour
{
    private float timeAlive = 0;

    public float timeToReturn;
    public float Speed;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    public bool isFacingRight;

    // Update is called once per frame
    void Update()
    {
        if(timeAlive < timeToReturn)
        {
            if (isFacingRight)
            {
                rb.velocity = new Vector2(Speed, 0);
            }
            else
            {
                rb.velocity = new Vector2(-Speed, 0);
            }
        }
        else
        {
            if (isFacingRight)
            {
                rb.velocity = new Vector2(-Speed, 0);
            }
            else
            {
                rb.velocity = new Vector2(Speed, 0);
            }
        }


        if(timeAlive > (timeToReturn * 2))
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        timeAlive += 0.02f;
    }

    

}
