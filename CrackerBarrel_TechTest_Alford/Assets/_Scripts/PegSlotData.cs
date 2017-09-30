using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds information about an individual peg. Color, game object transform when existing in the world.
/// </summary>
public class PegData
{
    public Color pegColor = Color.blue;
    public Transform pegTransform;
}

public delegate void EmptySlotHook();
public delegate void PegDataFuncHook(PegData pegColorData);
public delegate void PegSlotFuncHook(PegSlotData changingSlotData);


/// <summary>
/// Each peg slot of the board is represented logically by this class. Should hold connections to its neighbors, and functions to sync neighbor changes.
/// </summary>
public class PegSlotData{

    [SerializeField]
    private PegData pegInSlot; //Specifies if there is a peg in this slot
    private PegSlotData[] pegNeighbors; //This should be all the neighbors of this peg slot, even outside of the game board (null values). Index lookup should be done with enum values for directions

    public event PegDataFuncHook PegAdded; //Whenever a peg is added to this slot, call this event
    public event EmptySlotHook PegRemoved; //Whenever a peg is removed from this slot, call this event
    public event EmptySlotHook PegSelected; //When this peg is selected
    public event EmptySlotHook PegDeselected; //when this peg is de-selected

	public PegSlotData()
    {
        //Default to have a peg in the slot when created.
        pegInSlot = new PegData();

        //Always static 6, refernce types default to null - specifies off the board directions
        pegNeighbors = new PegSlotData[6];
    }

    /// <summary>
    /// Add the passed in neighbor to our pegNeighbors array in the index specified by neighborDirection. This function has an optional parameter to specify to do a two way connection... Connect to each other.
    /// </summary>
    /// <param name="connectingNeighbor">Can be null.</param>
    /// <param name="neighborDirection">Should be direction noted in the enum list.</param>
    /// <param name="connectOppositeAfter">Optional bool to specify a two way connection, defaults to true</param>
    public void ConnectPegNeighbor(PegSlotData connectingNeighbor, PegDirection neighborDirection, bool connectOppositeAfter=true)
    {
        if (neighborDirection != PegDirection.INVALID)
        {
            pegNeighbors[(int)neighborDirection] = connectingNeighbor;

            //If we need to connect our neighbor to ourselves also
            if (connectOppositeAfter && connectingNeighbor != null)
            {
                //Connect the other way now, but pass in false to the optional paremeter to specify we don't need to do the extra connection
                connectingNeighbor.ConnectPegNeighbor(this, GetOppositeDirection(neighborDirection), false);
            }
        }
    }

    /// <summary>
    /// Check if this slot has a peg in it. Null is an empty slot.
    /// </summary>
    /// <returns>True for peg in slot.</returns>
    public bool HasPeg()
    {
        return pegInSlot != null;
    }

    /// <summary>
    /// Check if there is a peg in the nearby slot based on the checkDirection.
    /// </summary>
    /// <param name="checkDirection">The direction to check.</param>
    /// <returns>True if there is peg in checked slot.</returns>
    public bool HasPegInDirection(PegDirection checkDirection)
    {
        if (pegNeighbors != null && checkDirection != PegDirection.INVALID && pegNeighbors[(int)checkDirection] != null)
        {
            //Return the value of HasPeg function of the peg in checkDirection
            return pegNeighbors[(int)checkDirection].HasPeg();
        }
        return false;
    }

    /// <summary>
    /// Returns the neighbor in the passed in direction.
    /// </summary>
    /// <param name="neighborDirection">The direction of the neighbor you need.</param>
    /// <returns>Neighbor PegSlotData class... or null</returns>
    public PegSlotData GetNeighbor(PegDirection neighborDirection)
    {
        if (neighborDirection != PegDirection.INVALID)
            return pegNeighbors[(int)neighborDirection];
        return null;
    }

    /// <summary>
    /// Queries whether or not we can jump this peg in a direction. To jump a peg, there needs to be a peg in the nearby slot and an empty slot on the other side of the nearby peg.
    /// </summary>
    /// <param name="jumpDirection">Direction to check for jumping.</param>
    /// <returns>Returns the peg slot we can jump to. Else null.</returns>
    public PegSlotData CanJumpInDirection(PegDirection jumpDirection)
    {
        //verify the immediate neighbor in jumpDirection is holding a peg
        if (HasPegInDirection(jumpDirection))
        {
            //There is a peg in jumpDirection

            PegSlotData neighborData = GetNeighbor(jumpDirection);
            
            //verify that pegslot on the other side of the neighbor is empty
            if (neighborData != null && neighborData.HasPegInDirection(jumpDirection) == false)
            {
                return neighborData.GetNeighbor(jumpDirection);
            }
        }
        //can't jump
        return null;
    }

