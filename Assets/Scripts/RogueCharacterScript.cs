using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueCharacterScript : PlayerController
{
    //FOR THE ROGUE PREFAB: Movement Cooldown will be set to 0. This is to allow two uses of it if desired (to enter and then exit)
        //Instead, the cooldown will be set in this script for 8 seconds after Shadowwalk ends.
    bool isShadowwalking;
    float shadowwalkTimer;

    public override void CharacterUpdate()
    {
        if(isShadowwalking)
        {
            shadowwalkTimer -= Time.deltaTime;
            if(shadowwalkTimer <= 0.0f)
            {
                ExitShadowwalk();
            }
        }
    }

    //Primary Skill - Throwing Knife 
    //TODO: Fix bug where contacting a wall will immediately destroy the knife
    public override void PrimaryAttack()
    {
        if(isShadowwalking)
        {
            ExitShadowwalk();
            //throw 3 knives
            //start cooldown
        }
        GameObject primaryAttack = Instantiate(primaryAttackPrefab, rb2d.position, Quaternion.identity);
        RoguePrimaryScript knife = primaryAttack.GetComponent<RoguePrimaryScript>();
        knife.SetUser(this.GetComponent<PlayerController>());
        knife.Shoot(lookDir);
    }
    public override void SecondaryAttack()
    {
        //quick slash - close range melee swipe, does more damage/stuns on backstab
        GameObject secondaryAttack = Instantiate(secondaryAttackPrefab, rb2d.position, Quaternion.identity);
        throw new System.NotImplementedException();
    }
    public override void SupportSkill()
    {
        //threaten - stun enemies in look direction for .5 seconds, and they get a debuff where they drop more money on death
        throw new System.NotImplementedException();
    }
    //Movement Skill - Shadowwalk - become invis for 3 seconds, becoming slightly faster and stunning enemies in a short range around you on exit for .5 secs (press v again/3 seconds pass/use another attack)
    public override void MovementSkill()
    {
        if(isShadowwalking)
        {
            ExitShadowwalk();
        }else{
            //make player transparent?
            Debug.Log("walking");
            isInvis = true;
            isShadowwalking = true;
            playerSpeed = baseSpeed*1.2f;
            shadowwalkTimer = 3.0f;
        }
    }
    public override void UltimateSkill()
    {
        //pitch black - create a shadow field at your location for 7 seconds, slowing enemies inside. while in the field, M1 throws 3 knives in a fan shape
        throw new System.NotImplementedException();
    }

    void ExitShadowwalk()
    {
        Debug.Log("no longer walkign");
        isInvis = false;
        isShadowwalking = false;
        playerSpeed = baseSpeed;
        movementCooldownTimer = 8.0f;
        //Stun enemies in a short range
    }
}
