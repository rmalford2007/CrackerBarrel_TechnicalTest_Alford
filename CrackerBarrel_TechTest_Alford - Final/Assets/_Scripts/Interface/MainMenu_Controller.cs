using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu_Controller : MonoBehaviour {

    public static MainMenu_Controller Instance;
    [Header("Main Menu Scene")]
    [Tooltip("The main menu background color. Only shown in the Default_Menu scene and not during play.")]
    public GameObject mainBackground;


    [Header("Main Navigation")]
    [Tooltip("This is the panel that holds panels for title, author, instructions, game setup, and more.")]
    public GameObject mainPanel;

    [Tooltip("This is the panel that provides the main navigation when the mainPanel is active.")]
    public GameObject mainNavigationPanel;

    [Tooltip("The main menu navigation play button. This is hidden when paused during play.")]
    public GameObject playButtonGameObject;

    [Tooltip("This is the panel that provides instructions to the player.")]
    public GameObject instructionsPanel;

    [Tooltip("This is where the game board settings are set.")]
    public GameObject gameSetupPanel;

    [Tooltip("The slider control for the size of the board. Only for initialization of what it starts with.")]
    public Slider boardSizeSlider;

    [Tooltip("This is the peg count text next to the slider control for board size.")]
    public TMP_Text boardPegCountText;

    [Tooltip("This controls the default size of a board, and holds the coloring information and styles for a specific board type. This will hold the currently selected preset, when presets are added so the player can choose. For now its up to the designer to set this.")]
    public BoardPreset defaultPreset;


    [Header("Play Mode Menus")]

    [Tooltip("This is the panel that is shown while the player is playing the game.")]
    public GameObject activePlayPanel;

    [Tooltip("This is the panel shown during a pause from play mode.")]
    public GameObject pausePanel;
    
    [Tooltip("The button to resume play from a pause state during play.")]
    public GameObject resumeButtonGameObject;

    [Tooltip("This is the text at the top of the pause screen. Show either paused, or game over text.")]
    public TMP_Text statusText;

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

            if(defaultPreset != null)
            {
                //Create an instance of the defaultPreset so changes do not persist through game closes
                defaultPreset = Instantiate(defaultPreset); 
            }

            if(boardSizeSlider != null && defaultPreset != null)
            {
                //Initialize the slider data
                boardSizeSlider.value = defaultPreset.baseRowPegCount;
            }

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

    /// <summary>
    /// First menu screen. 
    /// </summary>
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

    /// <summary>
    /// Hide the main panels.
    /// </summary>
    public void HideMenu()
    {
        pausePanel.SetActive(false);
        mainPanel.SetActive(false);
        mainBackground.SetActive(false);
        activePlayPanel.SetActive(true);
    }

    /// <summary>
    /// When play is clicked, move to game setup panel
    /// </summary>
    public void OnPlayClicked()
    {
        instructionsPanel.SetActive(false);
        mainNavigationPanel.SetActive(false);
        gameSetupPanel.SetActive(true);
    }

    /// <summary>
    /// Load the play scene.
    /// </summary>
    public void OnCreateBoard()
    {
        SceneManager.LoadScene("Default_Play");
    }

    /// <summary>
    /// Hook for resetting the gameboard
    /// </summary>
    public void OnResetClicked()
    {
        GameManager.OnResetBoard_Static();
        TogglePause();
    }

    /// <summary>
    /// When the exit button is clicked. We close the game if in the main menu scene. If in play scene, we load the main menu scene.
    /// </summary>
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
            if (defaultPreset != null)
                defaultPreset.baseRowPegCount = pegCount;
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
