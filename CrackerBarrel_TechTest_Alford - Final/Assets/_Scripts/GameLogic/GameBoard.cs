using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the logic for a single game board. Controls game over conditions, moving pegs, jumping pegs, and removing pegs.
/// </summary>
public class GameBoard : MonoBehaviour {

    [HideInInspector]
    [SerializeField]
    private BoardPreset boardDisplayInfo; //Set to serialize so we can modify this in a prefab instead before we instantiate the prefab, also hide it so it can't be modified manually in inspector
    private int baseRowPegCount = 5; //this information is held in boardDisplayInfo, but copy it here on start. Controls the size of the triangle.

    [Range(.5f, 10f)]
    [Tooltip("The distance between each peg.")]
    public float pegSpacing = 1f; //the horizontal spacing between peg holes, this is peg center to peg center distance. Each level will be staggered over by half peg spacing

    [Tooltip("The prefab that is instantiated for each slot on the game board.")]
    public GameObject pegSlotPrefab;

    public event Int_Event GameOver; //An event that broadcast gameover in addition to passing the int score as a parameter

    private bool boardChanged = false; //This bool should be set to true anytime a peg is jumped and is removed, to then check for game over conditions
    private float pegRowHeightChange = 1f; //The spacing to move each row of holes to. Should be half way between holes beneath them, but still pegSpacing away from other holes.

    private List<List<PegSlotData>> boardArrays; //This holds the initial data set of peg holes in the board. This should be used for initial connectivity of peg holes.
    private HashSet<PegSlotData> allPegSlots; //Hashed set of all slots for easier lookup
    private Dictionary<PegSlotData, PegSlot> pegTileGameObjects; //Dictionary that links the pegSlotData class to its appropriate world object that is spawned

    //Ran out of time for optimization - throttle the size of the board to something smaller
    //private HashSet<PegSlotData> moveableSlots; //Hashed set of slots. Plan is to only have this contain slots that can still jump

    private PegSlotData activePeg; //Holds the peg that is being held by the mouse, or actively clicked for moving

    private bool isFreshStart = true; //this flag should inform the board that the player needs to choose a peg as the first empty space to start the game

    private void Awake()
    {
        if (boardDisplayInfo == null)
        {
            Debug.Log("Display info null on creation");
        }
        else
            baseRowPegCount = boardDisplayInfo.baseRowPegCount;
        pegRowHeightChange = Mathf.Sqrt(Mathf.Pow(pegSpacing, 2f) - Mathf.Pow(pegSpacing / 2f, 2f)); //use pythagorean theorem to get the height value
        pegTileGameObjects = new Dictionary<PegSlotData, PegSlot>();
        InitBoard(baseRowPegCount);


        //Init the moveable slots
        //moveableSlots = new HashSet<PegSlotData>();
    }

    public void SetBoardPresetInfo(BoardPreset customPreset)
    {
        if(customPreset != null)
            boardDisplayInfo = customPreset;
    }

    public float GetBoardWidth()
    {
        return baseRowPegCount * pegSpacing;
    }

    private void ColorPegs()
    {
        boardDisplayInfo.RequestColor(ref boardArrays, 0, 0);
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
            boardArrays.Add(new List<PegSlotData>(pegCount - i));
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
            if(slotData.HasPeg() && slotData.CanJumpInAnyDirection())
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
        
        Vector3 nextPosition = Vector3.zero;
        float zPos, xPos, yPos;
        if (boardArrays != null)
        {
            //All peg graphics should be spawned here

            //Loop through the board array we created and spawn objects in the world for each hole
            //for each column
            for (int i = 0; i < boardArrays.Count; i++)
            {
                xPos = -(boardArrays.Count) / 2f * pegSpacing + i * pegSpacing;
                zPos = -boardArrays.Count / 2f * pegRowHeightChange;
                //for each row
                for (int j = 0; j < boardArrays[i].Count; j++)
                {
                    
                    xPos += pegSpacing / 2f;
                    
                    yPos = transform.position.y;
                    nextPosition.Set(xPos, yPos, zPos);

                    GameObject createdSlot = Instantiate(pegSlotPrefab, nextPosition, Quaternion.identity, transform);
                    
                    if(createdSlot != null)
                    {
                        PegSlot slotScript = createdSlot.GetComponent<PegSlot>();
                        if(slotScript != null)
                        {
                            if (i == 0 && j == boardArrays[i].Count - 1)
                            {
                                slotScript.SetSlotData(boardArrays[i][j], true);
                            }
                            else
                                slotScript.SetSlotData(boardArrays[i][j]);

                            slotScript.TileSelected += OnSelectPeg;
                            pegTileGameObjects.Add(boardArrays[i][j], slotScript);
                        }
                    }

                    //Move next position values
                    zPos += pegRowHeightChange;
                }
            }

            //Activate StartPegSelection to choose the first peg to remove for board setup

            //HARDCODE - Remove first peg to start game (top middle for now) - hardcode classic to see if it works
            ColorPegs();
        }


    }

