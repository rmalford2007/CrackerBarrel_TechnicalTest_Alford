using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PegColorStyle_RowByRow : PegColorStyle {
    
    public List<Color> rowColors;

    private int currentIndex = 0;
    
    public override void OnGUI()
    {

    }

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
        Debug.Log("Request in RowByRow");
        for (int row = 0; row < boardArray.Count; row++)
        {
            for (int col = 0; col < boardArray[row].Count; col++)
            {
               
                boardArray[col][row].SetPegData(new PegData(rowColors[currentIndex]));
            }
            StepNextColor();
        }

    }

}
