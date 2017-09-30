using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each peg slot of the board is represented logically by this class. Should hold connections to its neighbors, and functions to sync neighbor changes.
/// </summary>
[System.Serializable]
public class PegSlotData{

    private PegSlotData[] pegNeighbors; //This should be all the neighbors of this peg slot, even outside of the game board (null values). Index lookup should be done with enum values for directions

	public PegSlotData()
    {
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
    /// Get's the opposite direction enum and returns it. 
    /// Left returns right.
    /// Top Left return Bottom Right
    /// Top Right returns bottom left. 
    /// And the opposite of each...
    /// </summary>
    /// <param name="val">The direction to get the opposite of.</param>
    /// <returns></returns>
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
