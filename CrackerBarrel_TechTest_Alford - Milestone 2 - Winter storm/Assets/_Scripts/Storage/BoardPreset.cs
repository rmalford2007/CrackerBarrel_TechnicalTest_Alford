using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default_BoardPreset", menuName = "Create Board Preset", order = 1)]
public class BoardPreset : ScriptableObject
{
    [Tooltip("This number should be the number of pegs in the longest row of the board. In a classic game this should be 5 pegs. Changing this value will not resize the board during play, change this in edit mode.\nBounds: 5, 20 inclusive")]
    [Range(5, 20)]
    public int baseRowPegCount = 5;
    //public PegColorInfo colorData;

    internal virtual void OnEnable()
    {
        //if (colorData == null)
        //    colorData = new PegColorInfo();
    }
}
