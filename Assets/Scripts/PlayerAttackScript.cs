using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackScript : MonoBehaviour
{
    public int damage;
    protected Vector2 direction = new Vector2(0, 0);
    protected PlayerController attackUser;

    //Awake is called when object is created.
    protected virtual void Awake()
    {
        ExtraSetup();
    }

    protected virtual void ExtraSetup()
    {

    }

    public void SetUser(PlayerController p)
    {
        attackUser = p;
    }


}
