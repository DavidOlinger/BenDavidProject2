using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyParticleScript : MonoBehaviour
{
    public ParticleSystem mps; // Reference to the particle system
    AudioSource audioSource;
    [SerializeField] AudioClip pickupSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (mps == null)
        {
            mps = GetComponent<ParticleSystem>();
        }
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.GetComponent<PlayerScript>() != null)
        {
            
            // Get the particles as an array
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[mps.main.maxParticles];
            int numParticlesAlive = mps.GetParticles(particles);
            // Iterate through the particles
            for (int i = 0; i < numParticlesAlive; i++)
            {
                // Check if the particle is near the collision point
                Vector3 particlePosition = particles[i].position;
                if (Vector2.Distance( (Vector2)(particlePosition), (Vector2)other.transform.position) < other.GetComponent<PlayerScript>().pickupRadius) // Threshhold for how close the particle has to be.
                {
                    //Debug.Log("Particle hit the player at position: " + particlePosition);
                    particles[i].remainingLifetime = 0;
                    audioSource.PlayOneShot(pickupSound);
                    Debug.Log("got money");
                }
            }

            // Apply the changes back to the particle system
            mps.SetParticles(particles, numParticlesAlive);
        }
    }
}
