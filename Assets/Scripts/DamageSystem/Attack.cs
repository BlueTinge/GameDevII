using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//If an attack collides with Health, it deals damage
//Can be extended to provide additional effects
public class Attack : MonoBehaviour
{

    public float Damage { get; private set; }
    public Vector3 Knockback { get; private set; }
    public GameObject Origin { get; private set; }

    //delegates used to provide more customizability on a per-enemy-type basis
    //e.g. holy sword more effective vs. undead, or something

    //called whenever attack collides with other, default just returns damage
    public delegate float GetDamageDelegate(GameObject other);
    public GetDamageDelegate GetDamageFor;

    //called whenever attack collides with other, default just returns knockback
    public delegate Vector3 GetKnockbackDelegate(GameObject other);
    public GetKnockbackDelegate GetKnockbackFor;

    public void Initialize(float damage, Vector3 knockback, float timeToLive, GameObject origin)
    {
        Damage = damage;
        Knockback = knockback;
        Origin = origin;

        if (timeToLive != 0f) Invoke("EndAttack", timeToLive);

        GetDamageFor = delegate (GameObject other)
        {
            return Damage;
        };

        GetKnockbackFor = delegate (GameObject other)
        {
            return Knockback;
        };
    }



    //Called when attack ends normally or from animation events, etc.
    //e. g. player is hurt so attack is destroyed early
    //extend for more specificity
    public void EndAttack()
    {
        Destroy(this);
    }
}
