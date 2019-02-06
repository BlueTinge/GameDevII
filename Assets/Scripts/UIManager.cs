using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    bool paused = false;
    public Transform Canvas;

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            if (Canvas.gameObject.activeInHierarchy == false)
            {
                Canvas.gameObject.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                Canvas.gameObject.SetActive(false);
                Time.timeScale = 1f;
            }
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    public void Continue()
    {
        Canvas.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
