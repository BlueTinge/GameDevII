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
    public GameObject ContinueButton;
    public GameObject BackButton;
    public GameObject Journal1Button;
    public GameObject Journal2Button;
    public GameObject Journal3Button;
    public GameObject Journal4Button;
    public GameObject Journal5Button;
    public GameObject Journal6Button;
    public GameObject Journal7Button;
    public GameObject Journal8Button;
    public GameObject Journal9Button;
    public GameObject Journal10Button;
    public GameObject VatSketch;
    public GameObject JournalBackground;
    public GameObject JournalBackButton;
    public int JournalNum;

    public GameObject Player;

    public Text Potions;
    private int NumPotions;

    public TextAsset Journal1Data;
    public TextAsset Journal2Data;
    public TextAsset Journal3Data;
    public TextAsset Journal4Data;
    public TextAsset Journal5Data;
    public TextAsset Journal6Data;
    public TextAsset Journal7Data;
    public TextAsset Journal8Data;
    public TextAsset Journal9Data;
    public TextAsset Journal10Data;

    public static readonly int NumJournals = 10;
    public static bool[] IsJournalCollected = new bool[NumJournals];
    private GameObject myEventSystem;

    public static bool isInputEnabled = true;

    void Start()
    {
        myEventSystem = GameObject.Find("EventSystem");
        //myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().firstSelectedGameObject = ContinueButton;
        //myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(ContinueButton);
        NumPotions = Player.GetComponent<PlayerController>().NumPotions;
    }

    void Update()
    {
        NumPotions = Player.GetComponent<PlayerController>().NumPotions;
        Potions.text = "Potions: " + NumPotions.ToString();
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Back"))
        {
            if (PauseMenu.gameObject.activeInHierarchy == false & MenuState == 0)
            {
                PauseMenu.gameObject.SetActive(true);
                //Player.GetComponent<PlayerController>().img.text = "";
                myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
                myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(ContinueButton);
                Time.timeScale = 0f;
                MenuState += 1;
                StartCoroutine(SetInputActive());
            }

            else if (MenuState == 1 & PauseMenu.gameObject.activeInHierarchy == true)
            {
                PauseMenu.gameObject.SetActive(false);
                Time.timeScale = 1f;
                MenuState -= 1;
                isInputEnabled = true;
            }

            else if (PauseMenu.gameObject.activeInHierarchy == false & MenuState == 2)
            {
                JournalMenu.gameObject.SetActive(false);
                PauseMenu.gameObject.SetActive(true);
                myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
                myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(ContinueButton);
                MenuState -= 1;
            }

            else if (PauseMenu.gameObject.activeInHierarchy == false & MenuState == 3)
            {
                Player.GetComponent<PlayerController>().img.gameObject.SetActive(false);
                JournalMenu.gameObject.SetActive(false);
                JournalMenu.gameObject.SetActive(true);
                myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
                myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(GetLastButton(JournalNum));
                JournalBackground.gameObject.SetActive(false);
                if (IsJournalCollected[0] == true)
                {
                    Journal1Button.gameObject.SetActive(true);
                }
                if (IsJournalCollected[1] == true)
                {
                    Journal2Button.gameObject.SetActive(true);
                }
                if (IsJournalCollected[2] == true)
                {
                    Journal3Button.gameObject.SetActive(true);
                }
                if (IsJournalCollected[3] == true)
                {
                    Journal4Button.gameObject.SetActive(true);
                }
                if (IsJournalCollected[4] == true)
                {
                    Journal5Button.gameObject.SetActive(true);
                }
                if (IsJournalCollected[5] == true)
                {
                    Journal6Button.gameObject.SetActive(true);
                }
                if (IsJournalCollected[6] == true)
                {
                    Journal7Button.gameObject.SetActive(true);
                }
                if (IsJournalCollected[7] == true)
                {
                    Journal8Button.gameObject.SetActive(true);
                }
                if (IsJournalCollected[8] == true)
                {

                    Journal9Button.gameObject.SetActive(true);
                }
                if (IsJournalCollected[9] == true)
                {
                    Journal10Button.gameObject.SetActive(true);
                }
                JournalBackButton.gameObject.SetActive(false);
                MenuState -= 1;
            }

        }
    }

    void OnGUI()
    {
        if (MenuState != 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

            
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
        MenuState -= 1;
        StartCoroutine(SetInputActive());
    }

    public void Restart()
    {
        PauseMenu.gameObject.SetActive(false);
        MenuState -= 1;
        isInputEnabled = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void JournalButton()
    {
        PauseMenu.gameObject.SetActive(false);
        JournalMenu.gameObject.SetActive(true);
        MenuState += 1;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(BackButton);
        if (IsJournalCollected[0] == true)
        {
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(Journal1Button);
            Journal1Button.gameObject.SetActive(true);

        }
        if (IsJournalCollected[1] == true)
        {
            Journal2Button.gameObject.SetActive(true);
        }
        if (IsJournalCollected[2] == true)
        {
            Journal3Button.gameObject.SetActive(true);
        }
        if (IsJournalCollected[3] == true)
        {
            Journal4Button.gameObject.SetActive(true);
        }
        if (IsJournalCollected[4] == true)
        {
            Journal5Button.gameObject.SetActive(true);
        }
        if (IsJournalCollected[5] == true)
        {
            Journal6Button.gameObject.SetActive(true);
        }
        if (IsJournalCollected[6] == true)
        {
            Journal7Button.gameObject.SetActive(true);
        }
        if (IsJournalCollected[7] == true)
        {
            Journal8Button.gameObject.SetActive(true);
        }
        if(IsJournalCollected[8] == true)
        {

            Journal9Button.gameObject.SetActive(true);
        }
        if(IsJournalCollected[9] == true)
        {
            Journal10Button.gameObject.SetActive(true);
        }
    }

    public void Journal1Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal1Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        JournalBackground.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        Journal6Button.gameObject.SetActive(false);
        Journal7Button.gameObject.SetActive(false);
        Journal8Button.gameObject.SetActive(false);
        Journal9Button.gameObject.SetActive(false);
        Journal10Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        JournalNum = 1;
        MenuState += 1;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(JournalBackButton);
    }

    public void Journal2Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal2Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        JournalBackground.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        Journal6Button.gameObject.SetActive(false);
        Journal7Button.gameObject.SetActive(false);
        Journal8Button.gameObject.SetActive(false);
        Journal9Button.gameObject.SetActive(false);
        Journal10Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        JournalNum = 2;
        MenuState += 1;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(JournalBackButton);
    }

    public void Journal3Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal3Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        JournalBackground.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        Journal6Button.gameObject.SetActive(false);
        Journal7Button.gameObject.SetActive(false);
        Journal8Button.gameObject.SetActive(false);
        Journal9Button.gameObject.SetActive(false);
        Journal10Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        JournalNum = 3;
        MenuState += 1;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(JournalBackButton);
    }

    public void Journal4Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal4Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        JournalBackground.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        Journal6Button.gameObject.SetActive(false);
        Journal7Button.gameObject.SetActive(false);
        Journal8Button.gameObject.SetActive(false);
        Journal9Button.gameObject.SetActive(false);
        Journal10Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        JournalNum = 4;
        MenuState += 1;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(JournalBackButton);
    }

    public void Journal5Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal5Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        JournalBackground.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        Journal6Button.gameObject.SetActive(false);
        Journal7Button.gameObject.SetActive(false);
        Journal8Button.gameObject.SetActive(false);
        Journal9Button.gameObject.SetActive(false);
        Journal10Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        JournalNum = 5;
        MenuState += 1;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(JournalBackButton);
    }

    public void Journal6Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal6Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        JournalBackground.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        Journal6Button.gameObject.SetActive(false);
        Journal7Button.gameObject.SetActive(false);
        Journal8Button.gameObject.SetActive(false);
        Journal9Button.gameObject.SetActive(false);
        Journal10Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        JournalNum = 6;
        MenuState += 1;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(JournalBackButton);
    }
    public void Journal7Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal7Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        JournalBackground.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        Journal6Button.gameObject.SetActive(false);
        Journal7Button.gameObject.SetActive(false);
        Journal8Button.gameObject.SetActive(false);
        Journal9Button.gameObject.SetActive(false);
        Journal10Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        JournalNum = 7;
        MenuState += 1;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(JournalBackButton);
    }
    public void Journal8Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal8Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        JournalBackground.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        Journal6Button.gameObject.SetActive(false);
        Journal7Button.gameObject.SetActive(false);
        Journal8Button.gameObject.SetActive(false);
        Journal9Button.gameObject.SetActive(false);
        Journal10Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        JournalNum = 8;
        MenuState += 1;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(JournalBackButton);
    }
    public void Journal9Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal9Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        JournalBackground.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        Journal6Button.gameObject.SetActive(false);
        Journal7Button.gameObject.SetActive(false);
        Journal8Button.gameObject.SetActive(false);
        Journal9Button.gameObject.SetActive(false);
        Journal10Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        JournalNum = 9;
        MenuState += 1;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(JournalBackButton);
    }
    public void Journal10Menu()
    {
        Player.GetComponent<PlayerController>().img.text = Journal10Data.ToString();
        Player.GetComponent<PlayerController>().img.gameObject.SetActive(true);
        JournalBackground.gameObject.SetActive(true);
        Journal1Button.gameObject.SetActive(false);
        Journal2Button.gameObject.SetActive(false);
        Journal3Button.gameObject.SetActive(false);
        Journal4Button.gameObject.SetActive(false);
        Journal5Button.gameObject.SetActive(false);
        Journal6Button.gameObject.SetActive(false);
        Journal7Button.gameObject.SetActive(false);
        Journal8Button.gameObject.SetActive(false);
        Journal9Button.gameObject.SetActive(false);
        Journal10Button.gameObject.SetActive(false);
        JournalBackButton.gameObject.SetActive(true);
        JournalNum = 10;
        MenuState += 1;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(JournalBackButton);
    }
    public void Back()
    {
        JournalMenu.gameObject.SetActive(false);
        PauseMenu.gameObject.SetActive(true);
        MenuState -= 1;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(ContinueButton);
    }

    public void JournalBack()
    {
        if (Player.GetComponent<PlayerController>().img == true)
        {
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(GetLastButton(JournalNum));
            Player.GetComponent<PlayerController>().img.gameObject.SetActive(false);
            JournalBackground.gameObject.SetActive(false);
            if (IsJournalCollected[0] == true)
            {
                myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
                myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(Journal1Button);
                Journal1Button.gameObject.SetActive(true);

            }
            if (IsJournalCollected[1] == true)
            {
                Journal2Button.gameObject.SetActive(true);
            }
            if (IsJournalCollected[2] == true)
            {
                Journal3Button.gameObject.SetActive(true);
            }
            if (IsJournalCollected[3] == true)
            {
                Journal4Button.gameObject.SetActive(true);
            }
            if (IsJournalCollected[4] == true)
            {
                Journal5Button.gameObject.SetActive(true);
            }
            if (IsJournalCollected[5] == true)
            {
                Journal6Button.gameObject.SetActive(true);
            }
            if (IsJournalCollected[6] == true)
            {
                Journal7Button.gameObject.SetActive(true);
            }
            if (IsJournalCollected[7] == true)
            {
                Journal8Button.gameObject.SetActive(true);
            }
            if (IsJournalCollected[8] == true)
            {

                Journal9Button.gameObject.SetActive(true);
            }
            if (IsJournalCollected[9] == true)
            {
                Journal10Button.gameObject.SetActive(true);
            }
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

    public static void ResetJournals()
    {
        IsJournalCollected = new bool[NumJournals];
    }

    public string GetJournalNum(int JournalNum)
    {
        switch (JournalNum)
        {
            case 0:
                return Journal1Data.ToString();
            case 1:
                return Journal2Data.ToString();
            case 2:
                return Journal3Data.ToString();
            case 3:
                return Journal4Data.ToString();
            case 4:
                return Journal5Data.ToString();
            case 5:
                return Journal6Data.ToString();
            case 6:
                return Journal7Data.ToString();
            case 7:
                return Journal8Data.ToString();
            case 8:
                return Journal9Data.ToString();
            case 9:
                return Journal10Data.ToString();
            default:
                return null;
        }
    }
    public GameObject GetLastButton(int JournalNum)
    {
        switch (JournalNum)
        {
            case 1:
                return Journal1Button;
            case 2:
                return Journal2Button;
            case 3:
                return Journal3Button;
            case 4:
                return Journal4Button;
            case 5:
                return Journal5Button;
            case 6:
                return Journal6Button;
            case 7:
                return Journal7Button;
            case 8:
                return Journal8Button;
            case 9:
                return Journal9Button;
            case 10:
                return Journal10Button;
            default:
                return BackButton;
        }
    }
}
