using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    public void OnButtonClick()
    {
        PlayerPrefs.SetInt("isStartingGame", 1);
        if (!PlayerPrefs.HasKey("Scene")) // changed for now for testing
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("Scene"));
        }
        else
        {
            PlayerPrefs.SetFloat("SavePointX", 0);
            PlayerPrefs.SetFloat("SavePointY", 0);
            PlayerPrefs.SetInt("Scene", 1);
            SceneManager.LoadScene(1);
        }
    }




}