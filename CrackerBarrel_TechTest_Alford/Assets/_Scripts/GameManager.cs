using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton - Controls the start and stop events for the game. Holds the controls to show / hide / navigate the menu during play mode. 
/// </summary>
public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    public GameObject classicBoardPrefab;

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
	    if(classicBoardPrefab != null)
        {
            Instantiate(classicBoardPrefab);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
