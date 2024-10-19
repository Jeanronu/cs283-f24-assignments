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

            // Calculate the direction from the lookJoint to the target
            Vector3 r = lookJoint.forward;
            Vector3 e = directionToTarget;

            // Calculate the angle of rotation
            float phi = Mathf.Atan2(Vector3.Cross(r, e).magnitude, Vector3.Dot(r, r) + Vector3.Dot(r, e));

            // Calculate the axis of rotation
            Vector3 rotationAxis = Vector3.Cross(r, e).normalized;

            // Compute the rotation
            Quaternion computedRotation = Quaternion.AngleAxis(phi * Mathf.Rad2Deg, rotationAxis);

            // Apply the computed rotation to the lookJoint's parent
            lookJoint.parent.rotation = computedRotation * lookJoint.parent.rotation;

            // Draw a debug line between the look joint and the target
            Debug.DrawLine(lookJoint.position, target.position, Color.red);
        }
    }
}
