using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveManagerScript : MonoBehaviour
{
    public GameObject[] enemies; 
    public GameObject[] doors; 

    private bool isTriggered = false; // so the trigger works only once


    public bool isCinematic;

    private PlayerScript pScript;

    private void Start()
    {
        pScript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();


        foreach (var enemy in enemies)
        {
            if (!enemy.activeInHierarchy)
            {
                Destroy(gameObject);
                isTriggered = true;
            }
            enemy.GetComponent<MonsterLogicScript>().cantMove = true;
           // enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            //enemy.GetComponent<SpriteRenderer>().enabled = false;
        }


        
    }

   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            

            if (isTriggered) return; // Avoid retriggering
            isTriggered = true;

            //not using for now
            if (isCinematic)
            {
                PlayerScript pscript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
                pscript.cantMove = true;
                pscript.rb.velocity = Vector2.zero;
                
                Invoke("endCantMove", 2f);

                //could also change the music here by muting the main music and playing the music attached to the waveManager
            }




            foreach (var enemy in enemies)
            {
                if (enemy.TryGetComponent(out MonsterLogicScript script))
                {
                    script.cantMove = true;
                    //  script.waveStart();
                    if (!isCinematic)
                    {
                        script.waveStart();
                    }


                }
            }


            // Close all doors by calling their respective CloseDoor method
            foreach (var door in doors)
            {
                if (door.TryGetComponent(out DoorScript script))
                {
                    script.CloseDoor();
                }
            }

            // Start monitoring enemy states
            StartCoroutine(CheckEnemies());
        }
    }

    private void endCantMove()
    {
        Debug.Log("endingCantMove");
        GameObject.FindWithTag("Player").GetComponent<PlayerScript>().cantMove = false;


        foreach (var enemy in enemies)
        {
            if (enemy.TryGetComponent(out MonsterLogicScript script))
            {
                script.waveStart();


            }
        }

    }

    private IEnumerator CheckEnemies()
    {
        while(true && pScript.currHP >= 1)
        {


            bool allEnemiesDefeated = true;

            foreach (var enemy in enemies)
            {
                if (enemy.activeInHierarchy) // If any enemy is still active
                {
                    allEnemiesDefeated = false;
                    break;
                }
            }

            if (allEnemiesDefeated)
            {
                break;
            }

            yield return null; // Wait for the next frame
        }

        if(pScript.currHP < 1)
        {
            foreach (var door in doors)
            {
                if (door.TryGetComponent(out DoorScript script))
                {
                    script.OpenDoor();
                }
            }
        }


        // All enemies are defeated; open the doors
        foreach (var door in doors)
        {
            if (door.TryGetComponent(out DoorScript script))
            {
                script.OpenDoor();
                script.DestroyDoor();
            }
        }

        Destroy(gameObject);
    }
}
