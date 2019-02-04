using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStats : MonoBehaviour
{
    public float MaxHealth;
    public float Defense = 0;
    public float BaseImmunityPeriod = 1f;

    public float _currentHealth;
    public float CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            if (!isImmune)
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
    public delegate void ImmunityEndDelegate();
    public ImmunityEndDelegate OnImmunityEnd;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = MaxHealth;
    }

    //return actual amount taken
    public float TakeDamage(float damage)
    {
        float prevHealth = CurrentHealth;
        CurrentHealth -= Mathf.Max(damage - Defense, 0);
        float taken = (prevHealth - CurrentHealth);
        OnDamage(taken);
        return taken;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Attack attack = collision.gameObject.GetComponentInParent<Attack>();
        if (attack != null && gameObject != attack.Origin)
        {
            float damage = attack.GetDamageFor(gameObject);

            if(TakeDamage(damage) > 0)
            {
                //immunity period
                //TODO: allow specific attacks to influence immunity period
                isImmune = true;
                Invoke("EndImmunity", BaseImmunityPeriod);
                //TODO: knockback (direction??)
            }


        }
    }

    public void EndImmunity()
    {
        isImmune = false;
        OnImmunityEnd();
    }
}
