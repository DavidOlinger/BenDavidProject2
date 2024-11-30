using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHealth : MonoBehaviour
{


    LogicScript logicScript;

    void Start()
    {
        logicScript = GameObject.FindWithTag("TimeManager").GetComponent<LogicScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        logicScript.UpdateHealth();
    }
}
