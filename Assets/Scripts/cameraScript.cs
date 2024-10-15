using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour
{
    public GameObject Player;

    Vector3 offset = new Vector3(0, 0, -10);


    private void LateUpdate()
    {
        if (Player != null)
        {
            transform.position = new Vector3(Player.transform.position.x + offset.x, offset.y, offset.z);

        }
    }
}
