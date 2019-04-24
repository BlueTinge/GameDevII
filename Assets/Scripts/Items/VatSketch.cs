using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatSketch : MonoBehaviour, IInteractable
{
    public bool canCollect = true;
    private GameObject GameManager;

    void Awake()
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
        GameManager.GetComponent<UIManager>().VatSketch.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canCollect && other.tag.Equals("ItemZone") && other.GetComponentInParent<PlayerController>() != null)
        {
            CollectThis(other.GetComponentInParent<PlayerController>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        UnityEngine.Debug.Log("left trigger");
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            GameManager.GetComponent<UIManager>().VatSketch.SetActive(false);
        }
    }

    public bool CanInteract()
    {
        return canCollect;
    }

    public string GetInteractText()
    {
        return "Press E(Keyboard)/ X(Controller) to view the sketch";
    }

}
