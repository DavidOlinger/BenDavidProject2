using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    Rigidbody2D rb;

    public bool up;
    public bool down;
    public bool left;
    public bool right;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }


    public bool isOpening;
    // Update is called once per frame
    void Update()
    {
        if (isOpening)
        {
            if (up)
            {
                rb.velocity = Vector2.up * 3;
            }
            else if (down)
            {
                rb.velocity = Vector2.down * 3;
            }
            else if (left)
            {
                rb.velocity = Vector2.left * 3;
            }
            else if (right)
            {
                rb.velocity = Vector2.right * 3;
            }
        }

    }


    public void DestroyDoor()
    {
        Invoke("now", 1.2f);
    }

    void now()
    {
        Destroy(gameObject);
    }
}