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

    private void Awake()
    {
        if(Instance == null)
        {
            //First instance made stays
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            //If someone else tries to make one of these classes destroy it, keep the oldest one
            DestroyImmediate(this);
        }
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
        }
    }

    /// <summary>
    /// When a game is over. Display the score messages. And prompt for next steps.
    /// </summary>
    /// <param name="score">The remaining pegs on the finished board.</param>
    public void OnGameOver(int score)
    {
        Debug.Log("GameOver: score = " + score.ToString());
        Destroy(activeBoardObject);

        OnCreateNewBoard(classicBoardPrefab);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

}
