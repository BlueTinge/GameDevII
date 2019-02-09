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
    public delegate void KnockbackDelegate(Vector3 knockback);
    public KnockbackDelegate OnKnockback;
    public delegate void ImmunityEndDelegate();
    public ImmunityEndDelegate OnImmunityEnd;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = MaxHealth;

        OnDeath = delegate (float damage) { UnityEngine.Debug.Log("OnDeath not set"); };
        OnDamage = delegate (float damage) { UnityEngine.Debug.Log("OnDamage not set"); };
        OnImmunityEnd = delegate () { UnityEngine.Debug.Log("OnImmunityEnd not set"); };
        OnKnockback = delegate (Vector3 knockback)
        {
            Rigidbody rb= gameObject.GetComponent<Rigidbody>();
            if (rb != null) rb.AddForce(knockback);
            UnityEngine.Debug.Log(knockback);
        };
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

    private void OnTriggerEnter(Collider other)
    {
        Attack attack = other.gameObject.GetComponentInParent<Attack>();
        if (attack != null && gameObject != attack.Origin)
        {
            float damage = attack.GetDamageFor(gameObject);

            if(TakeDamage(damage) > 0)
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

    private void OnCollisionEnter(Collision other)
    {
        OnTriggerEnter(other.collider);
    }

    public void EndImmunity()
    {
        isImmune = false;
        OnImmunityEnd();
    }
}
