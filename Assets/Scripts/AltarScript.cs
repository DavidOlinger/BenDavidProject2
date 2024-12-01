using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarScript : MonoBehaviour
{
    private GameObject player;
    private AudioSource audioSource;
    [SerializeField] ParticleSystem interactionParticles;
    [SerializeField] AudioClip interactionSound;
    [SerializeField] GameObject interactionTrigger;
    InteractionTriggerScript interactionScript;

    public bool isActivatable;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        interactionScript = interactionTrigger.GetComponent<InteractionTriggerScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        isActivatable = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (interactionScript != null)
        {
            if (interactionScript.activated && isActivatable)
            {
                //kinda just here as an example, put whatever here
                ParticleSystem particles = Instantiate(interactionParticles, player.transform.position, Quaternion.identity);
                audioSource.PlayOneShot(interactionSound);
                Destroy(particles, particles.main.startLifetime.constantMax);
                isActivatable = false;
            }
            if (!interactionScript.activated) 
            {
                isActivatable = true;
            }
            
        }
    }
}
