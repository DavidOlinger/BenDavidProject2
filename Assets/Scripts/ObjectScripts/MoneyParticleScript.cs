using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyParticleScript : MonoBehaviour
{
    public ParticleSystem mps; // Reference to the particle system
    LogicScript logic;
    AudioSource audioSource;
    [SerializeField] AudioClip pickupSound;

    void Start()
    {
        logic = FindObjectOfType<LogicScript>();
        audioSource = GetComponent<AudioSource>();
        if (mps == null)
        {
            mps = GetComponent<ParticleSystem>();
        }

    }

    public void SpawnMoney(int amount) //spawn [amount] particles and then destroy myself
    {
        var emission = mps.emission;
        var burst = emission.GetBurst(0);
        burst.count = amount;
        emission.SetBurst(0, burst);
        mps.Play();

        Destroy(mps.gameObject, 30f);
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
                    logic.addMoney(1);
                }
            }

            // Apply the changes back to the particle system
            mps.SetParticles(particles, numParticlesAlive);
        }
    }
}
