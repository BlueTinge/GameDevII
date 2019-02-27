using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public int MenuState = 0;

    public Transform PauseMenu;
    public Transform JournalMenu;
    public Transform Journal1Button;
    public Transform JournalBackButton;

    public GameObject Player;
    

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (PauseMenu.gameObject.activeInHierarchy == false & MenuState == 0)
            {
                PauseMenu.gameObject.SetActive(true);
                Time.timeScale = 0f;
                MenuState += 1;
            }

            else if (MenuState == 1 & PauseMenu.gameObject.activeInHierarchy == true)
            {
                PauseMenu.gameObject.SetActive(false);
                Time.timeScale = 1f;
                MenuState -= 1;
            }

            else if (PauseMenu.gameObject.activeInHierarchy == false & MenuState == 2)
            {
                JournalMenu.gameObject.SetActive(false);
                PauseMenu.gameObject.SetActive(true);
                MenuState -= 1;
            }

            else if (PauseMenu.gameObject.activeInHierarchy == false & MenuState == 3)
            {
                Player.GetComponent<PlayerController>().img.gameObject.SetActive(false);
                Journal1Button.gameObject.SetActive(true);
                JournalBackButton.gameObject.SetActive(false);
                MenuState -= 1;
            }

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
        MenuState -= 1;
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
        MenuState += 1;
        if (Player.GetComponent<PlayerController>().JournalColllect1 == true)
        {
            Journal1Button.gameObject.SetActive(true);
        }

    }

    public void Journal1()
    {
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        MenuState += 1;
    }

    public void Back()
    {
        JournalMenu.gameObject.SetActive(false);
        PauseMenu.gameObject.SetActive(true);
        MenuState -= 1;
    }

    public void JournalBack()
    {
        if (Player.GetComponent<PlayerController>().img == true)
        {
            Player.GetComponent<PlayerController>().img.gameObject.SetActive(false);
            Journal1Button.gameObject.SetActive(true);
            JournalBackButton.gameObject.SetActive(false);
            MenuState -= 1;
        }
    }

    public void Pressed()
    {
        Debug.Log("I am pressed");
    }
}
