using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu_Controller : MonoBehaviour {

    public static MainMenu_Controller Instance;

    public GameObject mainPanel;
    public GameObject pausePanel;
    public GameObject playButtonGameObject;
    public GameObject mainBackground;
    public GameObject resumeButtonGameObject;
    public GameObject activePlayPanel;
    public GameObject instructionsPanel;
    public GameObject mainNavigationPanel;
    public GameObject gameSetupPanel;
    public TMP_Text  statusText;
    public TMP_Text boardPegCountText;

    bool isPaused = false;
    float saveTimeScale = 1f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;

            //We only want 1 menu to be sitting around during play, if we move between menu and start a bunch, be sure there is always one
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            //if we move between menu and start a bunch, be sure there is always one
            //So delete this extra one, he is newer
            DestroyImmediate(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        
	}

    public void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadMode)
    {
        if (SceneManager.GetActiveScene().name == "Default_Menu")
        {
            OnMainMenu();
        }
        else
        {
            HideMenu();
        }
    }

    public void TogglePause(string statusUpdate="")
    {
        if (isPaused)
            UnpauseGame();
        else
            PauseGame(statusUpdate);
    }

    public void TogglePause_GameOver(string statusUpdate)
    {
        PauseGame(statusUpdate, false);
    }

    private void PauseGame(string statusUpdate, bool allowResume=true)
    {
        isPaused = true;

        //Store the current timescale being used
        saveTimeScale = Time.timeScale;

        //Pause time - NOTE any interface effects need to use unscaled time or they will be affected by this as well
        Time.timeScale = 0f;

        //Sync status text
        if (statusUpdate == "")
            statusUpdate = "Game Paused";
        statusText.text = statusUpdate;

        //Set Visibility
        pausePanel.SetActive(true);
        mainPanel.SetActive(true);
        mainNavigationPanel.SetActive(true);
        gameSetupPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        playButtonGameObject.SetActive(false);
        resumeButtonGameObject.SetActive(allowResume);
        mainBackground.SetActive(false);
        activePlayPanel.SetActive(false);
    }

    public void OnToggleInstructions()
    {
        mainNavigationPanel.SetActive(!mainNavigationPanel.activeSelf);
        instructionsPanel.SetActive(!mainNavigationPanel.activeSelf);

        if (SceneManager.GetActiveScene().name != "Default_Menu")
        {
            //When not in the menu, we also need to toggle status text, reset and resume button which is in the pausePanel
            pausePanel.SetActive(!pausePanel.activeSelf);
        }
    }

    private void UnpauseGame()
    {
        HideMenu();

        isPaused = false;

        //Restore the used timescale
        Time.timeScale = saveTimeScale;
    }

    public void OnMainMenu()
    {
        pausePanel.SetActive(false);
        mainPanel.SetActive(true);
        mainNavigationPanel.SetActive(true);
        gameSetupPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        playButtonGameObject.SetActive(true);
        mainBackground.SetActive(true);
        activePlayPanel.SetActive(false);
    }

    public void HideMenu()
    {
        pausePanel.SetActive(false);
        mainPanel.SetActive(false);
        mainBackground.SetActive(false);
        activePlayPanel.SetActive(true);
    }

    public void OnPlayClicked()
    {
        instructionsPanel.SetActive(false);
        mainNavigationPanel.SetActive(false);
        gameSetupPanel.SetActive(true);
    }

    public void OnCreateBoard()
    {
        SceneManager.LoadScene("Default_Play");
    }

    public void OnResetClicked()
    {
        GameManager.OnResetBoard_Static();
        TogglePause();
    }

    public void OnExitClicked()
    {
        if(SceneManager.GetActiveScene().name == "Default_Menu")
            QuitGame();
        else
        {
            UnpauseGame();
            SceneManager.LoadScene("Default_Menu");
        }
    }

    public static bool GetPause()
    {
        if (Instance != null)
            return Instance.isPaused;
        return false;
    }

    /// <summary>
    /// This is an interface function for setting the text of the peg count according to the slider position being changed. 
    /// Proper use is to add this function to the OnValueChanged listener of a Unity Slider object, then drag the slider into the parameter field.
    /// </summary>
    /// <param name="changingSlider">The slider the text is linked to.</param>
    public void SetBoardSizeBySlider(Slider changingSlider)
    {
        if (boardPegCountText != null && changingSlider != null)
        {
            //to get the board peg count use the nth triangle number formula (n squared + n) / 2
            int pegCount = 0;
            if (changingSlider.wholeNumbers)
                pegCount = (int)(changingSlider.value);
            else
            {
                //slider is on floats, round it first
                pegCount = Mathf.RoundToInt(changingSlider.value);
            }
            pegCount = (int)((Mathf.Pow(pegCount, 2) + pegCount) / 2f);
            boardPegCountText.text = pegCount.ToString() + " Pegs";
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
