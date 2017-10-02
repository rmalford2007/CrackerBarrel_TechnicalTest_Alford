using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PegColorInfo {

    [Tooltip("The color of this peg information.")]
    public Color pegColor;

    [Range(0f, 1f)]
    public float percentVal = 1f;

	public PegColorInfo()
    {
        pegColor = Color.blue;
        percentVal = 1f;
    }
}
