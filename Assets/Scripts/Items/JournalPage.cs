using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalPage : MonoBehaviour
{

    public int JournalNum;
    public bool canCollect = true;
    public float rotationSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0,rotationSpeed,0));
    }

    public void CollectThis(PlayerController pl)
    {
        UIManager.IsJournalCollected[JournalNum] = true;

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
