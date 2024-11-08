using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableScript : MonoBehaviour
{
    public int hitPoints;
    public ParticleSystem hitParticles;
    public ParticleSystem breakParticles;
    public GameObject shadow;
    public float shakeAmplitude;
    public float shakeFrequency;
    public float shakeDuration;
    AudioSource audioSource;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip breakSound;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //TODO: if this wall has been broken (save in player prefs): Delete it.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Slash"))
        {
            hitPoints--;
            Debug.Log("wall hit");
            if (hitPoints > 0)
            {
                hitParticles.Play();
                audioSource.PlayOneShot(hitSound);
                StartCoroutine(ShakeMyselfCoroutine());
            } else
            { 
                Debug.Log("wall broken");
                audioSource.PlayOneShot(hitSound);
                BreakMyself();
            }
        }
    }

    private void BreakMyself()
    {
        audioSource.PlayOneShot(breakSound);

        breakParticles.Play();
        if (shadow != null)
        {
            Destroy(shadow);
        }
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 3f);

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
    }
}
