using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that colors the board by setting entire rows to a color, and moving to the next color in the list for the next row.
/// </summary>
[System.Serializable]
public class PegColorStyle_RowByRow : PegColorStyle {
    
    [Tooltip("The order of colors that appear on the board. Cycles when the last color is used.")]
    public List<Color> rowColors;

    private int currentIndex = 0; //this is the iterator that cycles through the color list as it is used.
    
    public PegColorStyle_RowByRow()
    {
        name = "Row By Row";
        rowColors = new List<Color>();
        rowColors.Add(Color.blue);
        rowColors.Add(Color.red);
        rowColors.Add(Color.white);
    }

    /// <summary>
    /// Add color to list regardless of what it is. Allow duplicates.
    /// </summary>
    /// <param name="nextColor"></param>
    public void AddColor(Color nextColor)
    {
        rowColors.Add(nextColor);
    }

    /// <summary>
    /// Cycle the index so its always valid. 
    /// </summary>
    private void StepNextColor()
    {
        if (rowColors != null && rowColors.Count > 0)
        {
            currentIndex++;
            if (currentIndex >= rowColors.Count)
                currentIndex = 0;
        }
    }

    /// <summary>
    /// Coloring function for the board to call to color its own pegs.
    /// </summary>
    /// <param name="boardArray"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public override void RequestColor(ref List<List<PegSlotData>> boardArray, int i, int j)
    {
        for (int row = 0; row < boardArray.Count; row++)
        {
            for (int col = 0; col < boardArray[row].Count; col++)
            {
                //Swap the col and row values, access rows at the bottom of the screen first. Since the array is sideways. This may cause problems if the board becomes a non balance triangle array.
                boardArray[col][row].SetPegData(new PegData(rowColors[currentIndex]));
            }
            StepNextColor();
        }

    }

}
