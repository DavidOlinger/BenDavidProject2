using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballScript : MonoBehaviour
{
    [SerializeField] ParticleSystem flameParticles;
    [SerializeField] ParticleSystem sparkParticles;
    [SerializeField] GameObject hitboxPrefab;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 directionOfMovement = (rb.velocity).normalized;
        float angle = Mathf.Atan2(directionOfMovement.y, directionOfMovement.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90);
        transform.rotation = targetRotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    public void Explode()
    {
        Destroy(Instantiate(flameParticles, transform.position, Quaternion.identity), 2);
        Destroy(Instantiate(sparkParticles, transform.position, Quaternion.identity), 6);
        Destroy(Instantiate(hitboxPrefab, transform.position, Quaternion.identity), 0.2f);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

}
