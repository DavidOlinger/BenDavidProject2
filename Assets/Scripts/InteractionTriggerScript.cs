using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTriggerScript : MonoBehaviour
{
    public bool activated;
    public float activationRadius;
    public float activationCooldown;
    public float cooldownCounter;
    GameObject player;
    PlayerScript ps;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ps = player.GetComponent<PlayerScript>();
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
            if (ps.isInteracting)
            {
                if (cooldownCounter >= activationCooldown && !activated)
                {
                    activated = true;
                    Debug.Log("Altar Activated");
                    Invoke("Deactivate", 0.05f);
                    cooldownCounter = 0;
                }

            }
        }
    }

    private void Deactivate()
    {
        activated=false;
        Debug.Log("Altar Deactivated");
    }

}
