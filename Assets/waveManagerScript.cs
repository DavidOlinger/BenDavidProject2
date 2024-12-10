using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveManagerScript : MonoBehaviour
{
    public GameObject[] enemies; 
    public GameObject[] doors; 

    private bool isTriggered = false; // so the trigger works only once


    public bool isCinematic;

    private void Start()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.activeInHierarchy)
            {
                Destroy(gameObject);
                isTriggered = true;
            }
            enemy.GetComponent<MonsterLogicScript>().cantMove = true;
        }


        //if (doors == null || doors.Length == 0) // NEED BETTER CHECK HERE
        //{
        //    Debug.Log("No doors remaining. Destroying WaveManager.");
        //    Destroy(gameObject); // Destroy the WaveManager if no doors are left
        //    isTriggered = true;
        //}
        
        
    }

    private void endCantMove()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerScript>().cantMove = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            

            if (isTriggered) return; // Avoid retriggering
            isTriggered = true;


            if (isCinematic)
            {
                PlayerScript pscript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
                pscript.cantMove = true;
                pscript.rb.velocity = Vector2.zero;
                
                Invoke("endCantMove", 1.7f);

                //could also change the music here by muting the main music and playing the music attached to the waveManager
            }

            foreach (var enemy in enemies)
            {
                if (enemy.TryGetComponent(out MonsterLogicScript script))
                {
                    script.cantMove = true;
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

    private IEnumerator CheckEnemies()
    {
        while(true)
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
