using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public GameObject myEventSystem;
    public GameObject GameButton;

    // Start is called before the first frame update
    void Start()
    {
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(GameButton);
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Quit()
    {
        Application.Quit();
    }

    public void StartButton()
    {
        UIManager.ResetJournals();
        Manager.Reset(); //to reload the checkpoints, etc.
        SceneManager.LoadScene(1);
    }
}
