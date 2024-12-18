using System.Collections;
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

    public Vector2 spawnPos;

    private LogicScript logicScript;

    public bool dontRespawn;

    public bool playerKnockback = true;


    #endregion



    // Start is called before the first frame update
    void Start()
    {
        cantMove = false;

        spawnPos = gameObject.transform.position;

        logicScript = GameObject.FindWithTag("TimeManager").GetComponent<LogicScript>();

        if (string.IsNullOrEmpty(enemyID))
        {
            enemyID = System.Guid.NewGuid().ToString(); // Assign a unique ID
        }


        if (logicScript.IsDontRespawn(enemyID))
        {
            Debug.Log($"Disabling enemy {enemyID} as it is marked as dontRespawn.");
            gameObject.SetActive(false);
        }


        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
        audioSource = GetComponent<AudioSource>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    

    public void waveStart()
    {

        
            if (CantMoveCoroutine != null)
            {
                StopCoroutine(CantMoveCoroutine);
            }
            CantMoveCoroutine = StartCoroutine(EndCantMove(0.08f + playerScript.stunOnHit)); // ends cantMove after 4 frames + how long the hitStun will be

            Invoke("DmgLaunch", playerScript.stunOnHit); // this should prob be a coroutine thing too lol
        
        
    }


    //Some of this should be called in the monster part
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slash") || collision.CompareTag("Vault"))
        {

            takeDmg(collision);
            
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
            playerScript.takeDamage(gameObject, dmgHitStun, 1, playerKnockback);
            //Debug.Log("Time Should have stopped - enemy");
            //playerScript.hitStop(0.24f, 0.01f);

        }


        if (collision.gameObject.CompareTag("Rock"))
        {
            Debug.Log("COLLIDED");
            if (Mathf.Abs(collision.gameObject.GetComponentInParent<Rigidbody2D>().velocity.x) > 3.5)
            {
                takeDmg(collision);
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

                collision.gameObject.GetComponentInParent<Rigidbody2D>().velocity = Vector2.zero;


            }
            else if (Mathf.Abs(collision.gameObject.GetComponentInParent<Rigidbody2D>().velocity.y) > 2)
            {
                takeDmg(collision);


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

                collision.gameObject.GetComponentInParent<Rigidbody2D>().velocity = Vector2.zero;

            }
        }

    }


    public void takeDmg(Collider2D collision)
    {
        playerScript.PlayHitSound(); //TODO: eventually change this to be monster specific

        //playerScript.slashKnockback(hitLaunch);

        if(PlayerPrefs.GetInt("Boon1") == 1)
        {
            hitCounter = hitCounter + 2;
        }
        else
        {
            hitCounter++;
        }
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

        }
        else
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
            else if (collision.CompareTag("HeavySlash"))
            {
                playerScript.cam.GetComponent<CameraMovementScript>().CameraShake(0.1f, 50f, 0.08f, 0.90f);
                playerScript.hitStop(0.05f, 0.01f);
            }
        }


        //rb.gravityScale = 0;
        //rb.velocity = new Vector2(0, rb.velocity.y);

        if (!collision.CompareTag("HeavySlash"))
        {
            sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 0.5f); // to simulate the white flash for now lol
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

        cantMove = false;

        // Disable enemy and notify LogicScript
        logicScript.MarkEnemyAsKilled(enemyID);
        gameObject.SetActive(false); // Disable this enemy
        //Destroy(gameObject);
    }


}
