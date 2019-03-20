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

    public void CollectThis(PlayerController pl)
    {
        if (pl == null) return;

        pl.NumPotions++;

        canCollect = false;

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canCollect && other.tag.Equals("ItemZone") && other.GetComponentInParent<Equipment>() != null)
        {
            CollectThis(other.GetComponentInParent<PlayerController>());
        }
    }
}
