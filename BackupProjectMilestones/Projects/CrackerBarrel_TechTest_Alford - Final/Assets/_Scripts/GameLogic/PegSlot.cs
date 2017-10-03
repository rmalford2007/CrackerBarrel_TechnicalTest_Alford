using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game logic is held in the PegSlotData. Game interactions with the player occur in the PegSlot class. Behaviour of the world objects occurs here.
/// </summary>
public class PegSlot : MonoBehaviour {

    public AnimationCurve blinkHoverCurve;
    public float hoverMultiplier = 3f;
    
    public event PegSlotData_Event TileSelected; //Event that occurs when this tile on the board is selected.

    private PegSlotData sourceSlotData; //The logic for this peg. Specifies if a peg is visually in this slot or not. 

    private Renderer pegHoldRenderer; //This is the renderer for the tile of this peg slot. 
    private GameObject pegObject; //The child object attached to this peg slot. This should be a 3d model of a peg
    private Renderer pegRenderer; //The color renderer for the actual peg

    private float elapsedHighlightTime = 0f;
    private Color tempColor;
    

    private PegMouseState currentMouseState = PegMouseState.DEFAULT;
    //private TileAction currentTileState = TileAction.DEFAULT; //Time ran out - Was going to use this enum for highlighting the tile sections when a tile is able to accept a peg during mouse over. Or valid jump locations.

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
	}
    
    private void OnDestroy()
    {
        UnsubscribeFromPegSlot();
    }

    /// <summary>
    /// Set the source script that this game object class belongs to. 
    /// </summary>
    /// <param name="setData"></param>
    /// <param name="isStarterSlot"></param>
    public void SetSlotData(PegSlotData setData, bool isStarterSlot=false)
    {
        sourceSlotData = setData;
        if(sourceSlotData != null)
        {
            PegAdded(sourceSlotData.GetPegColorData());
        }
        SubscibeToPegSlot();
    }

    /// <summary>
    /// Subscribe the data foundation class events for this slot. 
    /// </summary>
    void SubscibeToPegSlot()
    {
        if(sourceSlotData != null)
        {
            sourceSlotData.PegAdded += PegAdded;
            sourceSlotData.PegRemoved += PegRemoved;
            sourceSlotData.PegSelected += OnPegSlotSelected;
            sourceSlotData.PegDeselected += OnPegSlotDeselected;
        }
    }

    /// <summary>
    /// This probably doesn't matter in this games case. But just for general practice we need to unsubscribe when this object is destroyed.
    /// </summary>
    void UnsubscribeFromPegSlot()
    {
        if (sourceSlotData != null)
        {
            sourceSlotData.PegAdded -= PegAdded;
            sourceSlotData.PegRemoved -= PegRemoved;
            sourceSlotData.PegSelected -= OnPegSlotSelected;
            sourceSlotData.PegDeselected -= OnPegSlotDeselected;
            sourceSlotData.PegStartDrag -= OnPegStartDrag;
            sourceSlotData.PegStopDrag -= OnPegStopDrag;
        }
    }

    /// <summary>
    /// When this peg object begins to get dragged, hide it
    /// </summary>
    void OnPegStartDrag()
    {
        PegAdded(null);
    }

    /// <summary>
    /// When this peg object stops dragging, renabled the peg if there is color information passed in. Else this peg went somewhere else, ie jumped to another slot.
    /// When there is data here, it means the user dropped the peg off a cliff or on an invalid location, so we need to snap the peg on the mouse back to its home.
    /// </summary>
    /// <param name="droppedMousePeg"></param>
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
        //This method ended up bundled with PegAdded, but passing in null information
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
