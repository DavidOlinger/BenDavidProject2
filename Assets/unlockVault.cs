using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class unlockVault : MonoBehaviour
{
    public TextMeshProUGUI vaultText;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        vaultText.enabled = true;
        PlayerPrefs.SetFloat("CanVault", 1);
        Invoke("stopText", 2);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        vaultText.enabled = true;
        PlayerPrefs.SetFloat("CanVault", 1);
        Invoke("stopText", 2);
    }

    void stopText()
    {
        vaultText.enabled = false;
    }

}
