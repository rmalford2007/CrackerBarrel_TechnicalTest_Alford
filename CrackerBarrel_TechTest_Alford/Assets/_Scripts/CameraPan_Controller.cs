using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPan_Controller : MonoBehaviour {

    public AnimationCurve distanceDrivenSpeedCurve;
    public AnimationCurve zoomSpeed;

    private Camera currentCamera;

    private Vector3 startingCameraPosition;
    private Vector3 screenGrabPosition;
    private Vector3 screenDropPosition;
    private Vector3 screenCurrentPosition;
    private Vector3 dragDirection;
    private float maxZoom = 20f;

    private void Awake()
    {
        currentCamera = GetComponent<Camera>();
        startingCameraPosition = screenDropPosition = transform.position;
    }

    /// <summary>
    /// Center the camera on the board when it is created, and zoom out to capture the entire board in the camera
    /// </summary>
    /// <param name="boardTransform">Transform of the board game object</param>
    /// <param name="baseTriangleLength">The world length of 1 side of the board</param>
    public void CenterCameraOnBoard(Transform boardTransform, float baseTriangleLength)
    {
        //Calculate zoom distance based on triangle length divided by the resolution ratio
        float width = Screen.width;
        float height = Screen.height;
        
        float ratio = height / width;
        if (width < height) 
            ratio = (width + height) / 2f / width;

        SetDestination(CalculateDestination(boardTransform.position, baseTriangleLength * ratio));
    }

    /// <summary>
    /// Given the target's position, and our currently set zoom level, calculate the destination position so the camera will be directly overhead of target at distance of desiredZoom
    /// </summary>
    /// <param name="targetPosition">Vector3 position the camera needs to be over.</param>
    /// <param name="desiredZoom">Distance from the target when directly overhead</param>
    /// <returns></returns>
    Vector3 CalculateDestination(Vector3 targetPosition, float desiredZoom)
    {
        maxZoom = desiredZoom + desiredZoom / 2f;
        return targetPosition + Vector3.up * desiredZoom;
    }

    void SetDestination(Vector3 nextDestination)
    {
        screenDropPosition = nextDestination;
    }

    // Use this for initialization
    void Start () {
        
        //SetDestination(CalculateDestination(new Vector3(5f, 0f,0f), 3));
    }
	
	// Update is called once per frame
	void Update () {
        float deltaZoom = -Input.GetAxis("Mouse ScrollWheel");

        if(deltaZoom != 0f)
        {
            screenDropPosition.y = startingCameraPosition.y = Mathf.Clamp(startingCameraPosition.y + deltaZoom, 1.5f, maxZoom);
        }
        if(Input.GetButtonDown("Camera Pan"))
        {
            screenGrabPosition = Input.mousePosition;
           
            startingCameraPosition = transform.position;
        }
        if(Input.GetButton("Camera Pan"))
        {
            screenCurrentPosition = Input.mousePosition;

            dragDirection = (Camera.main.ScreenToWorldPoint(screenCurrentPosition) - Camera.main.ScreenToWorldPoint(screenGrabPosition)) * -1f; //for some reason if we don't multiply by a factor of 100, we lose some distance in the drag
            
            screenDropPosition = startingCameraPosition + dragDirection;

            //Make sure the eleveation is the same
            screenDropPosition.y = startingCameraPosition.y;
        }

        if(transform.position != screenDropPosition)
            transform.position = Vector3.MoveTowards(transform.position, screenDropPosition, Time.deltaTime * distanceDrivenSpeedCurve.Evaluate(Vector3.Distance(transform.position, screenDropPosition)));

        if (transform.position.y != currentCamera.orthographicSize)
        {
            currentCamera.orthographicSize = Mathf.MoveTowards(currentCamera.orthographicSize, transform.position.y, Time.deltaTime * zoomSpeed.Evaluate(Mathf.Abs(currentCamera.orthographicSize - transform.position.y)));
            if(Mathf.Abs(currentCamera.orthographicSize - transform.position.y) < .01f)
            {
                currentCamera.orthographicSize = transform.position.y;
            }
        }
    }
}
