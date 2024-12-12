using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBossScript : MonoBehaviour
{
    public ParticleSystem fireBeam;
    public float sweepTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BeamSweep());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator BeamSweep()
    {
        fireBeam.Play();
        fireBeam.GetComponent<BoxCollider2D>().enabled = true;
        
        Quaternion startRotation = fireBeam.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, 180);

        // Elapsed time
        float elapsedTime = 0f;

        // Interpolate rotation until duration is reached
        while (elapsedTime < sweepTime)
        {
            // Calculate interpolation parameter (0 to 1)
            float t = elapsedTime / sweepTime;

            // Use Slerp for smooth rotation
            fireBeam.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for next frame
            yield return null;
        }

        // Ensure the final rotation is exactly the target rotation
        fireBeam.transform.rotation = targetRotation;
        fireBeam.Stop();
        fireBeam.GetComponent<BoxCollider2D>().enabled = false;


        StartCoroutine(BeamSweep());
    }
}
