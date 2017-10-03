using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a preset to control coloring and board sizes. There may be multiple presets the player can choose from at some point.
/// </summary>
[CreateAssetMenu(fileName = "Default_BoardPreset", menuName = "Create Board Preset", order = 1)]
public class BoardPreset : ScriptableObject
{
    [Tooltip("This number should be the number of pegs in the longest row of the board. In a classic game this should be 5 pegs. Changing this value will not resize the board during play, change this in edit mode.\nBounds: 5, 20 inclusive")]
    [Range(5, 20)]
    public int baseRowPegCount = 5;
    
    [Tooltip("Display settings for the row by row style of this preset.")]
    public PegColorStyle_RowByRow rowByRowData;

    //[HideInInspector]
    [Tooltip("Display settings for the single color style of this preset.")]
    public PegColorStyle_SingleColor singleData;

    [Tooltip("Active style for the a created board that has this preset. Changing this will change between the display styles above.")]
    public ColorStyle selectedStyle = ColorStyle.SINGLE_COLOR;

    //You should add new enums here whenever you make a new style, since we ran into problems with class shearing, gotta manually add it and create a style variable above.
    public enum ColorStyle
    {
        ROW_BY_ROW,
        SINGLE_COLOR
    }

    /// <summary>
    /// Initialize the scriptable object since there is no constructor.
    /// </summary>
    public void OnEnable()
    {
        if (rowByRowData == null)
            rowByRowData = new PegColorStyle_RowByRow();
        if (singleData == null)
            singleData = new PegColorStyle_SingleColor();

    }

    /// <summary>
    /// Call the request color function, based on the currently chosen style. A game board should pass itself in here when it is ready to be colored. 
    /// </summary>
    /// <param name="boardArray"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public virtual void RequestColor(ref List<List<PegSlotData>> boardArray, int i, int j)
    {
        //When new styles are created add them to this switch
        switch(selectedStyle)
        {
            case ColorStyle.SINGLE_COLOR:
                if (singleData != null)
                    singleData.RequestColor(ref boardArray, i, j);
                break;
            case ColorStyle.ROW_BY_ROW:
                if (rowByRowData != null)
                    rowByRowData.RequestColor(ref boardArray, i, j);
                break;
            default:
                Debug.Log("Style not supported in RequestColor switch.");
                break;
        }
        
    }

}
