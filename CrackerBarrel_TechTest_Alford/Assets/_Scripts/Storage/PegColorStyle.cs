using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public virtual void OnGUI()
    {
       
    }
}
