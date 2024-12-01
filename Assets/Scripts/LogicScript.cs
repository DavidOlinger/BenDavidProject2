using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class LogicScript : MonoBehaviour
{
    //Variables
    #region
    public bool isPaused = false;
    public GameObject QuitButton;
    public GameObject CoverScreen;
    Image screenCover;
    Image buttonImage;
    TextMeshProUGUI quitText;

    public GameObject ReturnButton;
    Image returnButtonImage;
    TextMeshProUGUI returnText;

    public GameObject player;
    PlayerScript playerScript;

    [SerializeField] private Image[] hpIcons;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource ambienceSource;
    [SerializeField] TMP_Text moneyText;

    private string breakablesSaveFilePath;
    //private string enemiesSaveFilePath;
    private SaveData saveData;

    [System.Serializable]
    public class SaveData
    {
        public List<string> permanentlyBroken = new List<string>();
        public List<string> temporarilyBroken = new List<string>();
    }

    #endregion

    private void Awake()
    {
        breakablesSaveFilePath = Path.Combine(Application.persistentDataPath, "breakables.json");
        LoadData();
        
    }

    public void MarkBreakableAsBroken(string objectID, bool neverRespawn)
    {
        if (neverRespawn)
        {
            if (!saveData.permanentlyBroken.Contains(objectID))
            {
                Debug.Log(objectID);
                saveData.permanentlyBroken.Add(objectID);
            }
        }
        else
        {
            if (!saveData.temporarilyBroken.Contains(objectID))
            {
                saveData.temporarilyBroken.Add(objectID);
            }
        }

        SaveBreak();
    }

    public bool IsBreakableBroken(string objectID)
    {
        return saveData.permanentlyBroken.Contains(objectID) || saveData.temporarilyBroken.Contains(objectID);
    }

    private void SaveBreak()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(breakablesSaveFilePath, json);
    }

    private void LoadData()
    {
        if (File.Exists(breakablesSaveFilePath))
        {
            string json = File.ReadAllText(breakablesSaveFilePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            saveData = new SaveData();
        }
    }

    //Start + Update
    #region


    void Start()
    {
        


        //GameObject.FindWithTag("MainCamera").GetComponent<CameraMovementScript>().fadeToBlack(0);
        musicSource = GetComponent<AudioSource>();
        //musicSource.loop = true;
        //musicSource.volume = 0.5f;
        //musicSource.Play(); //all this can be done in editor
        Time.timeScale = 1;

        screenCover = CoverScreen.GetComponent<Image>();
        buttonImage = QuitButton.GetComponent<Image>();
        quitText = QuitButton.GetComponentInChildren<TextMeshProUGUI>();

        returnButtonImage = ReturnButton.GetComponent<Image>();
        returnText = ReturnButton.GetComponentInChildren<TextMeshProUGUI>();

        playerScript = player.GetComponent<PlayerScript>();


        //if (!PlayerPrefs.HasKey("loadBool"))
        //{
        //    PlayerPrefs.SetInt("loadBool", 1);
        //}

        if (PlayerPrefs.HasKey("isStartingGame"))
        {
            if(PlayerPrefs.GetInt("isStartingGame") > 0)
            {
                loadSavePoint();
            }
            else
            {
                loadTransition();
            }
        }



        
        if (!PlayerPrefs.HasKey("Money"))
        {
            PlayerPrefs.SetInt("Money", 0);
        }
        addMoney(0);
    }

    #endregion










    //SAVING
    #region
    public void addMoney(int amount)
    {
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") + amount);
        if (moneyText != null)
        {
            string money = "$" + PlayerPrefs.GetInt("Money");
            moneyText.text = money;
        }

    }



    public void savePoint(Vector2 spawnPoint)
    {
        PlayerPrefs.SetFloat("SavePointX", spawnPoint.x);
        PlayerPrefs.SetFloat("SavePointY", spawnPoint.y);

        PlayerPrefs.SetInt("Scene", SceneManager.GetActiveScene().buildIndex);

        playerScript.currHP = playerScript.maxHP;
        UpdateHealth();

        //SaveEnemyData();

        //RespawnEnemies();
    }

    public void loadSavePoint()
    {
        if (PlayerPrefs.HasKey("SavePointX"))
        {
            player.transform.position = new Vector3(PlayerPrefs.GetFloat("SavePointX"), PlayerPrefs.GetFloat("SavePointY"), 0);
            Invoke("UpdateHealth", 0.5f);
            PlayerPrefs.SetInt("isStartingGame", 0);
        }
    }

    public void loadTransition()
    {
        if (PlayerPrefs.HasKey("SavePointX")) // later should change this to different variable
        {
            player.transform.position = new Vector3(PlayerPrefs.GetFloat("SavePointX"), PlayerPrefs.GetFloat("SavePointY"), 0);
        }
    }

    public void gameStartSceneLoad()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("Scene"));
    }

    #endregion



    //MENU
    #region
    public void MenuPause()
    {
        Time.timeScale = 0;
        isPaused = true;

        screenCover.enabled = true;
        buttonImage.enabled = true;
        quitText.enabled = true;

        returnButtonImage.enabled = true;
        returnText.enabled = true;
    }

    public void MenuUnPause()
    {
        Time.timeScale = 1;
        isPaused = false;

        screenCover.enabled = false;
        buttonImage.enabled = false;
        quitText.enabled = false;

        returnButtonImage.enabled = false;
        returnText.enabled = false;
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(0);
    }
    #endregion


    public void UpdateHealth()
    {
        for (int i = 0; i < hpIcons.Length; i++)
        {
            if (playerScript.currHP > i)
            {
                hpIcons[i].GetComponent<HPIconScript>().isBroken = false;
                //unbroken
            }
            else
            {
                hpIcons[i].GetComponent<HPIconScript>().isBroken = true;

                //broken
            }
        }
    }



}