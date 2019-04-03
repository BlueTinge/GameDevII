using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public GameObject myEventSystem;
    public GameObject GameButton;

    // Start is called before the first frame update
    void Start()
    {
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(GameButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MainMenu()
    {
        Manager.Reset(); //to reload the checkpoints, etc.
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
