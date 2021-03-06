﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//base class for weapons
//probably should be made abstract once we have weapon classes

[RequireComponent(typeof(Rigidbody))]
public class Weapon : MonoBehaviour, IInteractable
{

    public AudioSource audio;
    public AudioClip swing;

    public float BaseDamage;
    public float BaseKnockback;

    public bool IsHeld {get; private set;}

    public string WeaponName;

    [Tooltip("True if the weapon should use physics system to collide with stuff when held")]
    public bool IsPhysical = true;

    private GameObject _holder;
    public GameObject Holder
    {
        get { return _holder; }
        set
        {
            _holder = value;
            if (_holder == null)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                IsHeld = false;

                SetCollidersEnabled(true);

                if (!IsPhysical) foreach (Collider c in GetComponentsInChildren<Collider>())
                {
                    c.isTrigger = false;
                }
            }
            else
            {
                GetComponent<Rigidbody>().isKinematic = true;
                IsHeld = true;

                SetCollidersEnabled(false);

                if (!IsPhysical) foreach (Collider c in GetComponentsInChildren<Collider>())
                {
                    c.isTrigger = true;
                    //note: this may need to be expanded upon but seems fine for right now
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RecieveAttack(Attack attack)
    {
        //if (attack != null) Holder?.GetComponent<HealthStats>()?.RecieveAttack(attack); //this line disables weapon blocking
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("ItemZone") && other.GetComponentInParent<Equipment>() != null)
        {
            other.GetComponentInParent<Equipment>().Equip(gameObject);
            GetComponent<DisplaysInteractText>()?.ClearText();
        }

        RecieveAttack(other.gameObject.GetComponentInParent<Attack>());
    }

    private void OnCollisionEnter(Collision other)
    {
        RecieveAttack(other.gameObject.GetComponentInParent<Attack>());
    }

    public void DisableColliders()
    {
        SetCollidersEnabled(false);
    }

    public void SetCollidersEnabled(bool enabled)
    {
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = enabled;
        }
    }

    //ANIMATION EVENTS used for attack controlling
    //if theres a way to call these directly from a parent thatd be great
    //otherwise these methods are called from Equipment

    public void MakeLightAttack(float ttl, bool isSecondSwing)
    {
        //TODO: use rotation instead of velocity somehow
        gameObject.AddComponent<Attack>().Initialize(BaseDamage, (Holder.GetComponent<Rigidbody>().rotation * Vector3.forward) * BaseKnockback, ttl, Holder);
        if (isSecondSwing) gameObject.GetComponent<Attack>().SecondLightSwing = true;
        if (!isSecondSwing) audio.Play();
        //else audio.Play();

        SetCollidersEnabled(true);
        Invoke("DisableColliders", ttl);
    }

    public void MakeHeavyAttack(float ttl)
    {
        gameObject.AddComponent<Attack>().Initialize(BaseDamage*2, (Holder.GetComponent<Rigidbody>().rotation * Vector3.forward) * BaseKnockback * 3, ttl, Holder);
        audio.Play();

        SetCollidersEnabled(true);
        Invoke("DisableColliders", ttl);
    }

    public string GetInteractText()
    {
        return "Press E(Keyboard)/ X(Controller) to pick up " + WeaponName;
    }

    public bool CanInteract()
    {
        return (Holder == null);
    }
}
