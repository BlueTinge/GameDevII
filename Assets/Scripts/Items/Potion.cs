using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour, IInteractable
{

    public bool canCollect = true;
    private GameObject Img;


    void Awake()
    {
        Img = GameObject.FindGameObjectWithTag("Img");
    }
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

        Img.SetActive(false);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canCollect && other.tag.Equals("ItemZone") && other.GetComponentInParent<PlayerController>() != null)
        {
            CollectThis(other.GetComponentInParent<PlayerController>());
        }
    }

    string IInteractable.GetInteractText()
    {
        return "Press E(Keyboard)/ X(Controller) to pick up potion";
    }

    bool IInteractable.CanInteract()
    {
        return canCollect;
    }
}
