using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementScript : MonoBehaviour
{
    private Rigidbody2D camBody; //get references to everything
    public GameObject target;
    public GameObject fadeOutPrefab;
    private Camera cam;

    public float pullForce = 5.0f; //the strength of the force that pulls the player to the cursor

    private Vector2 vectorToTarget;
    private float distanceToTarget;
    private Vector2 directionToTarget;
    public float distanceMod = 16; // how much the distance affect the force applied. total = (force * distance) / distanceMod

    public float yOffset;
    public float xOffset;
    public float playerLookAhead;
    public float groundMargin;
    public float groundLevel;


    // Start is called before the first frame update
    void Start()
    {

        cam = GetComponent<Camera>();
        //puck = GameObject.Find("Puck");
        camBody = GetComponent<Rigidbody2D>(); //finding instances of components


    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Vector3 targetLocation = target.transform.position;
        PlayerScript playerScript = target.GetComponent<PlayerScript>();
        if (playerScript != null)
        {
            xOffset = playerScript.playerLookAhead.x;
            yOffset = playerScript.playerLookAhead.y;

        }

        //get distance to target, get its magnitude and direction
        vectorToTarget = new Vector2(transform.position.x - targetLocation.x - xOffset, transform.position.y - targetLocation.y - yOffset);
        distanceToTarget = Mathf.Abs(vectorToTarget.magnitude);
        directionToTarget = -vectorToTarget.normalized;


        if (cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).y < groundLevel - groundMargin) //prevents camera from travelling below ground
        {
            if (directionToTarget.y < 0)
            {
                directionToTarget.y = 0f;
            }
        }

        camBody.velocity = ((new Vector3(directionToTarget.x, directionToTarget.y, 0f)) * pullForce * distanceToTarget);

       
    }

    public void fadeToBlack()
    {
        FadeObjectScript fade = Instantiate(fadeOutPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<FadeObjectScript>();
        fade.fadeInTime = 0.25f;
        fade.fadeOutTime = 0.1f;
        fade.lingerTime = 0.25f;
    }
}
