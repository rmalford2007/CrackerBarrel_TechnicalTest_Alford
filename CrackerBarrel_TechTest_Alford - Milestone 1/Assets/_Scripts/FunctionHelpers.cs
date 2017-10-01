using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Int_Event(int val);
public delegate void Void_Event();
public delegate void PegData_Event(PegData pegColorData);
public delegate void PegSlotData_Event(PegSlotData changingSlotData);

public enum PegMouseState
{
    HOVER,
    DRAG,
    DEFAULT,
    SELECTED
}

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
