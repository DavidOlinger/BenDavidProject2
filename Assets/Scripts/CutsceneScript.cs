using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CutsceneScript : MonoBehaviour
{

    TextMeshProUGUI[] text;
    public int counter = 0;

    private void Start()
    {
        text = GetComponentsInChildren<TextMeshProUGUI>();


        Invoke("textAppear", 3);
        counter = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayerPrefs.SetInt("Scene", 1);
            SceneManager.LoadScene(1);
        }
    }
    

    public void textAppear()
    {
       
        text[counter].enabled = true;
        Invoke("textGone", 3f);
        Debug.Log("ttttt");
    }

    public void textGone()
    {
        Debug.Log("whyyyy");

        text[counter].enabled = false;
        counter = counter + 1;

        if (counter == 9)
        {
            PlayerPrefs.SetInt("Scene", 1);
            SceneManager.LoadScene(1);
        }

        Invoke("textAppear", 1.5f);
    }
}
