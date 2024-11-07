using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionScript : MonoBehaviour
{
    public int nextSceneIndex;

    public Vector2 spawnPoint;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            PlayerPrefs.SetFloat("SavePointX", spawnPoint.x);
            PlayerPrefs.SetFloat("SavePointY", spawnPoint.y);

            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
