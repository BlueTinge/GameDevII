using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string GetInteractText();

    bool CanInteract();
}

public class DisplaysInteractText : MonoBehaviour
{
    public IInteractable interactable;
    private GameObject Player;

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if(interactable == null)
        {
            interactable = GetComponent<IInteractable>();
            if(interactable == null)
            {
                Debug.LogWarning("This GameObject does not have an IInteractable attached.");
            }
        }
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (interactable.CanInteract() && other.tag.Equals("ItemZoneArea") && Player.GetComponent<PlayerController>().State != PlayerState.DEATH)
        {
            Player.GetComponent<PlayerController>().img.text = interactable.GetInteractText();
            Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        }
    }

    public void ClearText()
    {
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("ItemZoneArea"))
        {
            ClearText();
        }
    }
}
