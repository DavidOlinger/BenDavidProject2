using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningEffectScript : MonoBehaviour
{
    public float branchingProbability = 0.3f;
    public float minBranchDelay = 0.1f;
    public float maxBranchDelay = 0.8f;

    private ParticleSystem particleSystem1;
    private ParticleSystem.Particle[] particles;
    private ParticleSystem subEmitter;
    // Start is called before the first frame update
    void Start()
    {
        particleSystem1 = GetComponent<ParticleSystem>();

        // Get the first sub-emitter from the particle system
        var subEmitters = particleSystem1.subEmitters;
        if (subEmitters.subEmittersCount > 0)
        {
            subEmitter = subEmitters.GetSubEmitterSystem(0);
        }

        // Initialize particle array
        particles = new ParticleSystem.Particle[particleSystem1.main.maxParticles];

    }

    // Update is called once per frame
    void Update()
    {
        // Get all currently active particles
        int numParticlesAlive = particleSystem1.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            var particle = particles[i];

            // Calculate how far through its lifetime the particle is (0 to 1)
            float normalizedLifetime = 1f - (particle.remainingLifetime / particle.startLifetime);

            // Generate a random value to compare against branching probability
            float random = Random.Range(0f, 1f);

            // Check if we should create a branch at this point
            if (normalizedLifetime >= minBranchDelay &&
                normalizedLifetime <= maxBranchDelay &&
                random < branchingProbability)
            {
                // Emit a new particle from the sub-emitter at the current particle's position
                if (subEmitter != null)
                {
                    var emission = new ParticleSystem.EmitParams
                    {
                        position = particle.position,
                        velocity = Quaternion.Euler(0, 0, Random.Range(-45f, 45f)) * particle.velocity,
                        startLifetime = particle.remainingLifetime
                    };

                    subEmitter.Emit(emission, 1);

                    // Prevent this particle from branching again
                    particle.remainingLifetime = 0;
                }
            }

            particles[i] = particle;
        }

        // Apply the modified particles back to the system
        particleSystem1.SetParticles(particles, numParticlesAlive);
    }
}

