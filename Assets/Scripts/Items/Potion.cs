using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{

    public bool canCollect = true;

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
        if (canCollect && other.tag.Equals("ItemZone") && other.GetComponentInParent<Equipment>() != null)
        {
            canCollect = false;
            other.GetComponentInParent<PlayerController>().NumPotions++;

        }
    }
}
