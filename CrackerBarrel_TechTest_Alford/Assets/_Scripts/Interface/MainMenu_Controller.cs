using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Controller : MonoBehaviour {

    public GameObject mainPanel;
    public GameObject pausePanel;
    public GameObject playButtonGameObject;
    public GameObject mainBackground;

	// Use this for initialization
	void Start () {
        if (SceneManager.GetActiveScene().name == "Default_Menu")
            OnMainMenu();
        else
            HideMenu();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPauseGame()
    {
        pausePanel.SetActive(true);
        mainPanel.SetActive(true);
        playButtonGameObject.SetActive(false);
        mainBackground.SetActive(false);
    }

    public void OnMainMenu()
    {
        pausePanel.SetActive(false);
        mainPanel.SetActive(true);
        playButtonGameObject.SetActive(true);
        mainBackground.SetActive(true);
    }

    public void HideMenu()
    {
        pausePanel.SetActive(false);
        mainPanel.SetActive(false);
        mainBackground.SetActive(false);
    }

    public void OnPlayClicked()
    {
        SceneManager.LoadScene("Default_Play");
    }

    public void OnResetClicked()
    {

    }

    public void OnExitClicked()
    {
        if(SceneManager.GetActiveScene().name == "Default_Menu")
            QuitGame();
        else
        {
            SceneManager.LoadScene("Default_Menu");
        }
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
