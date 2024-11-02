using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    public void OnButtonClick()
    {
        if (PlayerPrefs.HasKey("Scene"))
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("Scene"));
        }
        else
        {
            PlayerPrefs.SetInt("Scene", 1);
            SceneManager.LoadScene(1);
        }
    }




}