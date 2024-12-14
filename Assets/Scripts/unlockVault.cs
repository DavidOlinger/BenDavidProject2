using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class unlockVault : MonoBehaviour
{
    public TextMeshProUGUI vaultText;

    private LogicScript logicScript;
    private BreakableScript breakScript;

    public GameObject backCover;
    private Image cover;

    public GameObject frontCover;
    private Image thickCover;

    //public GameObject vaultImage;
    //public GameObject vaultSlashImage;
    //private Image vImage;
    //private Image slashImage;


    public bool isVault;
    private void Start()
    {
        logicScript = GameObject.FindWithTag("TimeManager").GetComponent<LogicScript>();
        foreach (Transform child in transform)
        {
            breakScript = child.GetComponent<BreakableScript>();
        }


        cover = backCover.GetComponent<Image>();
        thickCover = frontCover.GetComponent<Image>();
        //vImage = vaultImage.GetComponent<Image>();
        //slashImage = vaultSlashImage.GetComponent<Image>();



    }

    public void VaultUnlock()
    {

        logicScript.isPaused = true;

        if (isVault)
        {
            PlayerPrefs.SetFloat("CanVault", 1);
        }

        else
        {
            PlayerPrefs.SetFloat("HeavySlash", 1);
        }
        vaultText.enabled = true;
        cover.enabled = true;
        thickCover.enabled = true;
        //vImage.enabled = true;
        //slashImage.enabled = true;

        isUnlocked = true;

    }

    private bool isTriggered = false;
    private bool isUnlocked = false;
    private void Update()
    {

        if (breakScript.hitPoints == 0 && !isTriggered)
        {
            Invoke("VaultUnlock", 2);

            //play a sound
            //play a particle effect, prob just steal the one from the alters

            isTriggered = true;
        }

        
            if (logicScript.isPaused == false && isUnlocked)
            {
                vaultText.enabled = false;
                cover.enabled = false;
                thickCover.enabled = false;
                //vImage.enabled = false;
                //slashImage.enabled = false;


        }

    }

    

}
