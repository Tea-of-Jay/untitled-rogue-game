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
    public override void SecondaryAttack()
    {
        //quick slash
        GameObject secondaryAttack = Instantiate(secondaryAttackPrefab, rb2d.position, Quaternion.identity);
        throw new System.NotImplementedException();
    }
    public override void SupportSkill()
    {
        //threaten
        throw new System.NotImplementedException();
    }
    public override void MovementSkill()
    {
        //shadowwalk
        throw new System.NotImplementedException();
    }
    public override void UltimateSkill()
    {
        //pitch black
        throw new System.NotImplementedException();
    }
}
