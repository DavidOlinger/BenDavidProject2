using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraBounds : MonoBehaviour
{
    public float DownLevel;
    public float UpLevel;
    public float LeftLevel;
    public float RightLevel;

    private CameraMovementScript cScript;

    private void Start()
    {
        cScript = GameObject.FindWithTag("MainCamera").GetComponent<CameraMovementScript>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cScript.updateBoundaries(DownLevel, UpLevel, LeftLevel, RightLevel);
        }
    }
}
