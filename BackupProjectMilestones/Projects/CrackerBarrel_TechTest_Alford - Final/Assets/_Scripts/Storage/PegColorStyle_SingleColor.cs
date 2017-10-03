using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that color the entire board a single color.
/// </summary>
[System.Serializable]
public class PegColorStyle_SingleColor : PegColorStyle {
   
    public Color singleColor;
    
    public PegColorStyle_SingleColor()
    {
        name = "Single Color";
        singleColor = Color.red;
    }

    /// <summary>
    /// Add color to list regardless of what it is. Allow duplicates.
    /// </summary>
    /// <param name="nextColor"></param>
    public void AddColor(Color nextColor)
    {
        singleColor = nextColor;
    }


    /// <summary>
    /// Coloring function for the board to call to color its own pegs.
    /// </summary>
    /// <param name="boardArray"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public override void RequestColor(ref List<List<PegSlotData>> boardArray, int i, int j)
    {
        Debug.Log("Request in Single Color");
        for(int row = 0; row < boardArray.Count; row++)
        {
            for (int col = 0; col < boardArray[row].Count; col++)
            {
                boardArray[col][row].SetPegData(new PegData(singleColor));
            }
        }
    }
}
