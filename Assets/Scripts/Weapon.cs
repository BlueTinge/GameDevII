using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //If use of onTriggerStay is too unoptimized, we can use OnTriggerEnter and adjust the scaled of ItemZone rather than enabling/disabling it
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("ItemZone"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().Equip(gameObject);
        }
    }
}
