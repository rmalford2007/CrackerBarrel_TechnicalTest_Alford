using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPan_Controller : MonoBehaviour {
    private Camera currentCamera;

    private Vector3 startingPosition;
    private Vector3 destinationPosition;
    private float totalTravelDistance = 0f;
    private float currentTravelDistance = 0f;
    

    private float moveSpeed = 1f;
    private void Awake()
    {
        currentCamera = GetComponent<Camera>();
        startingPosition = destinationPosition = transform.position;
    }

    /// <summary>
    /// Center the camera on the board when it is created, and zoom out to capture the entire board in the camera
    /// </summary>
    /// <param name="boardTransform">Transform of the board game object</param>
    /// <param name="baseTriangleLength">The world length of 1 side of the board</param>
    public void CenterCameraOnBoard(Transform boardTransform, float baseTriangleLength)
    {
        //Calculate zoom distance based on triangle length divided by the resolution ratio
        SetDestination(CalculateDestination(boardTransform.position, baseTriangleLength / ((float)Screen.currentResolution.width / (float)Screen.currentResolution.width)));
    }

    /// <summary>
    /// Given the target's position, and our currently set zoom level, calculate the destination position so the camera will be directly overhead of target at distance of desiredZoom
    /// </summary>
    /// <param name="targetPosition">Vector3 position the camera needs to be over.</param>
    /// <param name="desiredZoom">Distance from the target when directly overhead</param>
    /// <returns></returns>
    Vector3 CalculateDestination(Vector3 targetPosition, float desiredZoom)
    {
        return targetPosition + Vector3.up * desiredZoom;
    }

    void SetDestination(Vector3 nextDestination)
    {
        startingPosition = transform.position;
        destinationPosition = nextDestination;
        totalTravelDistance = Vector3.Distance(startingPosition, destinationPosition);
        currentTravelDistance = 0f;
        
    }

    // Use this for initialization
    void Start () {
        
        //SetDestination(CalculateDestination(new Vector3(5f, 0f,0f), 3));
    }
	
	// Update is called once per frame
	void Update () {
		if(transform.position != destinationPosition)
        {
            currentTravelDistance += Time.deltaTime * moveSpeed;

            transform.position = Vector3.Lerp(startingPosition, destinationPosition, currentTravelDistance / totalTravelDistance);

            //Set the orthographic size to the elveation of the camera sinc elevation of camera doesn't matter in orthographic - aside from clipping 
            currentCamera.orthographicSize = transform.position.y;
        }
	}
}
