using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public float PlayerBounceFactor = -1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //called internally from a collision with Attack
    //call this externally to send an attack to this HealthStats
    public void RecieveAttack(Attack attack, PlayerController pl)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        Vector3 knockback = attack.GetKnockbackFor(gameObject);
        if (rb != null) rb.AddForce(knockback);

        if(pl != null)
        {
            StartCoroutine(pl.BounceBack(knockback * PlayerBounceFactor));
        }
    }

    //I hate using "stay" as it may potentially lead to performance issues
    //but not using stay means attack is ignored if created after collider has already entered collision
    //potential solution: check for collisions in attack, when attack is initialized?
    //  --this "solution" does not account for immunity when you first collide w/ an attack, and immunity ending afterwards. 

    private void OnTriggerStay(Collider other)
    {
        Attack attack = other.gameObject.GetComponentInParent<Attack>();
        PlayerController pl = other.gameObject.GetComponentInParent<PlayerController>();

        if (attack != null)
        {
            RecieveAttack(attack, pl);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        Attack attack = other.gameObject.GetComponentInParent<Attack>();
        PlayerController pl = other.gameObject.GetComponentInParent<PlayerController>();

        if (attack != null)
        {
            RecieveAttack(attack, pl);
        }
    }
}
