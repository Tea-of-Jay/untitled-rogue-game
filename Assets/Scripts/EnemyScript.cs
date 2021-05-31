using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    Rigidbody2D rb2d;
    EnemyState state;
    float timeTillMove;
    float timeToMove = 0.0f;
    Vector2 lookDir = new Vector2(0, 0);
    public float enemySpeed;
    public int enemyHP;


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        timeTillMove = Random.Range(0.5f, 2.5f);
        state = EnemyState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case EnemyState.Idle:
            {
                timeTillMove -= Time.deltaTime;
                if(timeTillMove <= 0.0f)
                {
                    lookDir.x = Random.Range(-1.0f, 1.0f);
                    lookDir.y = Random.Range(-1.0f, 1.0f);
                    lookDir.Normalize();
                    timeToMove = Random.Range(1.0f, 3.0f);
                    state = EnemyState.Travel;
                }
                break;
            }
            case EnemyState.Travel:
            {
                Vector2 pos = rb2d.position;
                if(timeToMove > 0)
                {
                    pos.x = pos.x + enemySpeed * lookDir.x * Time.deltaTime;
                    pos.y = pos.y + enemySpeed * lookDir.y * Time.deltaTime;
                    rb2d.MovePosition(pos);
                    timeToMove -= Time.deltaTime;
                }else{
                    state = EnemyState.Idle;
                    timeTillMove = Random.Range(0.5f, 2.5f);
                }
                break;
            }
            case EnemyState.Aggro:
            {
                break;
            }
            case EnemyState.Attack:
            {
                break;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D playerAttack)
    {
        //Destroy(gameObject);
        int dmg = playerAttack.gameObject.GetComponent<BulletScript>().damage;
        TakeDamage(dmg);
        Destroy(playerAttack.gameObject);
        Debug.Log("Hit by " +playerAttack.gameObject);
    }

    void TakeDamage(int d)
    {
        enemyHP -= d;
        if(enemyHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}

public enum EnemyState
{
    Idle,
    Travel,
    Aggro,
    Attack
}
