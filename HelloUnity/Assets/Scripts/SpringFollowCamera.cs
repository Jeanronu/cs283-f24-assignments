using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringFollowCamera : MonoBehaviour
{
    // Target to follow (the player)
    public Transform target;

    // Horizontal and vertical distances from the target
    public float horizontalDistance = 5f;
    public float verticalDistance = 3f;

    // Spring constants
    public float springConstant = 50f;
    public float dampConstant = 5f;

    // Private velocity tracker
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // Get the position, forward, and up vectors of the target (player)
        Vector3 tPos = target.position;
        Vector3 tForward = target.forward;
        Vector3 tUp = target.up;

        // Calculate the ideal camera position using the target's position
        Vector3 idealEye = tPos - tForward * horizontalDistance + tUp * verticalDistance;

        // Calculate the current displacement between the camera's actual position and the ideal position
        Vector3 displacement = transform.position - idealEye;

        // Compute spring acceleration based on the spring and damping constants
        Vector3 springAccel = (-springConstant * displacement) - (dampConstant * velocity);

        // Update the camera's velocity using the spring acceleration
        velocity += springAccel * Time.deltaTime;

        // Update the camera's position by applying the velocity
        transform.position += velocity * Time.deltaTime;

        // The camera should point at the target (from the camera position to the target)
        Vector3 cameraForward = tPos - transform.position;

        // Set the camera's rotation to look at the target
        transform.rotation = Quaternion.LookRotation(cameraForward);
    }
}
