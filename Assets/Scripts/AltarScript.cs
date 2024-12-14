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


    public int AlterNum;

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

        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayerPrefs.SetInt("Bless1", 0);
            PlayerPrefs.SetInt("Bless2", 0);
            PlayerPrefs.SetInt("Boon1", 0);
            PlayerPrefs.SetInt("Boon2", 0);
        }

        //if (PlayerPrefs.GetInt("Bless1") == 1)
        //{
        //    Debug.Log("BOOMERANG");
        //}
        //if (PlayerPrefs.GetInt("Bless2") == 1)
        //{
        //    Debug.Log("LUCKY");
        //}

        //if (PlayerPrefs.GetInt("Boon1") == 1)
        //{
        //    Debug.Log("DESTRUCTION");
        //}
        //if (PlayerPrefs.GetInt("Boon2") == 1)
        //{
        //    Debug.Log("SPEED");
        //}



        if (interactionScript != null)
        {
            if (interactionScript.activated && isActivatable)
            {
                //kinda just here as an example, put whatever here
                ParticleSystem particles = Instantiate(interactionParticles, player.transform.position, Quaternion.identity);
                audioSource.PlayOneShot(interactionSound);
                Destroy(particles, particles.main.startLifetime.constantMax);
                isActivatable = false;

                if(AlterNum == 1)
                {
                    PlayerPrefs.SetInt("Bless1", 1);

                }
                else if(AlterNum == 2)
                {
                    PlayerPrefs.SetInt("Bless2", 1);
                }
            }
            if (!interactionScript.activated) 
            {
                isActivatable = true;
            }
            
        }
    }
}
