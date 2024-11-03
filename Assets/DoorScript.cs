using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    Rigidbody2D rb;
    Vector2 startPos;
    Vector2 deletePos;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        startPos = transform.position;
        deletePos = startPos + new Vector2(0, 4);
    }


    public bool isOpening;
    // Update is called once per frame
    void Update()
    {
        if (isOpening)
        {
            rb.velocity = Vector2.up * 3;
        }

        if (transform.position.y >= deletePos.y)
        {
            Destroy(gameObject); // simple for now lol
        }


    }


    
}
