using UnityEngine;

public class GazeController : MonoBehaviour
{
    public Transform target;       // Target to follow
    public Transform lookJoint;    // The joint that will point towards the target (e.g., head, eyes)

    void Update()
    {
        if (target != null && lookJoint != null)
        {
            // Vector from the joint to the target
            Vector3 directionToTarget = target.position - lookJoint.position;

            // Ensure the direction vector is normalized
            directionToTarget.Normalize();

            // Calculate the target rotation based on the direction to the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

            // Rotate the lookJoint towards the target using Slerp
            lookJoint.rotation = Quaternion.Slerp(lookJoint.rotation, targetRotation, Time.deltaTime * 5f);

            // Draw a debug line between the look joint and the target
            Debug.DrawLine(lookJoint.position, target.position, Color.red);
        }
    }
}
