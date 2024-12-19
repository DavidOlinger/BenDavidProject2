using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamageScript : MonoBehaviour
{
    GameObject player;
    PlayerScript playerScript;
    public float dmgHitStun;
    public bool playerKnockback;
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("PlayerDmg") && !playerScript.invincible)
        {
            playerScript.takeDamage(gameObject, dmgHitStun, damage, playerKnockback);
            //Debug.Log("Time Should have stopped - enemy");
            //playerScript.hitStop(0.24f, 0.01f);

        }

    }

}
