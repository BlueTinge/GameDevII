using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalPage : MonoBehaviour, IInteractable
{

    public int JournalNum;
    public bool canCollect = true;
    public float rotationSpeed = 1f;
    private GameObject Img;
    private GameObject Player;
    private GameObject GameManager;
    private GameObject myEventSystem;



    void Awake()
    {
        Img = GameObject.FindGameObjectWithTag("Img");
        Player = GameObject.FindGameObjectWithTag("Player");
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
        myEventSystem = GameObject.Find("EventSystem");
    }
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

        Img.SetActive(false);

        Destroy(gameObject);

        Time.timeScale = 0f;
        Player.GetComponent<PlayerController>().img.text = GameManager.GetComponent<UIManager>().GetJournalNum(JournalNum);
        GameManager.GetComponent<UIManager>().JournalMenu.gameObject.SetActive(true);
        GameManager.GetComponent<UIManager>().MenuState = 3;
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        GameManager.GetComponent<UIManager>().JournalBackButton.gameObject.SetActive(true);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(GameManager.GetComponent<UIManager>().JournalBackButton);

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
        return "Press E to pick up Journal";
    }

    bool IInteractable.CanInteract()
    {
        return canCollect;
    }
}
