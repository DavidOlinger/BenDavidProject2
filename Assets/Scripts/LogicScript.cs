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
    private string enemiesSaveFilePath; // Path for storing enemy data

    private SaveData saveData;


    [System.Serializable]
    public class SaveData
    {
        public List<string> permanentlyBroken = new List<string>();
        public List<string> disabledEnemies = new List<string>(); // Store IDs of disabled enemies


    }





    #endregion

    private void Awake()
    {
        breakablesSaveFilePath = Path.Combine(Application.persistentDataPath, "breakables.json");
        // Initialize paths and load save data
        enemiesSaveFilePath = Path.Combine(Application.persistentDataPath, "enemies.json");

        LoadData();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnEnemies();
        }
    }

    //Enemy Death Saving
    #region

    public void RespawnEnemies()
    {
        Debug.Log($"RespawnEnemies called. Disabled enemies count: {saveData.disabledEnemies.Count}");
        foreach (var enemyID in saveData.disabledEnemies)
        {
            MonsterLogicScript enemy = FindEnemyByID(enemyID);
            if (enemy != null)
            {
                Debug.Log($"Re-enabling enemy with ID: {enemyID}");
                enemy.gameObject.SetActive(true); // Re-enable the enemy
            }
            else
            {
                Debug.LogWarning($"Failed to find enemy to re-enable with ID: {enemyID}");
            }
        }

        foreach (var enemyID in saveData.disabledEnemies)
        {
            Debug.Log($"Attempting to respawn enemy with ID: {enemyID}");
        }

        saveData.disabledEnemies.Clear(); // Clear disabled enemies list after respawning
        SaveEnemies();
    }

    public void MarkEnemyAsKilled(string enemyID)
    {
        if (!saveData.disabledEnemies.Contains(enemyID))
        {
            saveData.disabledEnemies.Add(enemyID);
            SaveEnemies();
        }
    }

    private MonsterLogicScript FindEnemyByID(string enemyID)
    {
        foreach (var enemy in FindObjectsOfType<MonsterLogicScript>())
        {
            Debug.Log($"Checking enemy with ID: {enemy.enemyID}");
            if (enemy.enemyID == enemyID)
            {
                Debug.Log($"Found enemy with ID: {enemyID}");
                return enemy;
            }
        }

        Debug.Log($"No enemy found with ID: {enemyID}");
        return null; // Enemy not found
    }

    private void SaveEnemies()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(enemiesSaveFilePath, json);
    }

    #endregion



    // Breakables
    #region
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
        

        SaveBreak();
    }

    public bool IsBreakableBroken(string objectID)
    {
        return saveData.permanentlyBroken.Contains(objectID);
    }

    private void SaveBreak()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(breakablesSaveFilePath, json);
    }

    #endregion


    private void LoadData()
    {
        // Updated to load both breakables and enemies
        if (File.Exists(breakablesSaveFilePath))
        {
            string json = File.ReadAllText(breakablesSaveFilePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            saveData = new SaveData();
        }

        //if (File.Exists(enemiesSaveFilePath))
        //{
        //    string enemyJson = File.ReadAllText(enemiesSaveFilePath);
        //    var loadedData = JsonUtility.FromJson<SaveData>(enemyJson);
        //    saveData.killedEnemies = loadedData.killedEnemies; // Load enemy data
        //}

        if (File.Exists(enemiesSaveFilePath))
        {
            string json = File.ReadAllText(enemiesSaveFilePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            saveData = new SaveData();
        }


    }









    //Start
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



    //PLAYER SAVING
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