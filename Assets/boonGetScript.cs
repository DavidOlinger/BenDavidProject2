using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class boonGetScript : MonoBehaviour
{
    public TextMeshProUGUI boonText;


    private BreakableScript breakScript;


    
    private void Start()
    {
        foreach (Transform child in transform)
        {
            breakScript = child.GetComponent<BreakableScript>();
        }



    }

    public void BoonUnlock()
    {


        if(breakScript.objectID == "Alter1")
        {
            PlayerPrefs.SetInt("Boon1", 1);
            PlayerPrefs.SetInt("Bless1", 0);


        }
        else if (breakScript.objectID == "Alter2")
        {
            PlayerPrefs.SetInt("Boon2", 1);
            PlayerPrefs.SetInt("Bless2", 0);


        }

        boonText.enabled = true;


        Invoke("go", 3);

    }



    public void go()
    {
        boonText.enabled = false;
    }


    private bool isTriggered = false;
    private void Update()
    {

        if (breakScript.hitPoints == 0 && !isTriggered)
        {
            BoonUnlock();
            //play a sound
            //play a particle effect, prob just steal the one from the alters

            isTriggered = true;
        }

    }


}
