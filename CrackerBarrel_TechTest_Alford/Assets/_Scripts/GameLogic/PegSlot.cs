using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegSlot : MonoBehaviour {

    public AnimationCurve blinkHoverCurve;
    public float hoverMultiplier = 3f;

    public event PegSlotData_Event TileSelected;

    private PegSlotData sourceSlotData; //The logic for this peg. Specifies if a peg is visually in this slot or not. 

    private Renderer pegHoldRenderer; //This is the renderer for the tile of this peg slot. 
    private GameObject pegObject; //The child object attached to this peg slot. This should be a 3d model of a peg
    private Renderer pegRenderer;

    private float elapsedHighlightTime = 0f;
    private Color tempColor;
    

    private PegMouseState currentMouseState = PegMouseState.DEFAULT;
    private TileAction currentTileState = TileAction.DEFAULT;

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
        //Leave early if controls are disabled
        if (!GameManager.IsControlEnabled())
            return;

        //Peg specific coloring updates
        if (pegObject.activeSelf && currentMouseState != PegMouseState.DEFAULT && currentMouseState != PegMouseState.SELECTED)
        {

            if (currentMouseState == PegMouseState.DRAG)
            {
                elapsedHighlightTime += Time.deltaTime;
            }
            else if (currentMouseState == PegMouseState.HOVER)
            {
                elapsedHighlightTime += Time.deltaTime * hoverMultiplier;
            }

            tempColor = pegRenderer.material.color;
            tempColor.a = blinkHoverCurve.Evaluate(elapsedHighlightTime);
            pegRenderer.material.color = tempColor;
        }

        //Peg slot specific updates
        if(pegObject.activeSelf == false)
        {
            //This is the color animation for showing landable locations when a peg is selected
            if(currentTileState == TileAction.LANDING)
            {

            }

            //This is the color animation for showing non landable locations when a peg is selected.
            if(currentTileState == TileAction.DISABLE)
            {

            }
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
            sourceSlotData.PegSelected += OnPegSlotSelected;
            sourceSlotData.PegDeselected += OnPegSlotDeselected;
            sourceSlotData.PegStartDrag += OnPegStartDrag;
            sourceSlotData.PegStopDrag += OnPegStopDrag;
        }
    }

    void UnsubscribeFromPegSlot()
    {
        if (sourceSlotData != null)
        {
            sourceSlotData.PegAdded -= PegAdded;
            sourceSlotData.PegRemoved -= PegRemoved;
            sourceSlotData.PegSelected -= OnPegSlotSelected;
            sourceSlotData.PegDeselected -= OnPegSlotDeselected;
        }
    }

    void OnPegStartDrag()
    {
        PegAdded(null);
    }

    void OnPegStopDrag(PegData droppedMousePeg)
    {
        PegAdded(droppedMousePeg);
    }

    void OnPegSlotSelected()
    {
        //When this slot is selected, only change coloring of the peg in this slot
        if(pegObject != null && pegObject.activeSelf && pegRenderer != null)
        {
            pegRenderer.material.color = Color.green;
            currentMouseState = PegMouseState.SELECTED;
            
        }
    }

    void OnPegSlotDeselected()
    {
        //if (pegObject != null)
        //{
            
        //    if (GameManager.DragDropEnabled())
        //    {
        //        if (sourceSlotData.HasPeg())
        //            pegObject.SetActive(true);
        //    }

        //}
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
                pegObject.SetActive(true);
        }
    }

    /// <summary>
    /// When a peg is removed. Deactivate it in this slot visually.
    /// </summary>
    void PegRemoved()
    {
        if (pegObject != null)
            pegObject.SetActive(false);
    }

    private void ResetAlpha()
    {
        tempColor = pegRenderer.material.color;
        tempColor.a = 1f;
        pegRenderer.material.color = tempColor;
    }

    public void PegSlotClicked()
    {
        if (!GameManager.IsControlEnabled())
            return;

        //Tell subscribers we are clicking this tile
        if (TileSelected != null)
        {
            TileSelected.Invoke(sourceSlotData);
        }
    }

    private void OnMouseOver()
    {
        //Leave early if controls are disabled
        if (!GameManager.IsControlEnabled())
            return;

        if(currentMouseState != PegMouseState.DRAG)
            currentMouseState = PegMouseState.HOVER;
    }

    private void OnMouseEnter()
    {
        //Leave early if controls are disabled
        if (!GameManager.IsControlEnabled())
            return;

        //On enter, highlight renderer object
        if (currentMouseState != PegMouseState.DRAG)
        {
            elapsedHighlightTime = 0f;

            currentMouseState = PegMouseState.HOVER;
        }
    }

    private void OnMouseDown()
    {
        //Leave early if controls are disabled
        PegSlotClicked();
        //currentMouseState = MouseState.DRAG;
    }

    private void OnMouseExit()
    {
        //Leave early if controls are disabled
        if (!GameManager.IsControlEnabled())
            return;

        if (currentMouseState != PegMouseState.DRAG)
        {
            currentMouseState = PegMouseState.DEFAULT;
            ResetAlpha();
        }
    }

    private void OnMouseUp()
    {
        //Leave early if controls are disabled
        if (!GameManager.IsControlEnabled())
            return;

        currentMouseState = PegMouseState.DEFAULT;
        ResetAlpha();
    }
}
