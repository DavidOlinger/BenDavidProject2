using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionScript : MonoBehaviour
{
    public int nextSceneIndex;

    public Vector2 spawnPoint;


    CameraMovementScript camScript;
    PlayerScript pScript;

    private void Start()
    {
        camScript = GameObject.FindWithTag("MainCamera").GetComponent<CameraMovementScript>();
        pScript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();  

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            PlayerPrefs.SetFloat("SavePointX", spawnPoint.x);
            PlayerPrefs.SetFloat("SavePointY", spawnPoint.y);
            PlayerPrefs.SetInt("Scene", nextSceneIndex);
            PlayerPrefs.SetInt("CurrHP", pScript.currHP);

            camScript.fadeToBlack(0f);
            Invoke("goNext", 0.5f);
        }
    }

    void goNext()
    {
        SceneManager.LoadScene(nextSceneIndex);
    }
}
