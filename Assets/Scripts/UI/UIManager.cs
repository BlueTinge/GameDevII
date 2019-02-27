using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //bool paused = false;

    public Transform PauseMenu;
    public Transform JournalMenu;
    public Transform Journal1Button;

    public GameObject Player;
    

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            if (PauseMenu.gameObject.activeInHierarchy == false)
            {
                PauseMenu.gameObject.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                PauseMenu.gameObject.SetActive(false);
                Time.timeScale = 1f;
            }
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    public void Continue()
    {
        PauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("I am Continuing");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void JournalButton()
    {
        PauseMenu.gameObject.SetActive(false);
        JournalMenu.gameObject.SetActive(true);
        
        if (Player.GetComponent<PlayerController>().JournalColllect1 == true)
        {
            Journal1Button.gameObject.SetActive(true);
        }

    }

    public void Journal1()
    {
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
    }
    public void Pressed()
    {
        Debug.Log("I am pressed");
    }
}
