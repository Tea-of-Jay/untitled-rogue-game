using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    protected Rigidbody2D rb2d;
    protected Vector2 lookDir = new Vector2(1, 0);
    protected Vector2 velocity = new Vector2(0, 0);
    float horizontal, vertical, 
        primaryCooldownTimer, secondaryCooldownTimer, supportCooldownTimer, movementCooldownTimer, ultimateCooldownTimer;
    public float primaryCooldown, secondaryCooldown, supportCooldown, movementCooldown, ultimateCooldown;

    public float playerSpeed;
    public int playerHP, playerMoney;
    public GameObject primaryAttackPrefab;
    public GameObject secondaryAttackPrefab;

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

        //If M1 is held down and cooldown is 0, do primary attack.
        if(Input.GetMouseButton(0) && primaryCooldownTimer == 0.0f)
        {
            PrimaryAttack();
            primaryCooldownTimer = primaryCooldown;
        }
        //If M2 is pressed and cooldown is 0, do secondary attack.
        else if(Input.GetMouseButtonDown(1) && secondaryCooldownTimer == 0.0f)
        {
            SecondaryAttack();
            secondaryCooldownTimer = secondaryCooldown;
        }
        //If C is pressed and cooldown is 0, do support skill.
        else if(Input.GetKeyDown("c") && supportCooldownTimer == 0.0f)
        {
            SupportSkill();
            supportCooldownTimer = supportCooldown;
        }
        //If V is pressed and cooldown is 0, do movement skill.
        else if(Input.GetKeyDown("v") && movementCooldownTimer == 0.0f)
        {
            MovementSkill();
            movementCooldownTimer = movementCooldown;
        }
        //If Q is pressed and cooldown is 0, do ultimate skill.
        else if(Input.GetKeyDown("q") && ultimateCooldownTimer == 0.0f)
        {
            UltimateSkill();
            ultimateCooldownTimer = ultimateCooldown;
        }
        //Call reduce cooldown function.
        reduceCooldowns();
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

    //For each cooldown, reduce the value by deltaTime if it's greater than 0. If not, greater than 0, leave it at 0.
    void reduceCooldowns()
    {
        primaryCooldownTimer = (primaryCooldownTimer > 0.0f ? primaryCooldownTimer -= Time.deltaTime : 0.0f);
        secondaryCooldownTimer = (secondaryCooldownTimer > 0.0f ? secondaryCooldownTimer -= Time.deltaTime : 0.0f);
        supportCooldownTimer = (supportCooldownTimer > 0.0f ? supportCooldownTimer -= Time.deltaTime : 0.0f);
        movementCooldownTimer = (movementCooldownTimer > 0.0f ? movementCooldownTimer -= Time.deltaTime : 0.0f);
        ultimateCooldownTimer = (ultimateCooldownTimer > 0.0f ? ultimateCooldownTimer -= Time.deltaTime : 0.0f);
    }

    //Abstract skill methods.
    public abstract void PrimaryAttack(); //Default M1, basic damage attack skill unlocked by default.
    public abstract void SecondaryAttack(); //Default M2, secondary damage attack skill unlocked by levelling up.
    public abstract void SupportSkill(); //Default C, support based damage/utility skill unlocked by levelling up.
    public abstract void MovementSkill(); //Default V, movement based damage/utility skill unlocked by levelling up.
    public abstract void UltimateSkill(); //Default Q, anything goes damage/utility skill unlocked by levelling up.
}
