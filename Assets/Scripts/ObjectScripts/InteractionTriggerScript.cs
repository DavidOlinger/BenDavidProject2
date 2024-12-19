using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTriggerScript : MonoBehaviour
{
    public bool activated;
    public bool renderHighlight;
    public float activationRadius;
    public float activationCooldown;
    public float cooldownCounter;
    GameObject player;
    PlayerScript ps;
    SpriteRenderer sp;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ps = player.GetComponent<PlayerScript>();
        sp = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldownCounter < activationCooldown)
        {
            cooldownCounter += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        float distance = Vector2.Distance(player.transform.position, transform.position);
        if (distance < activationRadius)
        {
            if (renderHighlight)
            {
                sp.enabled = true;
            } else
            {
                sp.enabled = false;
            }

            if (ps.isInteracting)
            {
                if (cooldownCounter >= activationCooldown && !activated)
                {
                    activated = true;
                    Invoke("Deactivate", 0.05f);
                    cooldownCounter = 0;
                }

            }
        } else
        {
            if (sp.enabled)
            {
                sp.enabled = false;
            }
        }
    }

    private void Deactivate()
    {
        activated = false;
        
    }

}
