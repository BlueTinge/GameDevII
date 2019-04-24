using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStats : MonoBehaviour
{
    public float MaxHealth;
    public float Defense = 0;
    public float BaseImmunityPeriod = 1f;

    [SerializeField]protected float _currentHealth;
    public float CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            if (!isImmune || value > _currentHealth)
            {
                _currentHealth = value;
                if (_currentHealth <= 0) OnDeath(_currentHealth);
            }
        }
    }
    public bool isImmune = false;

    //delegates used for communicating with host controller on event
    public delegate void DeathDelegate(float overkill);
    public DeathDelegate OnDeath;
    public delegate void DamageDelegate(float damage);
    public DamageDelegate OnDamage;
    public delegate void KnockbackDelegate(Vector3 knockback);
    public KnockbackDelegate OnKnockback;
    public delegate void ImmunityEndDelegate();
    public ImmunityEndDelegate OnImmunityEnd;

    // Start is called before the first frame update
    void Awake()
    {
        _currentHealth = MaxHealth;

        if(OnDeath==null) OnDeath = delegate (float damage) { UnityEngine.Debug.Log("OnDeath not set"); };
        if(OnDamage==null) OnDamage = delegate (float damage) { UnityEngine.Debug.Log("OnDamage not set"); };
        if(OnImmunityEnd==null) OnImmunityEnd = delegate () { UnityEngine.Debug.Log("OnImmunityEnd not set"); };
        if(OnKnockback==null) OnKnockback = delegate (Vector3 knockback)
        {
            Rigidbody rb= gameObject.GetComponent<Rigidbody>();
            if (rb != null) rb.AddForce(knockback);
        };
    }

    //sets new health value
    //return actual amount taken
    public float TakeDamage(float damage)
    {
        float prevHealth = CurrentHealth;
        CurrentHealth -= Mathf.Max(damage - Defense, 0);
        float taken = (prevHealth - CurrentHealth);
        OnDamage(taken);
        return taken;
    }

    //called internally from a collision with Attack
    //call this externally to send an attack to this HealthStats
    public void RecieveAttack(Attack attack)
    {
        if (attack != null && gameObject != attack.Origin)
        {
            float damage = attack.GetDamageFor(gameObject);

            if (attack.SecondLightSwing)
            {
                //2nd attack can pierce def
                attack.SecondLightSwing = false;
                isImmune = false;
            }

            if (TakeDamage(damage) > 0)
            {
                //immunity period
                //TODO: allow specific attacks to influence immunity period
                isImmune = true;
                Invoke("EndImmunity", BaseImmunityPeriod);

                Vector3 knockback = attack.GetKnockbackFor(gameObject);
                OnKnockback(knockback);
            }

        }
    }

    //I hate using "stay" as it may potentially lead to performance issues
    //but not using stay means attack is ignored if created after collider has already entered collision
    //potential solution: check for collisions in attack, when attack is initialized?
    //  --this "solution" does not account for immunity when you first collide w/ an attack, and immunity ending afterwards. 

    private void OnTriggerStay(Collider other)
    {
        Attack attack = other.gameObject.GetComponentInParent<Attack>();
        if (attack != null)
        {
            RecieveAttack(attack);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        Attack attack = other.gameObject.GetComponentInParent<Attack>();
        if (attack != null)
        {
            RecieveAttack(attack);
        }
    }

    public void EndImmunity()
    {
        if(CurrentHealth > 0)
        {
            isImmune = false;
            OnImmunityEnd();
        }
    }

    public float GetImmunity()
    {
        return BaseImmunityPeriod;
    }
}
