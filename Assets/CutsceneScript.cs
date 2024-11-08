using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CutsceneScript : MonoBehaviour
{

    public TextMeshProUGUI[] text;



    // Start is called before the first frame update
    void Start()
    {
        Invoke("textAppear", 3);
    }


    private int counter = 0;
    // Update is called once per frame
    

    void textAppear()
    {
        if(counter == 8)
        {
            PlayerPrefs.SetInt("Scene", 1);
            SceneManager.LoadScene(1);
        }
        
        text[counter].enabled = true;
        Invoke("textGone", 3f);
    }

    void textGone()
    {
        text[counter].enabled = false;
        counter = counter + 1;

        Invoke("textAppear", 1.5f);
    }
}
