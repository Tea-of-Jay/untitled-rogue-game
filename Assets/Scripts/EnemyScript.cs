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
    Vector2 lookDir;
    Transform targetPlayer;
    Vector2 lastKnownPlayerPos;
    public float enemySpeed;
    public int enemyHP;


    //Start is called before the first frame update.
    void Start()
    {
        lookDir = GetRandomDirection();
        rb2d = GetComponent<Rigidbody2D>();
        timeTillMove = Random.Range(0.5f, 2.5f); //Random time to begin movement between 0.5 and 2.5 seconds.
    }

    //Update is called once per frame.
    void Update()
    {
        //Perform different actions depending on state. Travel is in FixedUpdate().
        switch(state) 
        { 
            //If idle, reduce time until moving next.
            case EnemyState.Idle:
                timeTillMove -= Time.deltaTime;
                if(timeTillMove <= 0.0f) //Pick a random direction and a random time to move between 1 and 3 seconds.
                {
                    lookDir = GetRandomDirection(); //Get a random direction to look.
                    RaycastHit2D dirCheck = Physics2D.Raycast(transform.position, lookDir, 3.0f, LayerMask.GetMask("Terrain")); //Cast a ray and check for solid terrain closeby
                    while(dirCheck.collider != null) //While a new look direction is facing too close to a wall, make a new look direction.
                    {
                        lookDir = GetRandomDirection();
                        dirCheck = Physics2D.Raycast(transform.position, lookDir, 3.0f, LayerMask.GetMask("Terrain"));
                    }
                    timeToMove = Random.Range(1.0f, 3.0f);
                    ChangeState("Travel"); //change state to Travel.
                }
                if(CheckVision()) //Call CheckVision, and set state to Aggro if it returns true.
                {
                    ChangeState("Aggro");
                } 
                break;
            case EnemyState.Travel:
            //Might not need anything here?
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
        Debug.DrawRay(transform.position, lookDir, Color.cyan);
        //estimated vision
        Debug.DrawRay(transform.position, new Vector2(lookDir.x * Mathf.Cos(1.5f) - lookDir.y * Mathf.Sin(1.5f), lookDir.x * Mathf.Sin(1.5f) + lookDir.y * Mathf.Cos(1.5f)), Color.green);
        Debug.DrawRay(transform.position, new Vector2(lookDir.x * Mathf.Cos(-1.5f) - lookDir.y * Mathf.Sin(-1.5f), lookDir.x * Mathf.Sin(-1.5f) + lookDir.y * Mathf.Cos(-1.5f)), Color.green);
        //TODO: These rays will detect walls
        Debug.DrawRay(transform.position, new Vector2(lookDir.x * Mathf.Cos(0.4f) - lookDir.y * Mathf.Sin(0.4f), lookDir.x * Mathf.Sin(0.4f) + lookDir.y * Mathf.Cos(0.4f)), Color.blue); //left
        Debug.DrawRay(transform.position, new Vector2(lookDir.x * Mathf.Cos(-0.4f) - lookDir.y * Mathf.Sin(-0.4f), lookDir.x * Mathf.Sin(-0.4f) + lookDir.y * Mathf.Cos(-0.4f)), Color.magenta); //right
        switch(state)
        {
            //If travel, do movement in set direction.
            case EnemyState.Travel:
                pos = pos + enemySpeed * lookDir * Time.deltaTime; //Update position to move to
                rb2d.MovePosition(pos); //Move to position
                timeToMove -= Time.deltaTime; //Reduce time remaining to move

                RaycastHit2D fwdCheck = Physics2D.Raycast(transform.position, lookDir, 0.8f, LayerMask.GetMask("Terrain")); //Create a short line to check for solid terrain
                RaycastHit2D rightCheck = Physics2D.Raycast(transform.position, 
                    new Vector2(lookDir.x * Mathf.Cos(-0.4f) - lookDir.y * Mathf.Sin(-0.4f), lookDir.x * Mathf.Sin(-0.4f) + lookDir.y * Mathf.Cos(-0.4f)), 
                        1.0f, LayerMask.GetMask("Terrain")); //Create a short line to check for solid terrain to the right
                RaycastHit2D leftCheck = Physics2D.Raycast(transform.position, 
                    new Vector2(lookDir.x * Mathf.Cos(0.4f) - lookDir.y * Mathf.Sin(0.4f), lookDir.x * Mathf.Sin(0.4f) + lookDir.y * Mathf.Cos(0.4f)), 
                        1.0f, LayerMask.GetMask("Terrain")); //Create a short line to check for solid terrain to the left

                bool wallRight = rightCheck.collider != null;
                bool wallLeft = leftCheck.collider != null;
                if(wallRight ^ wallLeft) //If a left wall is detected XOR a right wall is detected (this will only be entered if ONLY ONE of these is true.)
                {
                    //If a wall is detected right, make lookDir rotate left. (+0.1). If not, make lookDir rotate right (-0.1)
                    //(wallRight ? 0.1f : -0.1f) will evaluate to 0.1f if wallRight is TRUE, and -0.1f is wallRight is FALSE.
                    Vector2 newDir = lookDir;
                    newDir.x = lookDir.x * Mathf.Cos(wallRight ? 0.1f : -0.1f) - lookDir.y * Mathf.Sin(wallRight ? 0.1f : -0.1f);
                    newDir.y = lookDir.x * Mathf.Sin(wallRight ? 0.1f : -0.1f) + lookDir.y * Mathf.Cos(wallRight ? 0.1f : -0.1f);
                    lookDir = newDir;
                }
                if(timeToMove <= 0.0f || fwdCheck.collider != null) //If there's no time left to move OR there is solid terrain close ahead
                {
                    ChangeState("Idle"); //Become idle.
                }
                if(CheckVision()) //Call CheckVision, and set state to Aggro if it returns true.
                {
                    ChangeState("Aggro"); //Become aggro
                } 
                break;
            //If aggro, move towards targeted player.
            case EnemyState.Aggro:
                RaycastHit2D vision = Physics2D.Raycast(transform.position, (targetPlayer.position - transform.position).normalized, 
                    Vector2.Distance(targetPlayer.position, transform.position), LayerMask.GetMask("Terrain"));
                if(vision.collider == null)
                {
                    lastKnownPlayerPos = targetPlayer.position;
                    lookDir = (targetPlayer.position - transform.position).normalized;
                }
                //if(lastKnownPlayerPos == (Vector2)transform.position)
                //{
                //    ChangeState("Idle");
                //    break;
                //}
                pos = pos + enemySpeed * lookDir * Time.deltaTime;
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
                break;
            }
        }
    }

    //Vision check to see if there is a valid player in line of sight to target, and become Aggro if so.
    bool CheckVision()
    {
        ContactFilter2D playerFilter = new ContactFilter2D();
        playerFilter.SetLayerMask(LayerMask.GetMask("Player"));
        if(Physics2D.OverlapCircle(transform.position, 10.0f, playerFilter, colliders) > 0) //See if a player is in range of this enemy.
        {
            Transform player = colliders[colliders.Count-1].gameObject.GetComponent<Transform>(); //If so, grab player's Collider2D at the end of array.
            Vector2 playerDir = (player.position - transform.position);
            RaycastHit2D ray = Physics2D.Raycast(transform.position, playerDir, Vector2.Distance(player.position, transform.position), LayerMask.GetMask("Terrain"));
            Debug.DrawRay(transform.position, playerDir, Color.cyan);
            if(ray.collider == null && Vector2.Dot(playerDir, lookDir) > 0.5f) //If the dot product of the enemy's look direction and direction of player from enemy greater than 0.5, become aggro on player.
            {
                targetPlayer = player;
                lastKnownPlayerPos = targetPlayer.position;
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
                timeTillMove = Random.Range(0.5f, 2.5f);
                state = EnemyState.Idle;
                break;
            case "Travel":
                state = EnemyState.Travel;
                break;
            case "Aggro":
                lookDir = (targetPlayer.position - transform.position);
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

    Vector2 GetRandomDirection()
    {
        return new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
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
