using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegSlot : MonoBehaviour {

    public AnimationCurve blinkHoverCurve;
    public float hoverMultiplier = 3f;
    private PegSlotData sourceSlotData; //The logic for this peg. Specifies if a peg is visually in this slot or not. 

    private GameObject pegObject; //The child object attached to this peg slot. This should be a 3d model of a peg
    private Renderer pegRenderer;

    private float elapsedHighlightTime = 0f;
    private Color tempColor;
    public enum MouseState
    {
        HOVER,
        DRAG,
        DEFAULT
    }

    private MouseState currentMouseState = MouseState.DEFAULT;

    private void Awake()
    {
        pegObject = transform.GetChild(0).gameObject;
        pegRenderer = pegObject.GetComponent<MeshRenderer>();
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		if(pegObject.activeSelf && currentMouseState != MouseState.DEFAULT)
        {

            if (currentMouseState == MouseState.DRAG)
            {
                elapsedHighlightTime += Time.deltaTime;
            }
            else if (currentMouseState == MouseState.HOVER)
            {
                elapsedHighlightTime += Time.deltaTime * hoverMultiplier;
            }

            tempColor = pegRenderer.material.color;
            tempColor.a = blinkHoverCurve.Evaluate(elapsedHighlightTime);
            pegRenderer.material.color = tempColor;
        }
	}

    private void OnDestroy()
    {
        UnsubscribeFromPegSlot();
    }

    public void SetSlotData(PegSlotData setData, bool isStarterSlot=false)
    {
        sourceSlotData = setData;
        if(sourceSlotData != null)
        {
            PegAdded(sourceSlotData.GetPegColorData());
        }
        SubscibeToPegSlot();
    }

    void SubscibeToPegSlot()
    {
        if(sourceSlotData != null)
        {
            sourceSlotData.PegAdded += PegAdded;
            sourceSlotData.PegRemoved += PegRemoved;
        }
    }

    void UnsubscribeFromPegSlot()
    {
        if (sourceSlotData != null)
        {
            sourceSlotData.PegAdded -= PegAdded;
            sourceSlotData.PegRemoved -= PegRemoved;
        }
    }

    /// <summary>
    /// Hook for updating the peg in this slot and activating it.
    /// </summary>
    /// <param name="pegInformation">The peg color data.</param>
    void PegAdded(PegData pegInformation)
    {
        //If we get null info, turn the peg off
        if (pegInformation == null)
            PegRemoved();
        else
        {
            //Set the renderer color to the peg color
            if (pegRenderer != null)
            {
                pegRenderer.material.color = pegInformation.pegColor;
            }
            if(pegObject != null && pegObject.activeSelf == false)
                Debug.Log("SetActive");
            if (pegObject != null)
                pegObject.SetActive(true);
        }
    }

    /// <summary>
    /// When a peg is removed. Deactivate it in this slot visually.
    /// </summary>
    void PegRemoved()
    {
        Debug.Log("Peg Removed");
        if (pegObject != null)
            pegObject.SetActive(false);
    }

    private void ResetAlpha()
    {
        tempColor = pegRenderer.material.color;
        tempColor.a = 1f;
        pegRenderer.material.color = tempColor;
    }

    private void OnMouseOver()
    {
        if(currentMouseState != MouseState.DRAG)
            currentMouseState = MouseState.HOVER;
    }

    private void OnMouseEnter()
    {
        //On enter, highlight renderer object
        if (currentMouseState != MouseState.DRAG)
        {
            elapsedHighlightTime = 0f;

            currentMouseState = MouseState.HOVER;
        }
    }

    private void OnMouseDown()
    {
        currentMouseState = MouseState.DRAG;
    }

    private void OnMouseExit()
    {
        if (currentMouseState != MouseState.DRAG)
        {
            currentMouseState = MouseState.DEFAULT;
            ResetAlpha();
        }
    }

    private void OnMouseUp()
    {
        currentMouseState = MouseState.DEFAULT;
        ResetAlpha();
    }
}
