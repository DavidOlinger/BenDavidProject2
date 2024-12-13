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
    public ParticleSystem dashParticles;
    public float dashSpeed;



    //Movement
    public float pullForce; //the strength of the force that pulls the player to the cursor
    public float maxDistance = 10f;
    public float expectedMoveTime = 2f;

    //Health
    public float awakenDistance = 8;
    public bool isAlive;
    public bool isAwakened;
    public int hp = 20;
    public bool isInvincible;
    public float invincibleTime = 0.1f;
    [SerializeField] ParticleSystem awakenRoarParticles;
    [SerializeField] ParticleSystem hitParticles;
    [SerializeField] ParticleSystem deathAuraParticles;
    [SerializeField] ParticleSystem deathLightningParticles;


    private Vector3 velocity = Vector3.zero;
    public float moveSpeed = 5f;
    public float smoothTime = 0.3f; 
    [SerializeField] GameObject targetTL;
    [SerializeField] GameObject targetTR;
    [SerializeField] GameObject targetBL;
    [SerializeField] GameObject targetBR;
    [SerializeField] GameObject targetCenter;
    [SerializeField] GameObject targetTopCenter;


    private GameObject currentTarget;
    private Coroutine mainCR;
    public GameObject player;
    public GameObject myEye;

 


    // Components
    Rigidbody2D rb;
    SpriteRenderer sp;
    SpriteRenderer spEye;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        spEye = myEye.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector2.Distance(player.transform.position, transform.position) < awakenDistance && !isAwakened)
        {
            isAwakened = true;
            StartCoroutine(AwakenSequence());
        }
        if (isAwakened)
        {
            MoveToTarget(currentTarget);
        }
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slash") || collision.CompareTag("Vault"))
        {
            if (!isInvincible)
            {
                if (hp > 0)
                {
                    hp--;
                    isInvincible = true;
                    sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 0.8f);
                    spEye.color = new Color(sp.color.r, sp.color.g, sp.color.b, 0.8f);
                    Invoke("endInvincible", invincibleTime);
                    Destroy(Instantiate(hitParticles, transform.position, Quaternion.identity), 2);
                } else
                {
                    Die();
                }
                
            }
        }

    }

    private void endInvincible()
    {
        isInvincible = false;
        sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 1f);
        spEye.color = new Color(sp.color.r, sp.color.g, sp.color.b, 1f);


    }

    public void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DeathSequence());
    }


    private IEnumerator MainLoop()
    {
        
        yield return null;
        while (isAlive)
        {
            //cw down beam sweep
            currentTarget = targetCenter;
            yield return new WaitForSeconds(expectedMoveTime);
            StartCoroutine(BeamSweep(-180));
            yield return new WaitForSeconds(sweepTime+1);


            StartCoroutine(DarkDash(targetBL, targetBR));
            yield return new WaitForSeconds(2.5f * expectedMoveTime);



            //ccw down beam sweep
            currentTarget = targetCenter;
            yield return new WaitForSeconds(expectedMoveTime);
            StartCoroutine(BeamSweep(180));
            yield return new WaitForSeconds(sweepTime + 1);

            currentTarget = targetTopCenter;
            yield return new WaitForSeconds(expectedMoveTime);

            //15 ball barrage
            StartCoroutine(Barrage(15, 0.33f));
            yield return new WaitForSeconds(6);

            StartCoroutine(DarkDash(targetBR, targetBL));
            yield return new WaitForSeconds(2.5f*expectedMoveTime);



        }

    }
    private IEnumerator DeathSequence()
    {
        currentTarget = targetCenter;
        yield return new WaitForSeconds(expectedMoveTime);
        //any death dialogue or particles
        yield return new WaitForSeconds(5);
        //die
        yield return new WaitForSeconds(1);
        Destroy(gameObject);

    }

    private IEnumerator AwakenSequence()
    {
        isInvincible = true;
        currentTarget = targetCenter;
        yield return new WaitForSeconds(expectedMoveTime);
        //any opening dialogue or particles
        Destroy(Instantiate(awakenRoarParticles, transform.position, Quaternion.identity), 5);
        yield return new WaitForSeconds(5);
        isInvincible = false;
        StartCoroutine(MainLoop());
    }



    private IEnumerator DarkDash(GameObject target1, GameObject target2)
    {
        float savedMoveSpeed = moveSpeed;
        float savedSmoothTime = smoothTime;
        currentTarget = target1;
        yield return new WaitForSeconds(1.5f*expectedMoveTime);
        ParticleSystem warnPart = Instantiate(dashParticles, transform.position + new Vector3(0, 0.7f, 0), Quaternion.identity);
        Destroy(warnPart, 1);
        yield return new WaitForSeconds(0.2f);

        moveSpeed = dashSpeed;
        smoothTime = 0.01f;
        currentTarget = target2;

        ParticleSystem dashPart = Instantiate(dashParticles, transform.position + new Vector3(0, 0.7f, 0), Quaternion.identity);
        dashPart.transform.parent = transform;
        Destroy(dashPart, 1);

        yield return new WaitForSeconds(expectedMoveTime / 2);
        moveSpeed = savedMoveSpeed;
        smoothTime = savedSmoothTime;
    }

    private IEnumerator BeamSweep(float angle)
    {
        fireBeam.Play();
        fireBeam.GetComponent<BoxCollider2D>().enabled = true;
        
        Quaternion startRotation = fireBeam.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, angle);

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
