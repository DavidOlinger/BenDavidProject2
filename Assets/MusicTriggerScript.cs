using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTriggerScript : MonoBehaviour
{
    MusicManagerScript musicManagerScript;
    public AudioClip song;
    // Start is called before the first frame update
    void Start()
    {
        musicManagerScript = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            musicManagerScript.ChangeMusic(song);
        }
    }
}
