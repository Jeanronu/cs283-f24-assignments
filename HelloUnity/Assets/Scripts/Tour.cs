using UnityEngine;

public class Tour : MonoBehaviour
{
    public Transform[] pointsOfInterest;  // Array of POIs (empty GameObjects)
    public float speed = 2.0f;            // Speed of the camera movement

    private int currentPOIIndex = -1;     // Index of the current POI (-1 means no POI at start)
    private Vector3 targetPosition;       // Target position for Lerp
    private Quaternion targetRotation;    // Target rotation for Slerp
    private float transitionDuration;     // Duration of the transition
    private float transitionProgress = 1.0f;  // Progress of the transition (start fully transitioned)

    private Transform cameraTransform;    // Reference to the camera transform

    void Start()
    {
        // Get the camera transform (assuming it's a child of this GameObject)
        cameraTransform = GetComponentInChildren<Camera>().transform;

        // Make the cursor invisible at the start
        Cursor.visible = false;
    }

    void Update()
    {
        // Check if the 'N' key is pressed
        if (Input.GetKeyDown(KeyCode.N) && pointsOfInterest.Length > 0)
        {
            // Move to the next POI
            currentPOIIndex = (currentPOIIndex + 1) % pointsOfInterest.Length;
            StartTransition();
        }

        // If we are in a transition, perform the transition
        if (transitionProgress < 1.0f)
        {
            transitionProgress += Time.deltaTime / transitionDuration;
            // Lerp for position and Slerp for rotation using the camera's transform
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, transitionProgress);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, transitionProgress);
        }

        // Check for mouse click to hide the cursor
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.visible = false; // Hide the cursor when clicking the game window
        }

        // Check for Escape key to show the cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true; // Show the cursor when Escape is pressed
        }
    }

    private void StartTransition()
    {
        // Set the target position and rotation to the selected POI's position and rotation
        targetPosition = pointsOfInterest[currentPOIIndex].position;
        targetRotation = pointsOfInterest[currentPOIIndex].rotation;

        // Compute the duration of the transition based on the speed
        float distance = Vector3.Distance(cameraTransform.position, targetPosition);
        transitionDuration = distance / speed;

        // Reset the transition progress to start moving
        transitionProgress = 0f;
    }
}
