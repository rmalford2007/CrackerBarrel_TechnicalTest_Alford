using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This file should hold global type delegates and enums that don't belong in a specific single class. 

/// <summary>
/// Events that need to broadcast a int value with it
/// </summary>
/// <param name="val"></param>
public delegate void Int_Event(int val);

/// <summary>
/// Events that are parameterless. 
/// </summary>
public delegate void Void_Event();

/// <summary>
/// Events that pass a peg world object class with it
/// </summary>
/// <param name="pegColorData"></param>
public delegate void PegData_Event(PegData pegColorData);

/// <summary>
/// Events that pass a peg data game logic class with it
/// </summary>
/// <param name="changingSlotData"></param>
public delegate void PegSlotData_Event(PegSlotData changingSlotData);

/// <summary>
/// Holds values for mouse specific states for pegs
/// </summary>
public enum PegMouseState
{
    HOVER,
    DRAG,
    DEFAULT,
    SELECTED
}

/// <summary>
/// Holds action state for a tile when a peg is being dropped on it, or about to drop
/// </summary>
public enum TileAction
{
    DEFAULT,
    LANDING,
    DISABLE,
}


/// <summary>
/// This should specify the direction for peg related queries. Should only be 0 through 5, with -1 being invalid for error checking
/// </summary>
public enum PegDirection
{
    INVALID = -1,
    RIGHT = 0,
    BOTTOM_RIGHT = 1,
    BOTTOM_LEFT = 2,
    LEFT = 3,
    TOP_LEFT = 4,
    TOP_RIGHT = 5,
}
