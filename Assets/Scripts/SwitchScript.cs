using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public GameObject door;
    DoorScript scriptOfDoor;

    SpriteRenderer sp;

    // Start is called before the first frame update
    void Start()
    {
        scriptOfDoor = door.GetComponent<DoorScript>();
        sp = gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Slash") || collision.gameObject.CompareTag("Vault"))
        {
            if(door != null)
            {
                scriptOfDoor.isOpening = true;
                scriptOfDoor.DestroyDoor();
            }
            float currentRotationZ = transform.eulerAngles.z;
            float newRotationZ = currentRotationZ * -1;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, newRotationZ);
            
        }
    }
}
