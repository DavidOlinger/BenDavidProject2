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
        public List<string> dontRespawnEnemies = new List<string>(); // Tracks enemies that should never respawn


    }

    private Dictionary<string, MonsterLogicScript> enemyDictionary = new Dictionary<string, MonsterLogicScript>();



    #endregion

    private void Awake()
    {
        breakablesSaveFilePath = Path.Combine(Application.persistentDataPath, "breakables.json");
        // Initialize paths and load save data
        enemiesSaveFilePath = Path.Combine(Application.persistentDataPath, "enemies.json");
        Debug.Log(Application.persistentDataPath);
        LoadData();

    }

    private void Update()  //THIS IS JUST FOR TESTING, REMOVE FOR FULL GAME AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //  Debug.Log("RESPAWNING ENEMIES");
            RespawnEnemies();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("AHHH CLEARING THE SAVES");
            ClearAllSaves();
        }

    }

    //Enemy Death Saving
    #region



    public bool IsDontRespawn(string enemyID)
    {
        return saveData.dontRespawnEnemies.Contains(enemyID);
    }


    public void RespawnEnemies()
    {
        foreach (var enemyID in saveData.disabledEnemies)
        {
            if (enemyDictionary.TryGetValue(enemyID, out MonsterLogicScript enemy))
            {
               // Debug.Log($"Re-enabling enemy with ID: {enemyID}");
                if (!enemy.dontRespawn)
                {
                    enemy.gameObject.SetActive(true);
                    enemy.hitCounter = 0;
                    enemy.transform.position = enemy.spawnPos;
                }
                
            }
            else
            {
                Debug.LogWarning($"Failed to find enemy to re-enable with ID: {enemyID}");
            }
        }

        saveData.disabledEnemies.Clear(); // Clear the list after respawning
        SaveEnemies();
    }

    public void MarkEnemyAsKilled(string enemyID)
    {
       // Debug.Log($"Marking enemy as killed: {enemyID}");
        if (enemyDictionary.TryGetValue(enemyID, out MonsterLogicScript enemy))
        {
            //enemy.gameObject.SetActive(false);

            if (enemy.dontRespawn)
            {
                // Add to dontRespawnEnemies if not already present
                if (!saveData.dontRespawnEnemies.Contains(enemyID))
                {
                    saveData.dontRespawnEnemies.Add(enemyID);
                }
            }
            else if (!saveData.disabledEnemies.Contains(enemyID))
            {
                saveData.disabledEnemies.Add(enemyID);
            }

            SaveEnemies();
        }
        else
        {
            Debug.LogWarning($"Cannot disable enemy. ID {enemyID} not found in dictionary.");
        }
    }

    private MonsterLogicScript FindEnemyByID(string enemyID)
    {
        if (enemyDictionary.TryGetValue(enemyID, out MonsterLogicScript enemy))
        {
            //Debug.Log($"Found enemy in dictionary with ID: {enemyID}");
            return enemy;
        }

       // Debug.LogWarning($"No enemy found in dictionary with ID: {enemyID}");
        return null;
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

        if (PlayerPrefs.HasKey("isStartingGame"))
        {
            if (PlayerPrefs.GetInt("isStartingGame") > 0)
            {
                ClearAllSaves();
                PlayerPrefs.SetInt("isStartingGame", 0);
                gameStartSceneLoad();
            }
        }

        foreach (var enemy in FindObjectsOfType<MonsterLogicScript>())
        {
            if (!enemyDictionary.ContainsKey(enemy.enemyID))
            {
                enemyDictionary[enemy.enemyID] = enemy;
            }
        }

        //to load a saved campfire
        string lastCampfireID = PlayerPrefs.GetString("LastCampfireID", "");

        if (!string.IsNullOrEmpty(lastCampfireID))
        {
            // Find the campfire with the matching ID
            GameObject[] campfires = GameObject.FindGameObjectsWithTag("SavePoint");
            GameObject savedCampfire = null;

            foreach (GameObject campfire in campfires)
            {
                CampfireParticleScript campfireScript = campfire.GetComponent<CampfireParticleScript>();
                if (campfireScript != null && campfireScript.id == lastCampfireID)
                {
                    savedCampfire = campfire;
                    break;
                }
            }

            if (savedCampfire != null)
            {
                Debug.Log($"Found saved campfire with ID: {lastCampfireID}");

                CampfireParticleScript campfireScript = savedCampfire.GetComponent<CampfireParticleScript>();
                if (campfireScript != null)
                {
                    // Safely attempt to light the campfire
                    campfireScript.Light();
                }
            }
            else
            {
                Debug.LogWarning($"No campfire found with ID: {lastCampfireID}");
            }
        }



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

        loadSavePoint();



        
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



    public void savePoint(GameObject savepoint)
    {
        if (savepoint.GetComponent<CampfireParticleScript>() != null) 
        {
            PlayerPrefs.SetString("LastCampfireID", savepoint.GetComponent<CampfireParticleScript>().id);
        }
        Vector3 spawnPoint = savepoint.transform.position;
        PlayerPrefs.SetFloat("SavePointX", spawnPoint.x);
        PlayerPrefs.SetFloat("SavePointY", spawnPoint.y);

        PlayerPrefs.SetInt("Scene", SceneManager.GetActiveScene().buildIndex);

        playerScript.currHP = playerScript.maxHP;
        UpdateHealth();


        RespawnEnemies();
    }

    public void loadSavePoint() // this 
    {
        if (PlayerPrefs.HasKey("SavePointX"))
        {
            player.transform.position = new Vector3(PlayerPrefs.GetFloat("SavePointX"), PlayerPrefs.GetFloat("SavePointY"), 0);
            Invoke("UpdateHealth", 0.5f);
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
        PlayerPrefs.SetInt("CurrHP", PlayerPrefs.GetInt("MaxHP"));



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


    public void ClearAllSaves()
    {

        Debug.Log($"PermanentlyBroken list BEFORE clearing: {string.Join(", ", saveData.permanentlyBroken)}");
        // Clear the lists
        saveData.permanentlyBroken.Clear();
        // *** DEBUG: Log the contents of permanentlyBroken after clearing ***
        Debug.Log($"PermanentlyBroken list after clearing: {string.Join(", ", saveData.permanentlyBroken)}");



        Debug.Log($"DisabledEnemies list BEFORE clearing: {string.Join(", ", saveData.disabledEnemies)}");
        saveData.disabledEnemies.Clear();
        // *** DEBUG: Log the contents of disabledEnemies after clearing ***
        Debug.Log($"DisabledEnemies list after clearing: {string.Join(", ", saveData.disabledEnemies)}");



        Debug.Log($"DontRespawnEnemies list BEFORE clearing: {string.Join(", ", saveData.dontRespawnEnemies)}");
        saveData.dontRespawnEnemies.Clear();
        // *** DEBUG: Log the contents of dontRespawnEnemies after clearing ***
        Debug.Log($"DontRespawnEnemies list after clearing: {string.Join(", ", saveData.dontRespawnEnemies)}");




        // Write an empty SaveData instance to the JSON file
        string emptyJson = JsonUtility.ToJson(new SaveData(), true);

        // Clear the breakables save file
        if (File.Exists(breakablesSaveFilePath))
        {
            File.WriteAllText(breakablesSaveFilePath, emptyJson);
            // *** DEBUG: Log confirmation that the breakables save file was cleared ***
            Debug.Log($"Breakables save file cleared at path: {breakablesSaveFilePath}");
        }
        else
        {
            // *** DEBUG: Log if the breakables save file does not exist ***
            Debug.LogWarning($"Breakables save file does not exist at path: {breakablesSaveFilePath}");
        }

        // Clear the enemies save file
        if (File.Exists(enemiesSaveFilePath))
        {
            File.WriteAllText(enemiesSaveFilePath, emptyJson);
            // *** DEBUG: Log confirmation that the enemies save file was cleared ***
            Debug.Log($"Enemies save file cleared at path: {enemiesSaveFilePath}");
        }
        else
        {
            // *** DEBUG: Log if the enemies save file does not exist ***
            Debug.LogWarning($"Enemies save file does not exist at path: {enemiesSaveFilePath}");
        }

        // *** DEBUG: Final confirmation of save clearing ***
        Debug.Log("All game data cleared for a new game.");
    }


    //public void ClearAllSaves()
    //{
    //    // Clear the lists
    //    saveData.permanentlyBroken.Clear();
    //    Debug.Log(saveData.permanentlyBroken);

    //    saveData.disabledEnemies.Clear();
    //    Debug.Log(saveData.disabledEnemies);

    //    saveData.dontRespawnEnemies.Clear();
    //    Debug.Log(saveData.dontRespawnEnemies);


    //    // Write an empty SaveData instance to the JSON file
    //    string emptyJson = JsonUtility.ToJson(new SaveData(), true);

    //    // Clear  save files
    //    if (File.Exists(breakablesSaveFilePath))
    //    {
    //        File.WriteAllText(breakablesSaveFilePath, emptyJson);
    //    }

    //    string enemiesSaveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
    //    if (File.Exists(enemiesSaveFilePath))
    //    {
    //        File.WriteAllText(enemiesSaveFilePath, emptyJson);
    //    }


    //    //string emptyJson2 = JsonUtility.ToJson(new SaveData(), true);

    //    //// Clear the breakables save file
    //    //if (File.Exists())
    //    //{
    //    //    File.WriteAllText(, emptyJson2);
    //    //}



    //    Debug.Log("All game data cleared for a new game.");
    //}

}