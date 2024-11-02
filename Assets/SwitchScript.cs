using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public GameObject door;
    DoorScript scriptOfDoor;

    // Start is called before the first frame update
    void Start()
    {
        scriptOfDoor = door.GetComponent<DoorScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Slash") || collision.gameObject.CompareTag("Vault"))
        {
            if(door != null)
            {
                scriptOfDoor.OpenDoor();
            }
        }
    }
}
