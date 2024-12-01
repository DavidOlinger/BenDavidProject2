using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashScript : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    GameObject cam;
    SpriteRenderer sp;
    Animator animator;
    public float animationLength;
    public Vector3 slashPosition;
    public ParticleSystem sparkParticle;
    public ParticleSystem flareParticle;



    void Start()
    {
        Transform parentTransform = transform.parent;
        player = parentTransform.gameObject;
        sp = gameObject.GetComponent<SpriteRenderer>();
        cam = player.GetComponent<PlayerScript>().cam;
    }
    public void DisableCollider()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }
    public void DestroySelf() {
        player.GetComponent<PlayerScript>().StartChargingSlash();
        Destroy(gameObject); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Breakable"))
        {
            //Debug.Log("hitparticle collided");
            HitParticles(collision.gameObject);
        }
    }

    public void HitParticles(GameObject target)
    {
        //Debug.Log("hitparticle spawned");

        //Vector3 midpoint = new Vector3((transform.position.x + target.transform.position.x) / 2f,
        //        (transform.position.y + target.transform.position.y) / 2f, transform.position.z);

        // Calculate direction vector from player to target
        Vector2 direction = new Vector2(
            target.transform.position.x - player.transform.position.x,
            target.transform.position.y - player.transform.position.y
        );

        // Get the angle between player and target in degrees
        float angleToTarget = Random.Range(-10f, 10f) + 180f - Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //Debug.Log("angle to target: " + angleToTarget);

        
        ParticleSystem sparks1 = Instantiate(sparkParticle, target.transform.position, Quaternion.Euler(180f, 0f, angleToTarget - 90f));
        ParticleSystem sparks2 = Instantiate(sparkParticle, target.transform.position, Quaternion.Euler(180f, 0f, angleToTarget + 90f));
        ParticleSystem flare = Instantiate(flareParticle, target.transform.position, Quaternion.Euler(0f, 0f, 0f));

        //Debug.Log("particle system rotation x:" + sparks1.transform.rotation.x + " y:" + sparks1.transform.rotation.y + " z:" + sparks1.transform.rotation.z);
        flare.Stop();
        sparks1.Stop();
        sparks2.Stop();
        sparks1.Clear();
        sparks2.Clear();
        flare.Clear();
        sparks1.Play();
        sparks2.Play();
        flare.Play();
        Destroy(sparks1.gameObject, 0.5f);
        Destroy(sparks2.gameObject, 0.5f);
        Destroy(flare.gameObject, 0.5f);
    }

}
