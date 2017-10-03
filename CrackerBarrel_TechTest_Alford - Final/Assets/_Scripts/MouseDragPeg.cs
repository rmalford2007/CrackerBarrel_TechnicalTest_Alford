using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDragPeg : MonoBehaviour {
    public static MouseDragPeg Instance;

    [Tooltip("The distance from the camera to drop pegs onto the board. This should be further than the lowest board on the scene. May get pegs stuck to your mouse if this value is too small.")]
    public float raycastDropDistance = 100f;

    private GameObject pegObject; //peg game object that is hidden under the mouse
    private Renderer pegRenderer; //peg color control that syncs color with the peg being grabbed
    private PegSlotData activePeg;//the currently held peg
    private PegSlot activePegSlot;//the currently held peg's home game object script. Pegs should be returned home when bad things happen.

    private bool postPauseReset = false; //flag that tracks if pause has occured while holding a peg. Reset the peg to home when unpaused.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            activePegSlot = null;
            activePeg = null;

            //If there are no children, destroy now
            if (transform.childCount == 0)
                DestroyImmediate(this);
            else
            {
                pegObject = transform.GetChild(0).gameObject;
                if (pegObject != null)
                {
                    pegRenderer = pegObject.GetComponent<Renderer>();
                }
            }
        }
        else
            DestroyImmediate(this.gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    // Use this for initialization
    void Start () {
        pegObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if(!GameManager.IsControlEnabled())
        {
            if (activePeg != null && activePegSlot != null)
            {
                postPauseReset = true;
            }

            return;
        }
        if(postPauseReset)
        {
            if (GameManager.IsControlEnabled())
            {
                if (activePeg != null && activePegSlot != null)
                {
                    activePegSlot.PegSlotClicked();
                    postPauseReset = false;
                }
            }
            else
                return;
        }
        if(activePeg != null)
        {
            //Sync this transform position to the mouse location
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.y = transform.position.y;
            transform.position = mousePos;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (activePeg != null)
            {
                Vector3 rayOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit hitInfo;

                //Drop held peg onto another peg, or into the abyss. Note if the game is paused when this occurs it drops the peg into the abyss as well
                if(GameManager.IsControlEnabled() && Physics.Raycast(rayOrigin, -Vector3.up, out hitInfo, raycastDropDistance))
                {
                    //The only colliders in the game are peg slots
                    PegSlot pegHit = hitInfo.collider.gameObject.GetComponent<PegSlot>();
                    if(pegHit != null)
                    {
                        pegHit.PegSlotClicked();
                    }
                }
                else
                {
                    if(activePegSlot != null)
                        activePegSlot.PegSlotClicked();
                }
            }
        }
	}

    public void ActivatePeg(PegSlotData pegInHand)
    {
        activePeg = pegInHand;

        if (activePeg != null)
        {
            if (activePeg.HasPeg())
            {
                SyncPegInfo(activePeg.GetPegColorData());
            }

        }
        else
            pegObject.SetActive(false);
    }

    void SyncPegInfo(PegData pegInformation)
    {
        //Set the renderer color to the peg color
        if (pegRenderer != null)
        {
            pegRenderer.material.color = pegInformation.pegColor;
        }
        if (pegObject != null && pegObject.activeSelf == false)
        {
            pegObject.SetActive(true);
        }


        //Evaluate what is under the mouse to grab the world object collider we are picking up, so when we drop a peg in the abyss, we can call click on ourselves to return it
        Vector3 rayOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(rayOrigin, -Vector3.up, out hitInfo, raycastDropDistance))
        {
            //The only colliders in the game are peg slots
            PegSlot pegHit = hitInfo.collider.gameObject.GetComponent<PegSlot>();
            if (pegHit != null)
            {
                activePegSlot = pegHit;
            }
        }
    }
}
