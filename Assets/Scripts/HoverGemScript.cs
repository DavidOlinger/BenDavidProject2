using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverGemScript : MonoBehaviour
{
    public float hitCooldown;
    public ParticleSystem lightningParticles;
    public ParticleSystem auraParticles;
    public float hoverForce;
    public float hoverDist;
    BoxCollider2D hitbox;
    SpriteRenderer sp;
    Rigidbody2D rb;
    Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = new Vector3(transform.position.x, transform.position.y-hoverDist, transform.position.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y >= startPosition.y) 
        {
            rb.AddForce(new Vector2(0, -hoverForce));
        } else
        {
            rb.AddForce(new Vector2(0, hoverForce));

        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.GetComponent<VaultScript>() != null)
    //    {
    //        hitbox.enabled = false;
    //        sp.color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
    //        lightningParticles.Stop();
    //        auraParticles.Stop();

    //        Invoke("Reenable", hitCooldown);
    //    }
    //}

    //private void Reenable()
    //{
    //    hitbox.enabled = true;
    //    sp.color = new Color(1, 1, 1, 1);
    //    lightningParticles.Play();
    //    auraParticles.Play();

    //}
}
