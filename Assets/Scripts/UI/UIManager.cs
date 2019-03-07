using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager ActiveUIManager;

    public int MenuState = 0;

    public Transform PauseMenu;
    public Transform JournalMenu;
    public Transform Journal1Button;
    public Transform Journal2Button;
    public Transform Journal3Button;
    public Transform Journal4Button;
    public Transform Journal5Button;

    public Transform JournalBackButton;

    public GameObject Player;

    public TextAsset Journal1Data;
    public TextAsset Journal2Data;
    public TextAsset Journal3Data;
    public TextAsset Journal4Data;
    public TextAsset Journal5Data;


    public static bool isInputEnabled = true;

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
                StartCoroutine(SetInputActive());
            }

            else if (MenuState == 1 & PauseMenu.gameObject.activeInHierarchy == true)
            {
                PauseMenu.gameObject.SetActive(false);
                Time.timeScale = 1f;
                MenuState -= 1;
                isInputEnabled = false;
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
                Journal2Button.gameObject.SetActive(true);
                Journal3Button.gameObject.SetActive(true);
                Journal4Button.gameObject.SetActive(true);
                Journal5Button.gameObject.SetActive(true);
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
        StartCoroutine(SetInputActive());
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
        if (Player.GetComponent<PlayerController>().JournalColllect2 == true)
        {
            Journal2Button.gameObject.SetActive(true);
        }
        if (Player.GetComponent<PlayerController>().JournalColllect3 == true)
        {
            Journal3Button.gameObject.SetActive(true);
        }
        if (Player.GetComponent<PlayerController>().JournalColllect4 == true)
        {
            Journal4Button.gameObject.SetActive(true);
        }
        if (Player.GetComponent<PlayerController>().JournalColllect5 == true)
        {
            Journal5Button.gameObject.SetActive(true);
        }

    }

    public void Journal1Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal1Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        MenuState += 1;
    }

    public void Journal2Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal2Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        MenuState += 1;
    }

    public void Journal3Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal3Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        MenuState += 1;
    }

    public void Journal4Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal4Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        MenuState += 1;
    }

    public void Journal5Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal5Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
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
            Journal2Button.gameObject.SetActive(true);
            Journal3Button.gameObject.SetActive(true);
            Journal4Button.gameObject.SetActive(true);
            Journal5Button.gameObject.SetActive(true);
            JournalBackButton.gameObject.SetActive(false);
            MenuState -= 1;
        }
    }

    public void Pressed()
    {
        Debug.Log("I am pressed");
    }

    private IEnumerator SetInputActive()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        isInputEnabled = true;
        Debug.Log(isInputEnabled);
    }
}
