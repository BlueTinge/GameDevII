using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//If an attack collides with Health, it deals damage
//Can be extended to provide additional effects
public class Attack : MonoBehaviour
{
    public float Damage { get; private set; }
    public float Knockback { get; private set; }
    public GameObject Origin { get; private set; }

    public void Initialize(float damage, float knockback, float timeToLive, GameObject origin)
    {
        Damage = damage;
        Knockback = knockback;
        Origin = origin;

        if (timeToLive != 0f) Invoke("EndAttack", timeToLive);
    }

    //method stub in base class
    //extend for more specific effects
    //called when a health collides with this attack
    public float GetDamageFor(GameObject other)
    {
        return Damage;
    }

    //Called when attacke ends normally or from animation events, etc.
    //e. g. player is hurt so attack is destroyed early
    //extend for more specificity
    public void EndAttack()
    {
        Destroy(this);
    }
}
