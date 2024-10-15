using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{


    public float hpMax;

    public float hitCounter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slash"))
        {
            Debug.Log("hit detected");
            hitCounter++;
            if(hitCounter >= hpMax)
            {
                Destroy(gameObject);
            }
        }
    }

}
