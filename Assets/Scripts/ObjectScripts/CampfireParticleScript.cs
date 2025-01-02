using UnityEngine;

public class CampfireParticleScript : MonoBehaviour
{
    [SerializeField] private PlayerScript player;
    [SerializeField] private GameObject interaction;

    public bool isLit;
    public string id;

    [SerializeField] private ParticleSystem flames;
    [SerializeField] private ParticleSystem smoke;
    [SerializeField] private ParticleSystem sparks;

    public InteractionTriggerScript interactionTrigger;

    void Start()
    {
        // Add null checks before accessing components
        if (interaction == null)
        {
            Debug.LogError($"Interaction object not assigned for campfire {gameObject.name}");
            return;
        }

        
        interactionTrigger = interaction.GetComponent<InteractionTriggerScript>();

        if (interactionTrigger == null)
        {
            Debug.LogError($"No InteractionTriggerScript found on interaction object for campfire {gameObject.name}");
        }



        // Find player if not assigned in inspector
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.GetComponent<PlayerScript>();
            }
            else
            {
                Debug.LogError("Player not found in scene!");
            }
        }
    }

    void Update()
    {
        // Add null checks before accessing components
        //if (interactionTrigger == null || player == null) return;

        if (interactionTrigger.activated)
        {
            //if (!isLit)
            //{
                Debug.Log("attempting to light");
                Light();
                player.SetNewCampfire(gameObject);
            //}
        }

        interactionTrigger.renderHighlight = !isLit;
    }

    public void Light()
    {
        // If the fire is already lit, do nothing
       // if (isLit) return;

        
        // Enable emission for all particle systems
        EnableEmission(smoke, true);
        EnableEmission(flames, true);
        EnableEmission(sparks, true);

        isLit = true;
        Debug.Log("LightingFire");

    }

    public void PutOut()
    {
        // If the fire is already out, do nothing
        if (!isLit) return;

        // Disable emission for all particle systems
        EnableEmission(smoke, false);
        EnableEmission(flames, false);
        EnableEmission(sparks, false);

        // Null-safe interaction trigger render highlight
        if (interactionTrigger != null)
        {
            interactionTrigger.renderHighlight = true;
        }
        else
        { //was having lots of issues with this
            Debug.LogWarning("InteractionTrigger is null when trying to put out fire");
        }

        isLit = false;
    }

    private void EnableEmission(ParticleSystem particleSystem, bool enable)
    {
        if (particleSystem == null)
        {
            Debug.LogError("Attempted to modify emission on a null particle system!");
            return;
        }

        var emission = particleSystem.emission;
        emission.enabled = enable;
    }
}