    /// <summary>
    /// Jump a peg in jump direction if checks pass. Remove the jumped peg. Empty out the slot we are leaving.
    /// </summary>
    /// <param name="jumpDirection">Direction to jump.</param>
    /// <returns>True if successful</returns>
    public bool DoJumpInDirection(PegDirection jumpDirection)
    {
        //Verify that we can jump in this direction
        PegSlotData jumpLocation = CanJumpInDirection(jumpDirection);
        if (jumpLocation != null)
        {
            //Place the jumping tee in the empty space
            MovePeg(jumpLocation);

            //Remove the jumped tee
            GetNeighbor(jumpDirection).RemovePeg(); //already many checks deep, assume there is a neighbor peg, and remove it
            return true;
        }
        return false;
    }

    public void SetPegData(PegData setData)
    {
        pegInSlot = setData;
        OnPegAdded();
    }
    
    /// <summary>
    /// Move the peg from this classes pegInSlot to the moveTo classes pegInSlot. Set to null when moving. Assume there is no peg in the moveTo slot.
    /// </summary>
    /// <param name="moveTo">The empty slot we are moving the peg to.</param>
    /// <returns>True if successful</returns>
    private bool MovePeg(PegSlotData moveTo)
    {
        if(moveTo != null)
        {
            moveTo.SetPegData(pegInSlot);
            RemovePeg();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Set the peg in this slot to null. May need to broadcast a change here for graphics.
    /// </summary>
    private void RemovePeg()
    {
        pegInSlot = null;
        OnPegRemoved();
    }

    /// <summary>
    /// Look in all directions and return true if we can jump in any of the directions
    /// </summary>
    /// <returns></returns>
    public bool CanJumpInAnyDirection()
    {
        for(int i = 0; i < 6; i++)
        {
            if (CanJumpInDirection((PegDirection)i) != null)
                return true;
        }
        return false;
    }

    public PegData GetPegColorData()
    {
        return pegInSlot;
    }

    public void SetAsStartSlot()
    {

        //This slot is the empty start slot
        RemovePeg();
    }

    /// <summary>
    /// Select event for this peg slot
    /// </summary>
    public void Select()
    {
        if(PegSelected != null)
        {
            PegSelected.Invoke();
        }
    }

    /// <summary>
    /// Deselect event for this peg slot
    /// </summary>
    public void Deselect()
    {
        if(PegDeselected != null)
        {
            if(pegInSlot != null)
                SetPegData(pegInSlot);
            PegDeselected.Invoke();
        }
    }

    /// <summary>
    /// Get's the opposite direction enum and returns it. 
    /// Left returns right.
    /// Top Left returns Bottom Right
    /// Top Right returns bottom left. 
    /// And the opposite of each...
    /// </summary>
    /// <param name="val">The direction to get the opposite of.</param>
    /// <returns>The opposite direction enum</returns>
    public PegDirection GetOppositeDirection(PegDirection val)
    {
        switch(val)
        {
            case PegDirection.LEFT:
                return PegDirection.RIGHT;
            case PegDirection.RIGHT:
                return PegDirection.LEFT;
            case PegDirection.TOP_LEFT:
                return PegDirection.BOTTOM_RIGHT;
            case PegDirection.BOTTOM_RIGHT:
                return PegDirection.TOP_LEFT;
            case PegDirection.TOP_RIGHT:
                return PegDirection.BOTTOM_LEFT;
            case PegDirection.BOTTOM_LEFT:
                return PegDirection.TOP_RIGHT;
            default:
                Debug.LogError("PegSlot.GetOppositeDirection() - Attempting to query INVALID direction enum.");
                return PegDirection.INVALID;
        }
    }

    /// <summary>
    /// Event occurs when a peg is added to this slot.
    /// </summary>
    void OnPegAdded()
    {
        if(PegAdded != null)
        {
            PegAdded.Invoke(pegInSlot);
        }
    }

    /// <summary>
    /// Event occurs when a peg is removed from this slot
    /// </summary>
    void OnPegRemoved()
    {
        if(PegRemoved != null)
        {
            PegRemoved.Invoke();
        }
    }
}

/// <summary>
/// This should specify the direction for peg related queries. Should only be 0 through 5, with -1 being invalid for error checking
/// </summary>
public enum PegDirection
{
    INVALID= -1,
    RIGHT=0,
    BOTTOM_RIGHT = 1,
    BOTTOM_LEFT = 2,
    LEFT = 3,
    TOP_LEFT = 4,
    TOP_RIGHT = 5,
}
