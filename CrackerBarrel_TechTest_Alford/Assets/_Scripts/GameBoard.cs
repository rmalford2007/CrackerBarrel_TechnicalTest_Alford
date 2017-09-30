using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the logic for a single game board. Controls game over conditions, moving pegs, jumping pegs, and removing pegs.
/// </summary>
public class GameBoard : MonoBehaviour {

    [Range(5, 20)]
    [Tooltip("This number should be the number of pegs in the longest row of the board. In a classic game this should be 5 pegs. Changing this value will not resize the board during play, change this in edit mode.\nBounds: 5, 20 inclusive")]
    public int baseRowPegCount = 5;

    private List<List<PegSlotData>> boardArrays; //This holds the initial data set of peg holes in the board. This should be used for initial connectivity of peg holes.
    private HashSet<PegSlotData> allPegSlots; //Hashed set of all slots for easier lookup
    //private HashSet<PegSlotData> moveableSlots; //Hashed set of slots. Plan is to only have this contain slots that can still jump

    private void Awake()
    {

        InitBoard(baseRowPegCount);


        //Init the moveable slots
        //moveableSlots = new HashSet<PegSlotData>();
    }

    /// <summary>
    /// Initializes the game board data to use a triangle with base row holes specified by pegCount. Note - This doesn't create game objects, only the data side that the game objects will spawn from later.
    /// </summary>
    /// <param name="pegCount">The number of peg holes at the bottom of the triangle board.</param>
    private void InitBoard(int pegCount)
    {
        //Initialize 2d array - variable list sizes
        boardArrays = new List<List<PegSlotData>>();

        allPegSlots = new HashSet<PegSlotData>();

        for (int i = 0; i < pegCount; i++)
        {
            boardArrays.Add(new List<PegSlotData>(5 - i));
        }

        //Loop through and add peg slots
        //Note we are looking for a triangle sort of 2D array
        //   Textual representation of 2d game board / array. With row 0 at the bottom. Row id's left. Column Id's bottom. 
        //              *
        //             * *
        //            * * *
        //           * * * *
        //          * * * * *
        //
        //          LEFT SHIFT - gives next relation
        //
        //      4 |*
        //      3 |* *
        //      2 |* * *
        //      1 |* * * * 
        //      0 |* * * * * 
        //        -----------
        //         0 1 2 3 4

        // For each column
        for (int i = 0; i < pegCount; i++)
        {
            //Add variable amount of peg holes depending on column index (subtract the index i from the baseRowPegCount should get a 5, 4, 3, 2, 1 count that we need and scalable to higher values
            for (int j = 0; j < pegCount - i; j++)
            {
                boardArrays[i].Add(new PegSlotData());
                allPegSlots.Add(boardArrays[i][j]); //Add the new slot to the all hashset
            }
        }

        //Connect peg slots together, stop at the last peg of each row
        for (int i = 0; i < boardArrays.Count - 1; i++)
        {
            //Stop at the last peg of each column (since we are doing 2 way connections we stop at 1 from each of the bounds at the top and at the right)
            for (int j = 0; j < boardArrays[i].Count - 1; j++)
            {
                //Each peg will 2 way connect to the peg at the top right, and to the right.  (see text representation at the top if directions aren't clear)
                if (boardArrays[i][j] != null)
                {
                    boardArrays[i][j].ConnectPegNeighbor(boardArrays[i][j + 1], PegDirection.TOP_RIGHT);
                    boardArrays[i][j].ConnectPegNeighbor(boardArrays[i + 1][j], PegDirection.RIGHT);
                }
                else
                {
                    Debug.LogWarning("GameBoard.InitBoard - Encountered null peg slot. This shouldn't happen, all peg slots in the array should not be null at this point.");
                }

                //Then connect the top right peg with the right peg. Direction is from top right to right, Bottom Right
                if(boardArrays[i][j + 1] != null)
                    boardArrays[i][j + 1].ConnectPegNeighbor(boardArrays[i + 1][j], PegDirection.BOTTOM_RIGHT);
            }
        }
    }

    /// <summary>
    /// Check all pegs if there are any moves left. Returns true if there is at least 1 move. TODO: In a scalable format this may be slow.
    /// </summary>
    public bool Check_CanMoveAll()
    {
        //Check all slots if they can move
        foreach(PegSlotData slotData in allPegSlots)
        {
            if(slotData.CanJumpInAnyDirection())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Query how many pegs are left. Return an int amount of pegs remaining. TODO: In a scalable format this may be slow.
    /// </summary>
    public int Check_CountScore()
    {
        int remainingPegs = 0;
        //Check all slots if they contain a peg
        foreach (PegSlotData slotData in allPegSlots)
        {
            if (slotData.HasPeg())
            {
                remainingPegs++;
            }
        }
        return remainingPegs;
    }

    // Use this for initialization
    void Start () {

	    if(boardArrays != null)
        {
            //Loop through the board array we created and spawn objects in the world for each hole
            for(int i = 0; i < boardArrays.Count; i++)
            {
                for(int j = 0; j < boardArrays[i].Count; j++)
                {
                    Debug.Log("Creating peg hole game object.");
                }
            }
        }
	}

    /// <summary>
    /// Check if the passed in peg can jump in checkDirection. Imagine using this for helper interface items (Blink available locations to jump to when holding the peg)
    /// </summary>
    /// <param name="checkSlot">The slot data that is doing the jumping.</param>
    /// <param name="checkDirection">The direction to jump in.</param>
    /// <returns>Returns true if it can jump, else false</returns>
    public bool CanJumpInDirection(PegSlotData checkSlot, PegDirection checkDirection)
    {
        CanJumpInDirection(null, PegDirection.RIGHT);
        if(checkSlot != null)
        {
            return checkSlot.CanJumpInDirection(checkDirection) != null;
        }
        return false;
    }

    /// <summary>
    /// Jump the passed in peg in the noted jumpDirection. Does a valid jump check to verify it can jump.
    /// </summary>
    /// <param name="checkSlot">The slot to move.</param>
    /// <param name="jumpDirection">The direction to move</param>
    /// <returns>True if successful</returns>
    public bool DoJumpInDirection(PegSlotData checkSlot, PegDirection jumpDirection)
    {
        if (checkSlot != null)
        {
            return checkSlot.DoJumpInDirection(jumpDirection);
        }
        return false;
    }


	
	// Update is called once per frame
	void Update () {
		
	}
}
