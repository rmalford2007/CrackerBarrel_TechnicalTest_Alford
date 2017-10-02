using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPan_Controller : MonoBehaviour {

    public AnimationCurve distanceDrivenSpeedCurve;
    public AnimationCurve zoomSpeed;
    private Camera currentCamera;

    private Vector3 startingPosition;
    private Vector3 grabPosition;
    private Vector3 targetDropPosition;
    private Vector3 currentPosition;
    private Vector3 dragDirection;
    private float maxZoom = 20f;

    private void Awake()
    {
        currentCamera = GetComponent<Camera>();
        startingPosition = targetDropPosition = transform.position;
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
        targetDropPosition = nextDestination;
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
            targetDropPosition.y = startingPosition.y = Mathf.Clamp(startingPosition.y + deltaZoom, 1.5f, maxZoom);
        }
        if(Input.GetButtonDown("Camera Pan"))
        {
            grabPosition = Input.mousePosition;
           
            startingPosition = transform.position;
        }
        if(Input.GetButton("Camera Pan"))
        {
            currentPosition = Input.mousePosition;

            dragDirection = (Camera.main.ScreenToWorldPoint(currentPosition) - Camera.main.ScreenToWorldPoint(grabPosition)) * -1f; //for some reason if we don't multiply by a factor of 100, we lose some distance in the drag
            
            targetDropPosition = startingPosition + dragDirection;
            targetDropPosition.y = startingPosition.y;
        }

        if(transform.position != targetDropPosition)
            transform.position = Vector3.MoveTowards(transform.position, targetDropPosition, Time.deltaTime * distanceDrivenSpeedCurve.Evaluate(Vector3.Distance(transform.position, targetDropPosition)));

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
