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

    public GameObject primaryAttackPrefab;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetMouseButtonDown(0))
        {
            PrimaryAttack();
        }
    }

    void FixedUpdate()
    {
        Vector2 mouseLoc = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos = rb2d.position;
        Vector2 prevPos = pos;
        pos.x = pos.x + playerSpeed * horizontal * Time.deltaTime;
        pos.y = pos.y + playerSpeed * vertical * Time.deltaTime;
        velocity.x = (pos.x - prevPos.x) / Time.deltaTime;
        velocity.y = (pos.y - prevPos.y) / Time.deltaTime;
        lookDir.x = mouseLoc.x - pos.x;
        lookDir.y = mouseLoc.y - pos.y;
        lookDir.Normalize();
        rb2d.MovePosition(pos);
    }

    public void PrimaryAttack() //when adding new characters, move this into character specific scripts(?)
    {
        GameObject primaryAttack = Instantiate(primaryAttackPrefab, rb2d.position, Quaternion.identity);
        BulletScript bullet = primaryAttack.GetComponent<BulletScript>();
        bullet.Shoot(lookDir, velocity);
    }
}
