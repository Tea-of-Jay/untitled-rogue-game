using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    float initTime;
    Rigidbody2D rb2d;
    public float projectileSpeed;
    Vector2 direction = new Vector2(0, 0);
    Vector2 velocity = new Vector2(0, 0);

    // Awake called when object is created.
    void Awake()
    {
        initTime = Time.time;
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - initTime >= 1.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public void Shoot(Vector2 dir, Vector2 vel)
    {
        direction = dir;
        velocity = vel;
        rb2d.AddForce(direction * projectileSpeed + (velocity*50));
    }
}
