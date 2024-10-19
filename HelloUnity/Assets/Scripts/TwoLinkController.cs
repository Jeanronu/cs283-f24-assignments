using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoLinkController : MonoBehaviour
{
    public Transform target;          // The target to reach
    public Transform endEffector;     // The end of the limb (e.g., the hand)
    public Transform middleJoint;     // The middle joint (e.g., the elbow)
    public Transform baseJoint;       // The base joint (e.g., the shoulder)

    public float tolerance = 0.01f;   // Tolerance for checking if the target is already at the end effector's position

    void Update()
    {
        if (target != null && endEffector != null && middleJoint != null && baseJoint != null)
        {

            // Check if the target is already at the end effector's position
            if (Vector3.Distance(endEffector.position, target.position) < tolerance)
            {
                // If the target is already at the end effector's position, no rotate the joints
                return;
            }

            // Calculate the distance between the base joint and the target
            float distToTarget = Vector3.Distance(baseJoint.position, target.position);
            float upperArmLength = Vector3.Distance(baseJoint.position, middleJoint.position);
            float forearmLength = Vector3.Distance(middleJoint.position, endEffector.position);

            // Ensure that the target is within reach
            float totalLimbLength = upperArmLength + forearmLength;
            if (distToTarget > totalLimbLength)
            {
                // If the target is out of reach, clamp the target distance
                Vector3 direction = (target.position - baseJoint.position).normalized;
                target.position = baseJoint.position + direction * totalLimbLength;
            }

            //Rotate the base joint to point towards the target
            Vector3 baseToTarget = (target.position - baseJoint.position).normalized;
            Quaternion baseRotation = Quaternion.LookRotation(baseToTarget, baseJoint.up);
            baseJoint.rotation = Quaternion.Slerp(baseJoint.rotation, baseRotation, Time.deltaTime * 5f);

            //Calculate the angle for the middle joint
            float a = upperArmLength;
            float b = forearmLength;
            float c = distToTarget;

            // Calculate the elbow/middlejoint angle
            float angleMiddle = Mathf.Acos((a * a + b * b - c * c) / (2 * a * b)) * Mathf.Rad2Deg;

            // Apply the elbow rotation by rotating the local Z axis
            middleJoint.localRotation = Quaternion.Euler(0, 0, angleMiddle - 90f);  // Subtracting 90 to align it properly

            // Draw debug lines for visualization
            Debug.DrawLine(baseJoint.position, middleJoint.position, Color.red);
            Debug.DrawLine(middleJoint.position, endEffector.position, Color.blue);
            Debug.DrawLine(endEffector.position, target.position, Color.green);
        }
    }
}
