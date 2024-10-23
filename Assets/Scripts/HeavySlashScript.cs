using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavySlashScript : MonoBehaviour
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
    }
    public void DisableCollider()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }
    public void DestroySelf() {
        Debug.Log("DESTROY HEAVY SLASH PREFAB");
        Destroy(gameObject); 
    }

    public void TestEvent()
    {
        Debug.Log("If you'rereading this it worked.");

    }

}
