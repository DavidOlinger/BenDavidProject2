using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public GameObject door;
    DoorScript scriptOfDoor;
    private bool flippedOn;
    AudioSource audioSource;
    [SerializeField] AudioClip onSound;
    [SerializeField] AudioClip offSound;

    SpriteRenderer sp;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        scriptOfDoor = door.GetComponent<DoorScript>();
        sp = gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Slash") || collision.gameObject.CompareTag("Vault"))
        {
            if (flippedOn)
            {
                audioSource.PlayOneShot(offSound);
            } else
            {
                audioSource.PlayOneShot(onSound);
            }
            flippedOn = !flippedOn;

            if(door != null)
            {
                scriptOfDoor.OpenDoor();
                scriptOfDoor.DestroyDoor();
                scriptOfDoor = null;
            }

            sp.flipX = !sp.flipX;

            //float currentRotationZ = transform.eulerAngles.z;
            //float newRotationZ = currentRotationZ * -1;
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, newRotationZ);
            
        }
    }
}
