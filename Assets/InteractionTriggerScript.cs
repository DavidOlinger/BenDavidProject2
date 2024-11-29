using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTriggerScript : MonoBehaviour
{
    public bool activated;
    public float activationCooldown;
    private float cooldownCounter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldownCounter < activationCooldown)
        {
            cooldownCounter += Time.deltaTime;
        }

        if (activated)
        {
            //Activate whatever by checking this "activated" variable in another script
            activated = false;
            cooldownCounter = 0;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")){
            PlayerScript ps = collision.gameObject.GetComponent<PlayerScript>();
            if (ps.isInteracting)
            {
                if (cooldownCounter >= activationCooldown)
                {
                    activated = true;
                }
                
            }
        }
    }
}
