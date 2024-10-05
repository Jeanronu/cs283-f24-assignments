using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidFollowCamera : MonoBehaviour
{
    // Target to follow (the player)
    public Transform target;

    // Horizontal and vertical distances from the target
    public float horizontalDistance = 5f;
    public float verticalDistance = 3f;

    void LateUpdate()
    {
        if (target == null) return;

        // Get the position, forward, and up vectors of the target (player)
        Vector3 tPos = target.position;
        Vector3 tForward = target.forward;
        Vector3 tUp = target.up;

        // Calculate the new camera position using the formula:
        // eye = tPos - tForward * horizontalDistance + tUp * verticalDistance
        Vector3 eye = tPos - tForward * horizontalDistance + tUp * verticalDistance;

        // Calculate the direction the camera should face (from the camera to the player)
        Vector3 cameraForward = tPos - eye;

        // Set the camera's position
        transform.position = eye;

        // Set the camera's rotation to look at the player
        transform.rotation = Quaternion.LookRotation(cameraForward);
    }
}
