using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    Rigidbody2D rb2d;
    public EnemyState state;
    float timeTillMove;
    float timeToMove = 0.0f;
    float pi = Mathf.PI;
    bool wallRight, wallAngleRight, wallLeft, wallAngleLeft;
    List<Collider2D> colliders = new List<Collider2D>();
    Vector2 lookDir, lastKnownPlayerPos, lastKnownPlayerDir, rightView, leftView;
    Transform targetPlayer;

    //A line to check for solid terrain ahead, 
        //A line to check for solid terrain at an angle to the right, A line to check for solid terrain at an angle to the left,
            //A line to check for solid terrain directly to the right, A line to check for solid terrain directly to the left
    RaycastHit2D fwdCheck, rightAngleCheck, leftAngleCheck, rightCheck, leftCheck;
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
                if(CheckForPlayer()) //Call CheckForPlayer, and set state to Aggro if it returns true.
                {
                    ChangeState("Aggro");
                } 
                break;
            case EnemyState.Travel:
            //Might not need anything here?
                break;
            //Currently unimplemented, might not need anything here.
            case EnemyState.Aggro:
                break;
            //Currently unimplemented.
            case EnemyState.Attack:
                break;
            //Default switch case.
            default:
                break;
        }
    }

    //FixedUpdate is called each physics step.
    void FixedUpdate()
    {
        Vector2 pos = rb2d.position;

        //estimated vision

        //look direction
        Debug.DrawRay(transform.position, lookDir, Color.green);
        //angles
        Debug.DrawRay(transform.position, new Vector2(lookDir.x * Mathf.Cos(pi/2) - lookDir.y * Mathf.Sin(pi/2), lookDir.x * Mathf.Sin(pi/2) + lookDir.y * Mathf.Cos(pi/2)), Color.blue);
        Debug.DrawRay(transform.position, new Vector2(lookDir.x * Mathf.Cos(-pi/2) - lookDir.y * Mathf.Sin(-pi/2), lookDir.x * Mathf.Sin(-pi/2) + lookDir.y * Mathf.Cos(-pi/2)), Color.red);
        //directly left and right
        Debug.DrawRay(transform.position, new Vector2(lookDir.x * Mathf.Cos(pi/8) - lookDir.y * Mathf.Sin(pi/8), lookDir.x * Mathf.Sin(pi/8) + lookDir.y * Mathf.Cos(pi/8)), Color.cyan); //left
        Debug.DrawRay(transform.position, new Vector2(lookDir.x * Mathf.Cos(-pi/8) - lookDir.y * Mathf.Sin(-pi/8), lookDir.x * Mathf.Sin(-pi/8) + lookDir.y * Mathf.Cos(-pi/8)), Color.magenta); //right

        switch(state)
        {
            //If travel, do movement in set direction.
            case EnemyState.Travel:
            {
                UpdateLineOfSight();
        
                if(wallAngleRight ^ wallAngleLeft) //If a left wall is detected at an angle XOR a right wall is detected at an angle
                {
                    float angleChange = (wallAngleRight ? 0.1f : 0f) + (wallAngleLeft ? -0.1f : 0f) + (wallRight ? 0.1f : 0f) + (wallLeft ? -0.1f : 0f);
                    //If a wall is detected right, make lookDir rotate left. (+0.1). If not, make lookDir rotate right (-0.1)
                    //(wallRight ? 0.1f : -0.1f) will evaluate to 0.1f if wallRight is TRUE, and -0.1f is wallRight is FALSE.
                    Vector2 newDir = lookDir;
                    newDir.x = lookDir.x * Mathf.Cos(angleChange) - lookDir.y * Mathf.Sin(angleChange);
                    newDir.y = lookDir.x * Mathf.Sin(angleChange) + lookDir.y * Mathf.Cos(angleChange);
                    lookDir = newDir;
                }

                pos = pos + enemySpeed * lookDir * Time.deltaTime; //Update position to move to
                rb2d.MovePosition(pos); //Move to position
                timeToMove -= Time.deltaTime; //Reduce time remaining to move

                if(timeToMove <= 0.0f || fwdCheck.collider != null) //If there's no time left to move OR there is solid terrain close ahead
                {
                    ChangeState("Idle"); //Become idle.
                }
                if(CheckForPlayer()) //Call CheckForPlayer, and set state to Aggro if it returns true.
                {
                    ChangeState("Aggro"); //Become aggro.
                }
            }
                break;
            //If aggro, move towards targeted player.
            case EnemyState.Aggro:
            {
                //Check to see if a line between the enemy's position and player position is unobstructed.
                RaycastHit2D visionCheck = Physics2D.Linecast((Vector2)transform.position, (Vector2)targetPlayer.position, LayerMask.GetMask("Terrain"));
                Debug.DrawRay((Vector2)transform.position, (lastKnownPlayerPos - (Vector2)transform.position), Color.white);

                if(visionCheck.collider == null) //If there is no wall in between the enemy and player, update knowledge about player location.
                {
                    Debug.Log("spotted!");
                    lastKnownPlayerPos = targetPlayer.position;
                    Debug.DrawRay((Vector2)transform.position, lastKnownPlayerDir, Color.black, 5.0f);
                }
                lastKnownPlayerDir = (lastKnownPlayerPos - (Vector2)transform.position).normalized; //Update last known player direction from the enemy, since the enemy is always moving.
                UpdateLineOfSight(); //Call UpdateLineOfSight, which will cast rays at varying angles from the lookDir and update bools depending on what is/is not seen.
                rightView.x = lookDir.y; rightView.y = -1*lookDir.x; //update rightView to be a vector pointing 90 degrees to the right of lookDir,
                leftView = -1*rightView; //and leftView to point in the opposite direction of rightView.
                //Add to angleChange if there is a wall at an angle to the right, directly to the right, and/or the player is to the left.
                //Reduce from angleChange if there is a wall at an angle to the left, directly left, and/or player is to the right.
                //If angleChange is positive, the rotation goes counterclockwise. if negative, clockwise.
                float angleChange = (wallAngleRight ? 0.2f : 0f) + (wallAngleLeft ? -0.2f : 0f) 
                    + (wallRight ? 0.15f : 0f) + (wallLeft ? -0.15f : 0f)
                        + ((Vector2.Dot(lastKnownPlayerDir, leftView) > 0.0f) ? 0.1f : 0f) + ((Vector2.Dot(lastKnownPlayerDir, rightView) > 0.0f) ? -0.1f : 0f);
                //If the angle needs to change, then adjust lookDir.
                if(angleChange != 0)
                {
                    Vector2 newDir = lookDir;
                    newDir.x = lookDir.x * Mathf.Cos(angleChange) - lookDir.y * Mathf.Sin(angleChange);
                    newDir.y = lookDir.x * Mathf.Sin(angleChange) + lookDir.y * Mathf.Cos(angleChange);
                    lookDir = newDir.normalized;
                }
                //if already looking close enough towards the last known player position, just set lookdir to playerdir. (this might not work rn?)
                else if(Vector2.Dot(lookDir, lastKnownPlayerDir) > 0.9f)
                {
                        lookDir = lastKnownPlayerDir.normalized;
                }
                //if close enough to the last known player location, become idle.
                //TODO: adjust this to be an attack state, or do something else. Idle is only temporary.
                if(Vector2.Distance(lastKnownPlayerPos, transform.position) <= 0.5f)
                {
                    ChangeState("Idle");
                    break;
                }
                //Adjust enemy position.
                pos = pos + enemySpeed * lookDir * Time.deltaTime;
                rb2d.MovePosition(pos);
            }
                break;
            //Default switch case.
            default:
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
    bool CheckForPlayer()
    {
        ContactFilter2D playerFilter = new ContactFilter2D();
        playerFilter.SetLayerMask(LayerMask.GetMask("Player"));
        if(Physics2D.OverlapCircle(transform.position, 10.0f, playerFilter, colliders) > 0) //See if a player is in range of this enemy.
        {
            Transform player = colliders[colliders.Count-1].gameObject.GetComponent<Transform>(); //If so, grab player's Collider2D at the end of array.
            Vector2 playerDir = (player.position - transform.position).normalized;
            RaycastHit2D ray = Physics2D.Raycast(transform.position, playerDir, Vector2.Distance(player.position, transform.position), LayerMask.GetMask("Terrain"));
            Debug.DrawRay(transform.position, (player.position - transform.position), Color.cyan);
            if(ray.collider == null && Vector2.Dot(playerDir, lookDir) > 0.5f) //If the dot product of the enemy's look direction and direction of player from enemy greater than 0.5, become aggro on player.
            {
                targetPlayer = player;
                lastKnownPlayerPos = targetPlayer.position;
                lastKnownPlayerDir = (lastKnownPlayerPos - (Vector2)transform.position).normalized;
                return true;
            }
        }
        return false;
    }

    //Updates fwdCheck, rightCheck, leftCheck, rightAngleCheck, and leftAngleCheck depending on current location and look direction.
    void UpdateLineOfSight()
    {
        float pi = Mathf.PI;
        fwdCheck = Physics2D.Raycast(transform.position, lookDir, 1.0f, LayerMask.GetMask("Terrain")); //Create a short line to check for terrain straight ahead
        rightCheck = Physics2D.Raycast(transform.position, 
            new Vector2(lookDir.x * Mathf.Cos(-(pi/2)) - lookDir.y * Mathf.Sin(-(pi/2)), lookDir.x * Mathf.Sin(-(pi/2)) + lookDir.y * Mathf.Cos(-(pi/2))), 
                1.5f, LayerMask.GetMask("Terrain"));
        leftCheck = Physics2D.Raycast(transform.position, 
            new Vector2(lookDir.x * Mathf.Cos(pi/2) - lookDir.y * Mathf.Sin(pi/2), lookDir.x * Mathf.Sin(pi/2) + lookDir.y * Mathf.Cos(pi/2)), 
                1.5f, LayerMask.GetMask("Terrain"));
        rightAngleCheck = Physics2D.Raycast(transform.position, 
            new Vector2(lookDir.x * Mathf.Cos(-(pi/8)) - lookDir.y * Mathf.Sin(-(pi/8)), lookDir.x * Mathf.Sin(-(pi/8)) + lookDir.y * Mathf.Cos(-(pi/8))), 
                1.5f, LayerMask.GetMask("Terrain")); //Create a short line to check for solid terrain at an angle to the right
        leftAngleCheck = Physics2D.Raycast(transform.position, 
            new Vector2(lookDir.x * Mathf.Cos(pi/8) - lookDir.y * Mathf.Sin(pi/8), lookDir.x * Mathf.Sin(pi/8) + lookDir.y * Mathf.Cos(pi/8)), 
                1.5f, LayerMask.GetMask("Terrain")); //Create a short line to check for solid terrain at an angle to the left
        
        wallAngleRight = rightAngleCheck.collider != null; //true if there is a wall to the right angle
        wallAngleLeft = leftAngleCheck.collider != null; //true if there is a wall to the left angle
        wallRight = rightCheck.collider != null; //true if wall directly right
        wallLeft = leftCheck.collider != null; //true if wall directly left
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
                lookDir = (targetPlayer.position - transform.position).normalized;
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
