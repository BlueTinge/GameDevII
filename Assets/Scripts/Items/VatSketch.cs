using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatSketch : MonoBehaviour, IInteractable
{
    public bool canCollect = true;
    private GameObject GameManager;

    void awake()
    {
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
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
        GameManager.GetComponent<UIManager>().VatSketch.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canCollect && other.tag.Equals("ItemZone") && other.GetComponentInParent<PlayerController>() != null)
        {
            CollectThis(other.GetComponentInParent<PlayerController>());
        }
    }

    public bool CanInteract()
    {
        return canCollect;
    }

    public string GetInteractText()
    {
        return "Press E to view the sketch";
    }

}
