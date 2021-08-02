using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashScript : MonoBehaviour
{
    Rigidbody2D rb2d;
    public int damage;
    Vector2 direction = new Vector2(0, 0);

    //Awake is called when object is created.
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    //Update is called once per frame.
    void Update()
    {

    }

    //Called when this BoxCollider2D hits another, which is 'solid.' Projectile self destructs.
    void OnCollisionEnter2D(Collision2D solid)
    {
        Destroy(gameObject);
        Debug.Log("Collided with " +solid.gameObject);
    }

    //Rotate object quickly(?)
    public void Slash(Vector2 dir)
    {
        direction = dir;
        rb2d.AddTorque(1.0f);
    }
}
