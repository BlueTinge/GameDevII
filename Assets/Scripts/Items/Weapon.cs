using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//base class for weapons
//probably should be made abstract once we have weapon classes

[RequireComponent(typeof(Rigidbody))]
public class Weapon : MonoBehaviour
{

    public float BaseDamage;
    public float BaseKnockback;

    public bool IsHeld {get; private set;}
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
            }
            else
            {
                GetComponent<Rigidbody>().isKinematic = true;
                IsHeld = true;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("ItemZone") && other.GetComponentInParent<Equipment>() != null)
        {
            other.GetComponentInParent<Equipment>().Equip(gameObject);
        }
    }


    public void SetIsHeld(bool isHeld)
    {
        GetComponent<Rigidbody>().isKinematic = isHeld;
    }

    //ANIMATION EVENTS used for attack controlling
    //if theres a way to call these directly from a parent thatd be great
    //otherwise these methods are called from Equipment

    public void MakeLightAttack(float ttl)
    {
        gameObject.AddComponent<Attack>().Initialize(BaseDamage, BaseKnockback, ttl, Holder);
    }

    public void MakeHeavyAttack(float ttl)
    {
        gameObject.AddComponent<Attack>().Initialize(BaseDamage*2, BaseKnockback*2, ttl, Holder);
    }
}
