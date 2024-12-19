using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPIconScript : MonoBehaviour
{
    public bool isBroken;
    private Animator animator;
    [SerializeField] GameObject particleContainer;
    [SerializeField] ParticleSystem soulParticlesBreak;
    [SerializeField] ParticleSystem glassParticlesBreak;
    [SerializeField] ParticleSystem lightningParticlesBreak;
    RectTransform imageRectTransform;

    private void Start()
    {
        isBroken = false;
        animator = GetComponent<Animator>();
        imageRectTransform = GetComponent<RectTransform>();
        CanvasRenderer canvasRenderer = GetComponent<CanvasRenderer>();

    }

    private void FixedUpdate()
    {
        if (isBroken) 
        { 
            animator.SetBool("isBroken", true);
        } else
        {
            animator.SetBool("isBroken", false);
        }
    }

    private void Update()
    {
        particleContainer.transform.position = GetWorldPositionFromCenter();
    }

    public void BreakOrb()
    {

        soulParticlesBreak.Play();
        glassParticlesBreak.Play();
        lightningParticlesBreak.Play();
    }

    public void HealOrb()
    {

    }
    Vector3 GetWorldPositionFromCenter()
    {
        // Get the corners of the UI element
        Vector3[] corners = new Vector3[4];
        imageRectTransform.GetWorldCorners(corners);

        // Calculate center point
        Vector3 center = (corners[0] + corners[2]) / 2;

        // Convert to world position
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(center);
        worldPos.z = 0;
        return worldPos;
        // Use: transform.position = worldPos;
    }

}
