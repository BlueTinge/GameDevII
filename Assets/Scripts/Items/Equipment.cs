using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public GameObject CurrentWeapon;
    public Transform DomHand;
    public GameObject ItemZone;

    // Start is called before the first frame update
    void Start()
    {
        CurrentWeapon.GetComponent<Weapon>().Holder = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Equip(GameObject newWeapon)
    {
        if (newWeapon.GetComponent<Weapon>().IsHeld) return;

        UnityEngine.Debug.Log("Equipped weapon");

        CurrentWeapon.transform.parent = newWeapon.transform.parent;
        CurrentWeapon.transform.position = new Vector3(CurrentWeapon.transform.position.x, CurrentWeapon.transform.position.y, CurrentWeapon.transform.position.z + 0.2f);
        //CurrentWeapon.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
        CurrentWeapon.GetComponent<Weapon>().Holder = null;

        newWeapon.transform.parent = DomHand;
        newWeapon.transform.localPosition = new Vector3(0, 0, 0);
        newWeapon.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        newWeapon.GetComponent<Weapon>().Holder = gameObject;

        CurrentWeapon = newWeapon;

        if(ItemZone != null)
        {
            ItemZone.GetComponent<Collider>().enabled = false;
        }
    }
}
