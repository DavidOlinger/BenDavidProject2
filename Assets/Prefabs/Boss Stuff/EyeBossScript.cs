using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EyeBossScript : MonoBehaviour
{
    public GameObject triggerEnemy;
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
    [SerializeField] ParticleSystem deathSuckParticles;
    [SerializeField] ParticleSystem deathBurstParticles;


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
    public AudioClip hitSound;
    public AudioClip wakeSound;
    public AudioClip deathSound;
    public AudioClip chargeSound;
    public AudioClip beamSound;
 


    // Components
    Rigidbody2D rb;
    SpriteRenderer sp;
    SpriteRenderer spEye;
    Animator animator;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        spEye = myEye.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
                    animator.SetTrigger("hurt");
                    audioSource.PlayOneShot(hitSound);
                    if (PlayerPrefs.GetInt("Boon1") == 1)
                    {
                        hp = hp - 2;
                    }
                    else
                    {
                        hp--;
                    }
                    isInvincible = true;
                    sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 0.9f);
                    
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
        


    }

    public void Die()
    {
        StopAllCoroutines();
        Destroy(fireBeam);
        StartCoroutine(DeathSequence());
    }

    public void EyeColorRed()
    {
        spEye.color = new Color(1, 0.4f, 0.4f, 1);
    }

    public void EyeColorBlack()
    {
        spEye.color = new Color(0.5f, 0f, 0.5f, 1);
    }

    public void EyeColorOrange()
    {
        spEye.color = new Color(1, 0.6f, 0.1f, 1);
    }

    public void EyeColorReset()
    {
        spEye.color = new Color(1, 1, 1, 1);
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
            yield return new WaitForSeconds(3.5f * expectedMoveTime);



            //ccw down beam sweep
            currentTarget = targetCenter;
            yield return new WaitForSeconds(expectedMoveTime);
            StartCoroutine(BeamSweep(180));
            yield return new WaitForSeconds(sweepTime + 1);

            currentTarget = targetTopCenter;
            yield return new WaitForSeconds(expectedMoveTime);

            //15 ball barrage
            StartCoroutine(Barrage(25, 0.25f));
            yield return new WaitForSeconds(6.25f);

            StartCoroutine(DarkDash(targetBR, targetBL));
            yield return new WaitForSeconds(2.5f*expectedMoveTime);

        }

    }
    private IEnumerator DeathSequence()
    {
        animator.SetBool("dying", true);
        currentTarget = targetCenter;
        yield return new WaitForSeconds(expectedMoveTime);
        //any death dialogue or particles
        deathSuckParticles.Play();
        audioSource.PlayOneShot(deathSound);
        yield return new WaitForSeconds(4.5f);
        //die
        deathBurstParticles.Play();
        sp.enabled = false;
        spEye.enabled = false;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        triggerEnemy.SetActive(false);

    }

    private IEnumerator AwakenSequence()
    {
        isInvincible = true;
        currentTarget = targetCenter;
        yield return new WaitForSeconds(expectedMoveTime);
        //any opening dialogue or particles
        animator.SetBool("awakened", true);
        yield return new WaitForSeconds(1);
        audioSource.PlayOneShot(wakeSound);
        awakenRoarParticles.Play();
        yield return new WaitForSeconds(4);
        isInvincible = false;
        StartCoroutine(MainLoop());
    }



    private IEnumerator DarkDash(GameObject target1, GameObject target2)
    {
        animator.SetTrigger("triggerCharge");
        yield return new WaitForSeconds(0.5f);
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
        audioSource.PlayOneShot(chargeSound);

        ParticleSystem dashPart = Instantiate(dashParticles, transform.position + new Vector3(0, 0.7f, 0), Quaternion.identity);
        dashPart.transform.parent = transform;
        Destroy(dashPart, 1);

        yield return new WaitForSeconds(expectedMoveTime);
        moveSpeed = savedMoveSpeed;
        smoothTime = savedSmoothTime;
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("triggerEydle");
    }

    private IEnumerator BeamSweep(float angle)
    {
        animator.SetTrigger("triggerBeam");
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(beamSound);
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
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("triggerEydle");
    }

    private IEnumerator Barrage(int numFireballs, float delay)
    {
        animator.SetTrigger("triggerBarrage");
        yield return new WaitForSeconds(0.5f);
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
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("triggerEydle");

    }
}
