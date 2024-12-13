using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EyeBossScript : MonoBehaviour
{

    // Attacks
    public ParticleSystem fireBeam;
    public float sweepTime;
    public GameObject fireballPrefab;
    public float fireballXVel;
    public float fireballYVel;


    //Movement
    public float pullForce; //the strength of the force that pulls the player to the cursor
    public float maxDistance = 10f;


    private Vector3 velocity = Vector3.zero;
    public float moveSpeed = 5f;
    public float smoothTime = 0.3f; 
    [SerializeField] GameObject targetTL;
    [SerializeField] GameObject targetTR;
    [SerializeField] GameObject targetBL;
    [SerializeField] GameObject targetBR;
    [SerializeField] GameObject targetCenter;

    private GameObject currentTarget;




    // Components
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentTarget = targetCenter;
        StartCoroutine(BeamSweep());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveToTarget(currentTarget);
    }

    private void MoveToTarget(GameObject target)
    {
        Vector3 targetLocation = target.transform.position;

        // Use SmoothDamp for gradual, smooth movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetLocation,
            ref velocity,
            smoothTime,
            moveSpeed
        );
    }

    private IEnumerator MoveInCrossPattern()
    {
        currentTarget = targetCenter;
        yield return new WaitForSeconds(2f);
        currentTarget = targetBL;
        yield return new WaitForSeconds(2f);
        currentTarget = targetTL;
        yield return new WaitForSeconds(2f);
        currentTarget = targetBR;
        yield return new WaitForSeconds(2f);
        currentTarget = targetTR;
        yield return new WaitForSeconds(2f);
        currentTarget = targetCenter;
        StartCoroutine(Barrage(20, 0.3f));
    }


    private IEnumerator BeamSweep()
    {
        fireBeam.Play();
        fireBeam.GetComponent<BoxCollider2D>().enabled = true;
        
        Quaternion startRotation = fireBeam.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, 180);

        float elapsedTime = 0f;
        while (elapsedTime < sweepTime)
        {
            float t = elapsedTime / sweepTime;
            fireBeam.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fireBeam.transform.rotation = targetRotation;
        fireBeam.Stop();
        fireBeam.GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(MoveInCrossPattern());
    }

    private IEnumerator Barrage(int numFireballs, float delay)
    {
        for (int i = 0; i < numFireballs; i++)
        {
            yield return new WaitForSeconds(delay);
            GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(
                Random.Range(-fireballXVel, fireballXVel),
                fireballYVel
            );
        }
    }
}
