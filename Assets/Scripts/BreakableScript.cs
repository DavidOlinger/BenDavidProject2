using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableScript : MonoBehaviour
{
    public int hitPoints;
    public ParticleSystem hitParticles;
    public ParticleSystem breakParticles;
    public ParticleSystem moneyParticles;
    public ParticleSystem ambientParticles;
    private ParticleSystem ambientParticleSystem;
    public GameObject shadow;
    public GameObject[] childSprites;


    public float shakeAmplitude;
    public float shakeFrequency;
    public float shakeDuration;
    public int moneyOnBreak;
    public int moneyOnHit;
    AudioSource audioSource;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip breakSound;

    public bool neverRespawn; // if its a one time breakable
    //private bool isBroken;
    public string objectID;

    public bool needsHeavySlash;

    private LogicScript logicScript;

    void Start()
    {


        audioSource = GetComponent<AudioSource>();
        if (ambientParticles != null)
        {
            ambientParticleSystem = Instantiate(ambientParticles, transform.position, Quaternion.identity);
        }
        //TODO: if this wall has been broken (save in player prefs): Delete it.

        LogicScript logicScript = GameObject.FindWithTag("TimeManager").GetComponent<LogicScript>();


        if (logicScript != null && logicScript.IsBreakableBroken(objectID))
        {
            if (ambientParticleSystem != null)
            {
                ambientParticleSystem.Stop();
                Destroy(ambientParticleSystem.gameObject, ambientParticleSystem.main.startLifetime.constantMax); //lets all the particles die before it kills the system
            }
            Destroy(gameObject);
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("Slash") || collision.gameObject.CompareTag("Vault") || collision.gameObject.CompareTag("HeavySlash")) && !needsHeavySlash)
        {
            if (PlayerPrefs.GetInt("Boon1") == 1)
            {
                hitPoints = hitPoints - 5;
            }
            else
            {
                hitPoints--;
            }
           // Debug.Log("wall hit");
            if (hitPoints > 0)
            {
                hitParticles.Play();
                audioSource.PlayOneShot(hitSound);
                StartCoroutine(ShakeMyselfCoroutine());
                if (moneyParticles != null)
                {
                    ParticleSystem moneyDropParticles = Instantiate(moneyParticles, transform.position, Quaternion.identity);
                    moneyDropParticles.GetComponent<MoneyParticleScript>().SpawnMoney(moneyOnHit);
                }
            } else
            { 
                //Debug.Log("wall broken");
                audioSource.PlayOneShot(hitSound);
                BreakMyself();
            }
        }

        if(collision.gameObject.CompareTag("HeavySlash") && needsHeavySlash)
        {
            hitPoints--;
            // Debug.Log("wall hit");
            if (hitPoints > 0)
            {
                hitParticles.Play();
                audioSource.PlayOneShot(hitSound);
                StartCoroutine(ShakeMyselfCoroutine());
                if (moneyParticles != null)
                {
                    ParticleSystem moneyDropParticles = Instantiate(moneyParticles, transform.position, Quaternion.identity);
                    moneyDropParticles.GetComponent<MoneyParticleScript>().SpawnMoney(moneyOnHit);
                }
            }
            else
            {
                //Debug.Log("wall broken");
                audioSource.PlayOneShot(hitSound);
                BreakMyself();
            }
        }
    }

    private void BreakMyself()
    {

        LogicScript logic = FindObjectOfType<LogicScript>();
        if (logic != null)
        {
            Debug.Log("permaDeleted");
            logic.MarkBreakableAsBroken(objectID, neverRespawn);
        }


        audioSource.PlayOneShot(breakSound);

        breakParticles.Play();
        if (moneyParticles != null)
        {
            ParticleSystem moneyDropParticles = Instantiate(moneyParticles, transform.position, Quaternion.identity);
            moneyDropParticles.GetComponent<MoneyParticleScript>().SpawnMoney(moneyOnBreak);
        }
        if (shadow != null)
        {
            Destroy(shadow);
        }

        if(childSprites != null)
        {
            foreach (GameObject child in childSprites)
            {
                if (child != null) // Check if the child object is not null
                {
                    Destroy(child); // Destroy the child object
                }
            }
        }
      

        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        if (ambientParticleSystem != null)
        {
            ambientParticleSystem.Stop();
            Destroy(ambientParticleSystem.gameObject, ambientParticleSystem.main.startLifetime.constantMax); //lets all the particles die before it kills the system
        }

        

        Destroy(gameObject, 5f);

    }

    IEnumerator ShakeMyselfCoroutine()
    {
        Vector3 startPosition = transform.position; //save the initial position of the thing

        float shakeTimer = 0;
        float xAmp = shakeAmplitude; //max distance from startPosition the object can teleport to.
        float yAmp = shakeAmplitude;
        while (shakeTimer < shakeDuration)
        {
            yield return new WaitForSecondsRealtime(1f / shakeFrequency); //run loop frequency times per second
            shakeTimer += 1f / shakeFrequency; //count up time
            transform.position = startPosition + new Vector3(Random.Range(-xAmp, xAmp), Random.Range(-yAmp, yAmp), 0); //teleport to random position within bounds
            xAmp *= 0.9f; //decay bounds
            yAmp *= 0.9f;
        }
        transform.position = startPosition;
    }
}
