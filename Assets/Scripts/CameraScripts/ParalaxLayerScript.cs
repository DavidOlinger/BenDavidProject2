using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParalaxLayerScript : MonoBehaviour
{
    GameObject cam;
    public float paralaxCoef;
    public Vector3 originPos;


    void Start()
    {
        originPos = transform.position;
        Transform parentTransform = transform.parent;
        cam = parentTransform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(paralaxCoef * (cam.transform.position.x - originPos.x), transform.position.y, transform.position.z);
        
    }
}
