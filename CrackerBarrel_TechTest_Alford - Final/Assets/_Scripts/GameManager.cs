using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TransformFloatEvent : UnityEvent<Transform, float> { }
/// <summary>
/// Singleton - Controls the start and stop events for the game. Holds the controls to show / hide / navigate the menu during play mode. 
/// </summary>
public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [Tooltip("The prefab for a board that needs to be created.")]
    public GameObject boardPrefab;
    [Tooltip("The mouse drag prefab that is created when drag and drop interaction is enabled.")]
    public GameObject mouseObjectPrefab;

    [Tooltip("The event that occurs right after a board is created. Passes the size of 1 side of the board with it.")]
    public TransformFloatEvent BoardCreated;

    private BoardPreset chosenBoardPreset;
    private GameObject activeBoardObject;
    private GameBoard activeBoardScript;

    private bool isGameOver = false;
    public bool useDragDrop = true;
    private void Awake()
    {
        if(Instance == null)
        {
            //First instance made stays
            Instance = this;
        }
        else
        {
            //If someone else tries to make one of these classes destroy it, keep the oldest one
            DestroyImmediate(this);
        }
    }

    private void OnDestroy()
    {
        if(Instance == this)
            Instance = null;
    }
    // Use this for initialization
    void Start () {

        //Get the mouse interaction the player wanted to use, chosen on the game setup screen
        if (PlayerPrefs.HasKey("DragDrop"))
        {
            if (PlayerPrefs.GetInt("DragDrop") == 1)
            {
                useDragDrop = true;
            }
            else
                useDragDrop = false;
        }

        if (MainMenu_Controller.Instance != null)
        {
            chosenBoardPreset = MainMenu_Controller.Instance.defaultPreset;
            GameBoard boardScript = boardPrefab.GetComponent<GameBoard>();
            if (boardScript != null)
            {
                boardScript.SetBoardPresetInfo(chosenBoardPreset);
            }
            else
                Debug.Log("Unable to set data in prefab instance.");

            if(mouseObjectPrefab != null)
                Instantiate(mouseObjectPrefab);
        }
        OnCreateNewBoard(boardPrefab);
	}

    /// <summary>
    /// Creates a passed in game board.
    /// </summary>
    /// <param name="boardPrefab"></param>
    void OnCreateNewBoard(GameObject boardPrefab)
    {
        if (boardPrefab != null)
        {
            activeBoardObject = Instantiate(boardPrefab);
            if (activeBoardObject != null)
            {
                activeBoardScript = activeBoardObject.GetComponent<GameBoard>();
                activeBoardScript.GameOver += OnGameOver;

                OnBoardCreated(activeBoardObject.transform, activeBoardScript.GetBoardWidth());
            }

            isGameOver = false;
        }
    }

    public bool GetDragDropFlag()
    {
        return useDragDrop;
    }

    /// <summary>
    /// Broadcast the board creation event. Sends off the center of the board that is made, and the size of one side.
    /// </summary>
    /// <param name="eventTransform"></param>
    /// <param name="eventFloat"></param>
    private void OnBoardCreated(Transform eventTransform, float eventFloat)
    {
        if (BoardCreated != null)
            BoardCreated.Invoke(eventTransform, eventFloat);
    }
    
    /// <summary>
    /// Reset the board if this manager exists
    /// </summary>
    public static void OnResetBoard_Static()
    {
        if (Instance != null)
        {
            Instance.OnResetBoard();
        }
    }

    /// <summary>
    /// Checks if drag n drop is enabled.
    /// </summary>
    /// <returns></returns>
    public static bool DragDropEnabled()
    {
        if(Instance != null)
        {
            return Instance.GetDragDropFlag();
        }
        return false;
    }

    public void OnResetBoard()
    {
        ResetActiveBoard();
    }

    /// <summary>
    /// When a game is over. Display the score messages. And prompt for next steps.
    /// </summary>
    /// <param name="score">The remaining pegs on the finished board.</param>
    public void OnGameOver(int score)
    {
        isGameOver = true;

        //Evaluate score text
        string scoreText = "";
        //HARDCODE for now
        switch(score)
        {
            case 1:
                scoreText = "You Win!";
                break;
            default:
                scoreText = "You Lose.\nYou had " + score.ToString() + " pegs remaining.";
                break;
            //case 1:
            //    scoreText = "Left Only One\nYou're genius";
            //    break;
            //case 2:
            //    scoreText = "Left Two\nYou're purty smart";
            //    break;
            //case 3:
            //    scoreText = "Left Three\nYou're just plain dumb";
            //    break;

            //case 4:
            //default:
            //    scoreText = "Left Four or More\nYou're just plain\n\"EG-NO-RA-MOOSE\"";
            //    break;
        }
        TogglePause(scoreText);
    }

    /// <summary>
    /// Destroys the current board in the world, then remakes the board.
    /// </summary>
    private void ResetActiveBoard()
    {
        Destroy(activeBoardObject);

        OnCreateNewBoard(boardPrefab);
    }

    /// <summary>
    /// Static hook to get the pause state. Hides the MainMenu controller from other game logic classes.
    /// </summary>
    /// <returns></returns>
    public static bool IsControlEnabled()
    {
        return !MainMenu_Controller.GetPause();
    }

    /// <summary>
    /// When the user hits escape during play, or if the game ends. We pause for both. If the game over state occurs, we pass the game over text with the score to the main menu controller.
    /// </summary>
    /// <param name="statusText"></param>
    private void TogglePause(string statusText="")
    {
        if(isGameOver)
            MainMenu_Controller.Instance.TogglePause_GameOver(statusText);
        else
            MainMenu_Controller.Instance.TogglePause(statusText);
    }
	
	// Update is called once per frame
	void Update () {
        //Non main menu scene commands
       
        //Toggle menu visibility / pause game
        if (Input.GetButtonDown("Toggle Menu"))
        {
            if(!isGameOver && MainMenu_Controller.Instance != null)
            {
                TogglePause();
            }
        }
    }

}
