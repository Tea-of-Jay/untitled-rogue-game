using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    Rigidbody2D rb2d;
    public EnemyState state;
    float timeTillMove;
    float timeToMove = 0.0f;
    List<Collider2D> colliders = new List<Collider2D>();
    Vector2 lookDir = new Vector2(0, 0);
    Transform targetPlayer;
    public float enemySpeed;
    public int enemyHP;


    //Start is called before the first frame update.
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        timeTillMove = Random.Range(0.5f, 2.5f); //Random time to begin movement between 0.5 and 2.5 seconds.
    }

    //Update is called once per frame.
    void Update()
    {
        //Perform different actions depending on state. Travel is in FixedUpdate().
        Debug.DrawLine(transform.position, ((Vector2)transform.position + (lookDir * 3)), Color.cyan);
        switch(state) 
        { 
            //If idle, reduce time until moving next.
            case EnemyState.Idle:
                timeTillMove -= Time.deltaTime;
                //Debug.Log("Currently Idle. Time till move: " +timeTillMove);
                if(timeTillMove <= 0.0f) //Pick a random direction and a random time to move between 1 and 3 seconds.
                {
                    lookDir.x = Random.Range(-1.0f, 1.0f);
                    lookDir.y = Random.Range(-1.0f, 1.0f);
                    lookDir.Normalize();
                    timeToMove = Random.Range(1.0f, 3.0f);
                    state = EnemyState.Travel; //change state to Travel.
                }
                CheckVision(); //Call CheckVision, which will set state to Aggro if there's a valid player nearby.
                break;
            case EnemyState.Travel:
                if(timeToMove <= 0.0f)
                {
                    timeTillMove = Random.Range(0.5f, 2.5f);
                    state = EnemyState.Idle;
                }
                break;
            //Currently unimplemented.
            case EnemyState.Aggro:
                break;
            //Currently unimplemented.
            case EnemyState.Attack:
                break;
        }
    }

    //FixedUpdate is called each physics step.
    void FixedUpdate()
    {
        Vector2 pos = rb2d.position;
        switch(state)
        {   
            //If travel, do movement in set direction.
            case EnemyState.Travel:
                //Debug.Log("Currently Travelling. Time to move: " +timeToMove);
                pos.x = pos.x + enemySpeed * lookDir.x * Time.deltaTime;
                pos.y = pos.y + enemySpeed * lookDir.y * Time.deltaTime;
                rb2d.MovePosition(pos);
                timeToMove -= Time.deltaTime;
                CheckVision(); //Call CheckVision, which will set state to Aggro if there's a valid player nearby.
                break;
            //If aggro, move towards targeted player.
            case EnemyState.Aggro:
                lookDir = (targetPlayer.position - transform.position).normalized;
                pos.x = pos.x + enemySpeed * lookDir.x * Time.deltaTime;
                pos.y = pos.y + enemySpeed * lookDir.y * Time.deltaTime;
                rb2d.MovePosition(pos);
                timeToMove -= Time.deltaTime;
                break;
        }
    }

    //If the enemy enters a trigger...
    void OnTriggerEnter2D(Collider2D trigger)
    {
        //Determine which trigger type was entered.
        switch(trigger.tag)
        {
            //If the enemy came into contact with a player's attack, deal damage and destroy the attack object.
            case "PlayerAttack":
            {
                int dmg = trigger.gameObject.GetComponent<BulletScript>().damage;
                TakeDamage(dmg);
                Destroy(trigger.gameObject);
                //Debug.Log("Hit by " +trigger.gameObject);
                break;
            }
        }
    }

    //Vision check to see if there is a valid player in line of sight to target, and become Aggro if so.
    bool CheckVision()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Player"));
        if(Physics2D.OverlapCircle(transform.position, 10.0f, filter, colliders) > 0) //See if a player is in range of this enemy.
        {
            Transform player = colliders[colliders.Count-1].gameObject.GetComponent<Transform>(); //If so, grab player's Collider2D at the end of array.
            Vector2 playerDir = (player.position - transform.position).normalized;
            if(Vector2.Dot(playerDir, lookDir) > 0.5f) //If the dot product of the enemy's look direction and direction of player from enemy greater than 0.5, become aggro on player.
            {
                lookDir = playerDir;
                targetPlayer = player;
                state = EnemyState.Aggro;
                return true;
            }
        }
        return false;
    }

    //Public HP reducer. Destroy this enemy if HP falls to 0 or below.
     public void TakeDamage(int d)
    {
        enemyHP -= d;
        if(enemyHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    //Public state set function. Take the string passed in and change state based on that.
    public void ChangeState(string s)
    {
        switch(s)
        {
            case "Idle":
                state = EnemyState.Idle;
                break;
            case "Travel":
                state = EnemyState.Travel;
                break;
            case "Aggro":
                state = EnemyState.Aggro;
                break;
            case "Attack":
                state = EnemyState.Attack;
                break;
        }
    }

    public string GetState()
    {
        switch(state)
        {
            case EnemyState.Idle:
                return "Idle";
            case EnemyState.Travel:
                return "Travel";
            case EnemyState.Aggro:
                return "Aggro";
            case EnemyState.Attack:
                return "Attack";
            default:
                return "None";
        }
    }
}

//Collection of different states this enemy can be.
public enum EnemyState
{
    Idle,
    Travel,
    Aggro,
    Attack
}
