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
        PlayerPrefs.SetInt("Scene", 7);
        PlayerPrefs.SetInt("Money", 0);
        PlayerPrefs.SetFloat("CanVault", 0);
        PlayerPrefs.SetInt("WallJump", 0);
        PlayerPrefs.SetInt("MaxHP", 4);
        PlayerPrefs.SetInt("CurrHP", 4);
        PlayerPrefs.SetInt("Bless1", 0);
        PlayerPrefs.SetInt("Bless2", 0);
        PlayerPrefs.SetInt("Bless3", 0);
        PlayerPrefs.SetInt("Boon1", 0);
        PlayerPrefs.SetInt("Boon2", 0);
        PlayerPrefs.SetInt("Boon3", 0);

        SceneManager.LoadScene("Cutscene");
    }

    public void LoadSave()
    {
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