    /// <summary>
    /// Check if the passed in peg can jump in checkDirection. Imagine using this for helper interface items (Blink available locations to jump to when holding the peg)
    /// </summary>
    /// <param name="checkSlot">The slot data that is doing the jumping.</param>
    /// <param name="checkDirection">The direction to jump in.</param>
    /// <returns>Returns true if it can jump, else false</returns>
    public PegSlotData CanJumpInDirection(PegSlotData checkSlot, PegDirection checkDirection)
    {
        if(checkSlot != null)
        {
            return checkSlot.CanJumpInDirection(checkDirection);
        }
        return null;
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
            if(checkSlot.DoJumpInDirection(jumpDirection))
            {
                //Jump was good, now that a peg was removed, evaluate board for game over conditions
                boardChanged = true;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// When a peg has any sort of mouse click on it, evaluate the current state
    /// </summary>
    /// <param name="selectedSlotData">The peg data that is being clicked</param>
    public void OnSelectPeg(PegSlotData selectedSlotData)
    {
        if (isFreshStart)
        {
            //Make sure this isn't null, do a peg check in case something is messed up - if it is messed up there are other problems besides this... as all pegs should be in the board
            if(selectedSlotData != null && selectedSlotData.HasPeg())
            {
                selectedSlotData.SetAsStartSlot();
                isFreshStart = false;
            }
        }
        else
        {
            //If drag n drop support is enabled or not
            if (GameManager.DragDropEnabled() && MouseDragPeg.Instance != null)
            {
                DragDrop_Evaluate(selectedSlotData);
            }
            else
            {
                //No drag n drop, do classic click to activate pegs, click again to drop them somewhere
                ClickClick_Evaluate(selectedSlotData);
            }
            
        }
    }

    /// <summary>
    /// Drag n drop behavior for a peg being dropped somewhere
    /// </summary>
    /// <param name="selectedSlotData"></param>
    private void DragDrop_Evaluate(PegSlotData selectedSlotData)
    {
        //Drag n dropped on ourselves
        if(activePeg == selectedSlotData)
        {
            if(activePeg != null)
            {
                activePeg.OnPegStopDrag();
                activePeg = null;
                MouseDragPeg.Instance.ActivatePeg(null);
            }
        }
        //Nothing selected, start dragging
        else if(activePeg == null && selectedSlotData != null)
        {
            activePeg = selectedSlotData;
            MouseDragPeg.Instance.ActivatePeg(activePeg);
            activePeg.OnPegStartDrag();
        }
        else if (activePeg != null && selectedSlotData != null)
        {
            //There is no peg in this selected slot, but we already have an active peg, this means we are dropping the peg in this slot

            //Evaluate the jump direction to see if we can process the jump request
            PegDirection jumpDirection = EvaluateDirection(activePeg, selectedSlotData);
            //If this is a valid direction, verify that we can jump here
            if (jumpDirection != PegDirection.INVALID)
            {
                PegSlotData checkJumpSlot = CanJumpInDirection(activePeg, jumpDirection);

                //Check the landing position in the jumpDirection, if the returned position is equal to our clicked position, then this is a valid jump
                if (checkJumpSlot == selectedSlotData)
                {
                    if (DoJumpInDirection(activePeg, jumpDirection))
                    {
                        //Finished jumping deselect the tiles we used
                        activePeg.OnPegStopDrag();
                        activePeg = null;

                        MouseDragPeg.Instance.ActivatePeg(activePeg);
                    }
                }
                else
                {
                    activePeg.OnPegStopDrag();
                    activePeg = null;

                    MouseDragPeg.Instance.ActivatePeg(activePeg);
                }
            }
            else
            {
                activePeg.OnPegStopDrag();
                activePeg = null;

                MouseDragPeg.Instance.ActivatePeg(activePeg);
            }
        }
    }

    /// <summary>
    /// Multiple click interaction for a peg being clicked 
    /// </summary>
    /// <param name="selectedSlotData"></param>
    private void ClickClick_Evaluate(PegSlotData selectedSlotData)
    {
        //if there is a peg in the slot, activate it, but first deactivate the old peg that was active
        if (selectedSlotData != null && selectedSlotData.HasPeg())
        {
            if (activePeg != null)
            {
                activePeg.Deselect();
            }
            activePeg = selectedSlotData;

            if (selectedSlotData != null)
                selectedSlotData.Select();

        }
        else if (activePeg != null && selectedSlotData != null)
        {
            //There is no peg in this selected slot, but we already have an active peg, this means we are dropping the peg in this slot

            //Evaluate the jump direction to see if we can process the jump request
            PegDirection jumpDirection = EvaluateDirection(activePeg, selectedSlotData);
            //If this is a valid direction, verify that we can jump here
            if (jumpDirection != PegDirection.INVALID)
            {
                PegSlotData checkJumpSlot = CanJumpInDirection(activePeg, jumpDirection);

                //Check the landing position in the jumpDirection, if the returned position is equal to our clicked position, then this is a valid jump
                if (checkJumpSlot == selectedSlotData)
                {
                    if (DoJumpInDirection(activePeg, jumpDirection))
                    {
                        //Finished jumping deselect the tiles we used
                        activePeg.Deselect();
                        activePeg = null;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Compares the angle from actionSlot to destinationSlot to get an equivalent PegDirection enum value.
    /// </summary>
    /// <param name="actionSlot">Jumping peg slot</param>
    /// <param name="destinationSlot">Landing peg slot</param>
    /// <returns></returns>
    public PegDirection EvaluateDirection(PegSlotData actionSlot, PegSlotData destinationSlot)
    {
        if(actionSlot != null && pegTileGameObjects.ContainsKey(actionSlot) && destinationSlot != null && pegTileGameObjects.ContainsKey(destinationSlot))
        {
            //Both slots are in the dictionary, get the the transform position vectors from each
            Vector3 directionVector = pegTileGameObjects[destinationSlot].transform.position - pegTileGameObjects[actionSlot].transform.position;
            Vector3 rightVector = pegTileGameObjects[actionSlot].transform.right;
            Vector3 upAxis = pegTileGameObjects[actionSlot].transform.up;

            float deltaAngle = Vector3.SignedAngle(directionVector, rightVector, upAxis);
            
            if ((deltaAngle >= -30f && deltaAngle <= 0f) || (deltaAngle >= 0f && deltaAngle < 30f))
            {
                return PegDirection.RIGHT;
            }
            else if (deltaAngle >= 30f && deltaAngle < 90f)
            {
                return PegDirection.TOP_RIGHT;
            }
            else if (deltaAngle >= 90f && deltaAngle < 150f)
            {
                return PegDirection.TOP_LEFT;
            }
            else if (deltaAngle >= 150f && deltaAngle > -150f)
            {
                return PegDirection.LEFT;
            }
            else if (deltaAngle >= -150f && deltaAngle < -90f)
            {
                return PegDirection.BOTTOM_LEFT;
            }
            else if (deltaAngle >= -90f && deltaAngle < -30f)
            {
                return PegDirection.BOTTOM_RIGHT;
            }

        }
        return PegDirection.INVALID;
    }
	
	// Update is called once per frame
	void Update () {
        if(boardChanged)
        {
            boardChanged = false;
            //Check if there are no moves left
            if(!Check_CanMoveAll())
            {
                //No moves left, game over
                OnGameOver();
            }
        }
    }

    void OnGameOver()
    {
        //Now that the game is over, count pegs to report to the game manager
        int pegScore = Check_CountScore();

        if(GameOver != null)
        {
            GameOver.Invoke(pegScore);
        }
    }
}
