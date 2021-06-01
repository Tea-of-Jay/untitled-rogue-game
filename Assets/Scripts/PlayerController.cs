using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb2d;
    float horizontal;
    float vertical;
    Vector2 lookDir = new Vector2(1, 0);
    Vector2 velocity = new Vector2(0, 0);

    public float playerSpeed;
    public int playerHP;
    public int playerMoney;
    public GameObject primaryAttackPrefab;

    //Start is called before the first frame update.
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    //Update is called once per frame.
    void Update()
    {
        //Store horizontal and vertical keypress values.
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        //If M1 is pressed, do primary attack.
        if (Input.GetMouseButtonDown(0))
        {
            PrimaryAttack();
        }
    }

    //FixedUpdate is called each physics step.
    void FixedUpdate()
    {
        //Adjust look direction based on location of mouse, 
            //and new position/velocity based on current position, keypress values, speed and time.
        Vector2 mouseLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos = rb2d.position;
        Vector2 prevPos = pos;
        pos.x = pos.x + playerSpeed * horizontal * Time.deltaTime; pos.y = pos.y + playerSpeed * vertical * Time.deltaTime;
        velocity.x = (pos.x - prevPos.x) / Time.deltaTime; velocity.y = (pos.y - prevPos.y) / Time.deltaTime;
        lookDir.x = mouseLoc.x - pos.x; lookDir.y = mouseLoc.y - pos.y;
        lookDir.Normalize();
        rb2d.MovePosition(pos);
    }

    //If the player enters a trigger...
    void OnTriggerStay2D(Collider2D trigger)
    {
        //Determine which trigger type was entered.
        // switch(trigger.tag)
        // {
        //     //If the player entered the vision range of an enemy, make sure enemy is NOT aggro AND there is NOT an obstructed linecast between the player and enemy.
        //     case "EnemyVision":
        //         RaycastHit2D ray = Physics2D.Raycast(transform.position, trigger.GetComponent<Collider2D>().GetComponentInParent<Transform>().position, 
        //             1 << LayerMask.NameToLayer("Terrain")); //Cast ray
        //         Debug.Log("Ray found " +ray.collider);
        //         //if(ray.collider == null) && !trigger.GetComponent<Collider2D>().GetComponentInParent<EnemyScript>().GetState().Equals("Aggro"))
        //             //trigger.GetComponent<Collider2D>().GetComponentInParent<EnemyScript>().ChangeState("Aggro");
        //         break;
        // }
    }

    //Perform primary attack. TODO: when adding new characters, move this into character specific scripts(?)
    public void PrimaryAttack() //temporarily just launches a throwing knife by creating one and calling it's function 'Shoot().'
    {
        GameObject primaryAttack = Instantiate(primaryAttackPrefab, rb2d.position, Quaternion.identity);
        BulletScript bullet = primaryAttack.GetComponent<BulletScript>();
        bullet.Shoot(lookDir, velocity);
    }
}
