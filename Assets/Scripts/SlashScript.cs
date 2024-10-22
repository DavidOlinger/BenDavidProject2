using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashScript : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    SpriteRenderer sp;
    Animator animator;
    public float animationLength;
    public Vector3 slashPosition;



    void Start()
    {
        Transform parentTransform = transform.parent;
        player = parentTransform.gameObject;
        sp = gameObject.GetComponent<SpriteRenderer>();

        // Get the Animator component
        animator = GetComponent<Animator>();

        // Get the length of the current animation clip
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        animationLength = clipInfo[0].clip.length;

        // Destroy the object after the animation finishes
        Destroy(gameObject, animationLength);
    }

    
}
