using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultScript : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    SpriteRenderer sp;
    Animator animator;
    public float animationLength;
    private bool hasVaulted;
    public Vector3 vaultPosition;

    PlayerScript playerScript;

    void Start()
    {
        hasVaulted = false;
        


        Transform parentTransform = transform.parent;
        player = parentTransform.gameObject;
        playerScript = player.GetComponent<PlayerScript>();


        // Get the Animator component
        animator = GetComponent<Animator>();

        // Get the length of the current animation clip
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        animationLength = clipInfo[0].clip.length;

        // Destroy the object after the animation finishes
        Destroy(gameObject, animationLength);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (!hasVaulted) {
    //        playerScript.VaultLaunch(); 
    //        hasVaulted = true; 
    //    }
        
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasVaulted)  
        { 
            playerScript.VaultLaunch(); 
            hasVaulted = true;

            if (collision.gameObject.GetComponent<SpikeScript>() != null || collision.gameObject.GetComponent<EnemyScript>() != null) //only resets the cooldown if vault hits an enemy or spikes.
            {
                playerScript.vaultCooldown = 0;
            }
            
        }
    }

    public void DisableCollider()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }
    public void DestroySelf() { Destroy(gameObject); }
}
