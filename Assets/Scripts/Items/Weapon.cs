using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Weapon : MonoBehaviour
{

    private bool _isHeld;
    public bool IsHeld
    {
        get { return _isHeld; }
        set
        {
            _isHeld = value;
            GetComponent<Rigidbody>().isKinematic = value;
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
}
