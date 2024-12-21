using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    private InteractionTriggerScript interactScript;

    // Array to hold all HoverTextScript components from the children
    public HoverTextScript[] npcDialogue;




    // Variable to determine if the NPC is currently in a dialogue
    [SerializeField] private bool isTalking = false;




    // Index to track which HoverTextScript we are currently interacting with
    [SerializeField] private int currentDialogueIndex = 0;




    void Start()
    {
        interactScript = gameObject.GetComponent<InteractionTriggerScript>();

        int childCount = transform.childCount;
        npcDialogue = new HoverTextScript[childCount];

        for (int i = 0; i < childCount; i++)
        {
            HoverTextScript hoverText = transform.GetChild(i).GetComponent<HoverTextScript>();
            if (hoverText != null)
            {
                npcDialogue[i] = hoverText;
                hoverText.notInteracting = true; 
            }
        }


    }





    void Update()
    {
        if (interactScript.activated && !isTalking)
        {
            Debug.Log("talk STARTED");
            isTalking = true;
            interactScript.freezePlayer();
            StartCoroutine(startDialogue());
        }
    }




    // Coroutine to handle dialogue progression
    public IEnumerator startDialogue()
    {
        currentDialogueIndex = 0;
        interactScript.activated = false;
        interactScript.stopPlayerInteracting();



        while (isTalking)
        {

            if (npcDialogue.Length == 0) // Exit if no dialogues are present
            {
                isTalking = false;
                break;
            }



            npcDialogue[currentDialogueIndex].notInteracting = false;


            // Wait for the user to press "E"
            if (interactScript.activated)
            {
                interactScript.activated = false;
                interactScript.stopPlayerInteracting();
               


                npcDialogue[currentDialogueIndex].notInteracting = true;

                currentDialogueIndex++;

                



                // If we've gone through all dialogues, exit the loop
                if (currentDialogueIndex >= npcDialogue.Length)
                {
                    isTalking = false;
                }
                else
                {
                    // Set the next HoverTextScript's notInteracting to false
                    npcDialogue[currentDialogueIndex].notInteracting = false;

                }
            }

            interactScript.activated = false;


            yield return null; // Wait until the next frame
        }


        interactScript.activated = false;




        // Exit dialogue and unfreeze the player
        interactScript.unFreezePlayer();



        yield return null;
    }









}
