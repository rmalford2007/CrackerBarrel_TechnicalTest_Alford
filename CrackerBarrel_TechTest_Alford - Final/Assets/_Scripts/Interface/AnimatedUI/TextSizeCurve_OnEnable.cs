using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Provides text animation for a text object each time it is enabled from a disable state. Entry animation and persistent idle animation.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
[System.Serializable]
public class TextSizeCurve_OnEnable : MonoBehaviour
{
    [Tooltip("This is the additional font size percent we are adding to the starting font size. This plays from the beginning when OnEnable occurs")]
    public AnimationCurve sizeCurve;

    [Tooltip("This is the additional font size percent we are adding to the starting font size plus the hover size curve. This only occurs when the initial hover curve is completed. This should be a faint pulse if the player leaves his mouse over a button.")]
    public AnimationCurve idlePulseCurve;

    [Tooltip("Flag to control whether there is an idle pulse after the enable animation.")]
    public bool useIdlePulse = true;

    private TMP_Text theText;
    private float startingFontSize;
    private float animationCurveTime = 0f;
    private float elapsedTime = 0f;

    // Use this for initialization
    void Start()
    {
        theText = GetComponent<TMP_Text>();

        //if we are missing the pulse curve, turn off the flag
        if (idlePulseCurve == null)
            useIdlePulse = false;

        if (theText == null)
            Destroy(this);

        startingFontSize = theText.fontSize;
        animationCurveTime = sizeCurve.keys[sizeCurve.length - 1].time;
    }

    private void OnEnable()
    {
        elapsedTime = 0f;
        if (theText != null)
            theText.fontSize = startingFontSize + startingFontSize * sizeCurve.Evaluate(elapsedTime);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;
        if (elapsedTime < animationCurveTime)
        {
            elapsedTime = Mathf.Clamp(elapsedTime, 0.0f, animationCurveTime);
            theText.fontSize = startingFontSize + startingFontSize * sizeCurve.Evaluate(elapsedTime);
        }
        else if (useIdlePulse && elapsedTime >= animationCurveTime)
        {
            theText.fontSize = startingFontSize + startingFontSize * sizeCurve.Evaluate(animationCurveTime) + startingFontSize * idlePulseCurve.Evaluate(elapsedTime - animationCurveTime);
        }
    }
}
