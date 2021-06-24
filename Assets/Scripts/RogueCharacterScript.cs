using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueCharacterScript : PlayerController
{
    public override void PrimaryAttack()
    {
        GameObject primaryAttack = Instantiate(primaryAttackPrefab, rb2d.position, Quaternion.identity);
        BulletScript bullet = primaryAttack.GetComponent<BulletScript>();
        bullet.Shoot(lookDir, velocity);
    }
}
