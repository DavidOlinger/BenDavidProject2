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

    public string objectID;
    public bool neverRespawn;

    AudioSource audioSource;
    [SerializeField] AudioClip doorOpenSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        LogicScript logic = FindObjectOfType<LogicScript>();

        if (logic != null && logic.IsBreakableBroken(objectID))
        {
            Destroy(gameObject);
        }

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
        Invoke("now", 1.3f);
        LogicScript logic = FindObjectOfType<LogicScript>();
        if (logic != null)
        {
            Debug.Log("permaDeleted");
            logic.MarkBreakableAsBroken(objectID, neverRespawn);
        }
    }

    void now()
    {
        Destroy(gameObject);
    }

    public void OpenDoor()
    {
        isOpening = true;
        audioSource.PlayOneShot(doorOpenSound);
    }
}
