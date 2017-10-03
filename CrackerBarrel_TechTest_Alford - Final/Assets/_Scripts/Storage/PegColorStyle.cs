using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all color styles. Due to class shearing problems, the abstract part probably isn't needed. 
/// </summary>
[System.Serializable]
public abstract class PegColorStyle{

    public string name;

    /// <summary>
    /// Coloring function for the board to call to color its own pegs.
    /// </summary>
    /// <param name="boardArray"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public abstract void RequestColor(ref List<List<PegSlotData>> boardArray, int i, int j);
    
    public PegColorStyle()
    {
        name = "Base Style";
        //hideFlags = HideFlags.HideAndDontSave;
    }
}
