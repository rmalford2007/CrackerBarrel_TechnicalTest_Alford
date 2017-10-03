using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Provides text animations for a text object that is being moused over.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
[System.Serializable]
public class TextGrow_MouseHover : EventTrigger
{
    [Tooltip("This is the additional font size percent we are adding to the starting font size. This plays forward when hover begins, and backwards when leaving the hovered state.")]
    public AnimationCurve sizeCurve;

    [Tooltip("This is the additional font size percent we are adding to the starting font size plus the hover size curve. This only occurs when the initial hover curve is completed. This should be a faint pulse if the player leaves his mouse over a button.")]
    public AnimationCurve idlePulseCurve;

    public bool isInteractable;
    private TMP_Text theText;
    private float startingFontSize;
    private float animationCurveTime = 0f;
    private float elapsedTime = 0f;
    private bool isForward = false;
    private Button myButton;
    // Use this for initialization
    void Start()
    {
        myButton = GetComponent<Button>();
        if (myButton != null)
        {
            isInteractable = myButton.interactable;
        }

        theText = GetComponent<TMP_Text>();
        if (theText == null)
            Destroy(this);
        startingFontSize = theText.fontSize;
        animationCurveTime = sizeCurve.keys[sizeCurve.length - 1].time;

    }

    private void OnEnable()
    {
        isForward = false;
        elapsedTime = 0f;
        if (theText != null)
            theText.fontSize = startingFontSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInteractable != myButton.interactable)
        {
            isInteractable = myButton.interactable;
        }

        if (isInteractable)
        {
            elapsedTime += (Time.unscaledDeltaTime * (isForward ? 1.0f : -1.0f));
            if ((isForward && elapsedTime < animationCurveTime) || (!isForward && elapsedTime > 0.0f))
            {
                elapsedTime = Mathf.Clamp(elapsedTime, 0.0f, animationCurveTime);
                theText.fontSize = startingFontSize + startingFontSize * sizeCurve.Evaluate(elapsedTime);
            }
            else if(isForward && elapsedTime >= animationCurveTime)
            {
                theText.fontSize = startingFontSize + startingFontSize * sizeCurve.Evaluate(animationCurveTime) + startingFontSize * idlePulseCurve.Evaluate(elapsedTime - animationCurveTime);
            }
        }
        else
        {
            elapsedTime += (Time.unscaledDeltaTime * (-1.0f));
            if (!isForward && elapsedTime > 0.0f)
            {
                elapsedTime = Mathf.Clamp(elapsedTime, 0.0f, animationCurveTime);
                theText.fontSize = startingFontSize + startingFontSize * sizeCurve.Evaluate(elapsedTime);
            }
        }
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        isForward = true;
    }

    public override void OnPointerExit(PointerEventData data)
    {
        isForward = false;
    }
}
