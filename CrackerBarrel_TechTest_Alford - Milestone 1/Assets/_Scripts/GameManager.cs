using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Singleton - Controls the start and stop events for the game. Holds the controls to show / hide / navigate the menu during play mode. 
/// </summary>
public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    public GameObject classicBoardPrefab;
    
    private GameObject activeBoardObject;
    private GameBoard activeBoardScript;

    private bool isGameOver = false;

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
        OnCreateNewBoard(classicBoardPrefab);
	}

    void OnCreateNewBoard(GameObject boardPrefab)
    {
        if (boardPrefab != null)
        {
            activeBoardObject = Instantiate(boardPrefab);
            if (activeBoardObject != null)
            {
                activeBoardScript = activeBoardObject.GetComponent<GameBoard>();
                activeBoardScript.GameOver += OnGameOver;
            }

            isGameOver = false;
        }
    }

    public static void OnResetBoard_Static()
    {
        if (GameManager.Instance != null)
        {
            Instance.OnResetBoard();
        }
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
        Debug.Log("GameOver: score = " + score.ToString());

        //Evaluate score text
        string scoreText = "";
        //HARDCODE for now
        switch(score)
        {
            case 1:
                scoreText = "Left Only One\nYou're genius";
                break;
            case 2:
                scoreText = "Left Two\nYou're purty smart";
                break;
            case 3:
                scoreText = "Left Three\nYou're just plain dumb";
                break;

            case 4:
            default:
                scoreText = "Left Four or More\nYou're just plain\n\"EG-NO-RA-MOOSE\"";
                break;
        }
        TogglePause(scoreText);
    }

    private void ResetActiveBoard()
    {
        Destroy(activeBoardObject);

        OnCreateNewBoard(classicBoardPrefab);
    }

    public static bool IsControlEnabled()
    {
        return !MainMenu_Controller.GetPause();
    }

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
