using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float mouseSensitivity = 2.0f;  // Sensitivity of mouse movement
    public float moveSpeed = 5.0f;         // Speed of camera movement

    private float rotationX = 0.0f;         // Rotation around the X axis
    private float rotationY = 0.0f;         // Rotation around the Y axis

    void Update()
    {
        // Get mouse movement
        float mouseMoveX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseMoveY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Update rotation based on mouse movement
        rotationY += mouseMoveX;
        rotationX -= mouseMoveY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Limit vertical rotation

        // Apply rotation to the camera
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);

        // Get keyboard input for movement
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection); // Move relative to camera orientation
        moveDirection.y = 0; // Prevent vertical movement with WASD keys

        // Move the camera
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
