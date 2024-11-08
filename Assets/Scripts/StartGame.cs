using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    public void StartNewGame()
    {
        PlayerPrefs.SetInt("isStartingGame", 1);
        
            PlayerPrefs.SetFloat("SavePointX", 0);
            PlayerPrefs.SetFloat("SavePointY", 0);
            PlayerPrefs.SetInt("Scene", 1);
        PlayerPrefs.SetInt("Money", 0);
        PlayerPrefs.SetFloat("CanVault", 0);
        PlayerPrefs.SetInt("WallJump", 0);
        SceneManager.LoadScene(1);
    }

    public void LoadSave()
    {
        PlayerPrefs.SetInt("isStartingGame", 1);
        if (PlayerPrefs.HasKey("Scene"))
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("Scene"));
        }
        else
        {
            StartNewGame();
        }
    }




}