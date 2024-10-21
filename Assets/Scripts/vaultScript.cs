using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vaultScript : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    SpriteRenderer sp;
    Animator animator;
    public float animationLength;
    public Vector3 vaultPosition;
    public float vaultWindow;
    private bool hasVaulted;

    PlayerScript playerScript;

    void Start()
    {
        hasVaulted = false;
        


        Transform parentTransform = transform.parent;
        player = parentTransform.gameObject;
        playerScript = player.GetComponent<PlayerScript>();
        sp = gameObject.GetComponent<SpriteRenderer>();
        if (vaultPosition.x > 0)
        {
            sp.flipX = true;
        }
        else if (vaultPosition.y < 0)
        {
            transform.Rotate(new Vector3(0, 0, 90));
        }
        else if (vaultPosition.y > 0)
        {
            transform.Rotate(new Vector3(0, 0, -90));
        }

        // Get the Animator component
        animator = GetComponent<Animator>();

        // Get the length of the current animation clip
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        animationLength = clipInfo[0].clip.length;

        // Destroy the object after the animation finishes
        Destroy(gameObject, animationLength);
    }

    private void FixedUpdate()
    {
        vaultWindow -= 0.02f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (vaultWindow > 0 && !hasVaulted) {
            playerScript.VaultLaunch(); 
            hasVaulted = true; 
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (vaultWindow > 0 && !hasVaulted) 
        { 
            playerScript.VaultLaunch(); 
            hasVaulted = true; 
        }
    }
}
