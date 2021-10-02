using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoguePrimaryScript : PlayerAttackScript
{
    float initTime;
    Rigidbody2D rb2d;
    public float projectileSpeed;

    protected override void Awake()
    {
        base.Awake();
        rb2d = GetComponent<Rigidbody2D>();
    }
    protected override void ExtraSetup()
    {
        base.ExtraSetup();
        initTime = Time.time;
    }
    //Update is called once per frame.
    void Update()
    {
        //If projectile has been alive for 1 second or more, destroy it.
        if (Time.time - initTime >= 1.0f)
        {
            Destroy(gameObject);
        }
    }

    //Called when this BoxCollider2D hits another, which is 'solid.' Projectile self destructs.
    void OnCollisionEnter2D(Collision2D collider)
    {
        //If collider is an enemy, call it's TakeDamage function.
        if(collider.gameObject.tag.Equals("Enemy"))
        {
            EnemyScript es = collider.gameObject.GetComponent<EnemyScript>();
            es.TakeDamage(damage);
        }
        Destroy(gameObject);
        Debug.Log("Collided with " +collider.gameObject);
    }

    //Launch projectile towards direction 'dir,' with varying velocity 'vel.'
    public void Shoot(Vector2 dir)
    {
        direction = dir;
        rb2d.AddForce(direction * projectileSpeed);
    }
}
