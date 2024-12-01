using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Build;
using UnityEngine;

public class MonsterLogicScript : MonoBehaviour
{
    //Variables
    #region

    PlayerScript playerScript;
    GameObject player;
    SpriteRenderer sp;
    Rigidbody2D rb;

    //Health
    public int hpMax;
    public int hitCounter;

    //publics
    public float dmgHitStun;
    public bool cantMove;
    public bool ignoreStun;

    [SerializeField] ParticleSystem killParticles;
    [SerializeField] ParticleSystem hitParticles;
    [SerializeField] ParticleSystem moneyParticles;
    [SerializeField] int moneyOnKill;
    [SerializeField] int deathParticleNumber;

    AudioSource audioSource;
    [SerializeField] AudioClip deathSound;

    Coroutine CantMoveCoroutine;

    public string enemyID;

    private LogicScript logicScript;


    #endregion



    // Start is called before the first frame update
    void Start()
    {
        logicScript = GameObject.FindWithTag("TimeManager").GetComponent<LogicScript>();

        if (string.IsNullOrEmpty(enemyID))
        {
            enemyID = System.Guid.NewGuid().ToString(); // Assign a unique ID using GUID
        }

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
        audioSource = GetComponent<AudioSource>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {

    }


    //Some of this should be called in the monster part
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slash") || collision.CompareTag("Vault"))
        {

            //float hitLaunch = transform.position.x - collision.transform.position.x;

            playerScript.PlayHitSound(); //TODO: eventually change this to be monster specific

            //playerScript.slashKnockback(hitLaunch);

            hitCounter++;
            if (hitParticles != null)
            {
                ParticleSystem onHitParticles = Instantiate(hitParticles, transform.position, Quaternion.identity);
                Destroy(onHitParticles.gameObject, 3);
            }

            PlayDamagedAnim(0.4f);
            if (hitCounter >= hpMax)
            {
                playerScript.hitStop(0.22f, 0.01f);
                playerScript.cam.GetComponent<CameraMovementScript>().CameraShake(0.2f, 50f, 0.2f, 0.90f);
                Invoke("Kill", 0.015f);
                
            } else
            {
                if (collision.CompareTag("Slash"))
                {
                    playerScript.cam.GetComponent<CameraMovementScript>().CameraShake(0.1f, 50f, 0.08f, 0.90f);
                    playerScript.hitStop(0.05f, 0.01f);
                }
                else if (collision.CompareTag("Vault"))
                {
                    playerScript.cam.GetComponent<CameraMovementScript>().CameraShake(0.1f, 50f, 0.08f, 0.90f);
                    playerScript.hitStop(0.05f, 0.01f);
                }
            }

         
            //rb.gravityScale = 0;
            //rb.velocity = new Vector2(0, rb.velocity.y);

            sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 0.5f); // to simulate the white flash for now lol
            
            if (!ignoreStun)
            {
                if (CantMoveCoroutine != null)
                {
                    StopCoroutine(CantMoveCoroutine);
                }
                CantMoveCoroutine = StartCoroutine(EndCantMove(0.08f + playerScript.stunOnHit)); // ends cantMove after 4 frames + how long the hitStun will be

                Invoke("DmgLaunch", playerScript.stunOnHit); // this should prob be a coroutine thing too lol

            }
            else { Invoke("ResetColor", 0.1f); }
        }

        if (collision.CompareTag("PlayerDmg") && !playerScript.invincible)
        {
            playerScript.takeDamage(gameObject, dmgHitStun, 1);
            //Debug.Log("Time Should have stopped - enemy");
            //playerScript.hitStop(0.24f, 0.01f);

        }

    }


    private void DmgLaunch() //called by monsterLogic ****************************************
    {

        ResetColor();

        //rb.gravityScale = 1; 

        float PlayerToEnemyPosition = player.transform.position.x - transform.position.x;
        if (PlayerToEnemyPosition < 0)
        {
            rb.velocity = new Vector2(playerScript.knockbackOnHit, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-(playerScript.knockbackOnHit), rb.velocity.y);
        }
    }  //Applies the knockback from the players slash


    private IEnumerator EndCantMove(float duration) //TODO: Update the other script to use this cantmove value
    {
        
        cantMove = true;

        yield return new WaitForSeconds(duration);

        cantMove = false;
        rb.velocity = Vector2.zero;
    }


    private void PlayDamagedAnim(float duration)
    {
        if (hitParticles!=null){
            ParticleSystem onHitParticles = Instantiate(hitParticles, transform.position, Quaternion.identity);
            Destroy(onHitParticles.gameObject, 5f);
        }
     //call damage animation in other script
    }


    private void CancelDamageAnim()
    {
        // call it in other script
    }

    private void ResetColor()
    {
        sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 1f); ;
    }


    public void Kill()
    {

        audioSource.PlayOneShot(deathSound);

      //  logicScript.MarkEnemyAsKilled(enemyID, transform.position);


        // play death sound
        if (killParticles != null)
        {
            for (int i = 0; i < deathParticleNumber; i++)
            {
                ParticleSystem onKillParticles = Instantiate(killParticles, transform.position, Quaternion.identity);
                Destroy(onKillParticles.gameObject, 5f);
            }
        }

        if (moneyParticles != null)
        {
            ParticleSystem moneyDropParticles = Instantiate(moneyParticles, transform.position, Quaternion.identity);

            // Modify the burst count
            moneyDropParticles.GetComponent<MoneyParticleScript>().SpawnMoney(moneyOnKill);


        }
        
        Destroy(gameObject);
    }


}
