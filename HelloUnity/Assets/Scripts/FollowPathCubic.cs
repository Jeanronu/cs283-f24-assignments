using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathCubic : MonoBehaviour
{
    public List<Transform> pathPoints;  // List of control points for the path
    public float speed = 5.0f;          // Speed that character moves
    public bool useDeCasteljau = false; // If true, use De Casteljau's algorithm

    private int currentSegment = 0;
    private bool isMoving = false;

    void Start()
    {
        if (pathPoints.Count >= 2)
        {
            StartCoroutine(FollowBezierPath());
        }
    }

    IEnumerator FollowBezierPath()
    {
        while (true)
        {
            if (!isMoving && currentSegment < pathPoints.Count - 1)
            {
                isMoving = true;
                yield return StartCoroutine(MoveAlongBezier(currentSegment));
                currentSegment++;
            }
            yield return null;
        }
    }

    IEnumerator MoveAlongBezier(int segment)
    {
        float journeyTime = 0f;
        float segmentDuration = 1f / speed;

        while (journeyTime < segmentDuration)
        {
            journeyTime += Time.deltaTime;
            float t = journeyTime / segmentDuration;

            Vector3 newPosition;
            Vector3 forwardDirection;
            if (useDeCasteljau)
            {
                newPosition = DeCasteljauRecursive(t, pathPoints);
                forwardDirection = DeCasteljauRecursive(Mathf.Min(t + 0.01f, 1f), pathPoints) - newPosition;
            }
            else
            {
                newPosition = Bezier(t, pathPoints);
                forwardDirection = Bezier(Mathf.Min(t + 0.01f, 1f), pathPoints) - newPosition;
            }

            // Update the character's position
            transform.position = newPosition;

            // Character faces the direction it's moving in
            if (forwardDirection != Vector3.zero)
            {
                transform.forward = forwardDirection.normalized;
            }

            yield return null;
        }

        isMoving = false;
    }

    // General Bezier formula (n-point curve using De Casteljau's algorithm)
    Vector3 DeCasteljauRecursive(float t, List<Transform> points)
    {
        if (points.Count == 1) return points[0].position;

        List<Transform> newPoints = new List<Transform>();
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 lerpedPosition = Vector3.Lerp(points[i].position, points[i + 1].position, t);
            GameObject temp = new GameObject();  // Temporary object to store the new position
            temp.transform.position = lerpedPosition;
            newPoints.Add(temp.transform);
        }

        return DeCasteljauRecursive(t, newPoints);
    }

    // General Bezier for any number of points
    Vector3 Bezier(float t, List<Transform> points)
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (var point in points)
        {
            positions.Add(point.position);
        }

        while (positions.Count > 1)
        {
            List<Vector3> newPositions = new List<Vector3>();
            for (int i = 0; i < positions.Count - 1; i++)
            {
                newPositions.Add(Vector3.Lerp(positions[i], positions[i + 1], t));
            }
            positions = newPositions;
        }

        return positions[0];
    }

    // Visualizing the Bezier curve with red lines and blue spheres at control points
    void OnDrawGizmos()
    {
        if (pathPoints.Count >= 2)
        {
            Gizmos.color = Color.blue;

            // Draw blue spheres at control points
            foreach (var point in pathPoints)
            {
                Gizmos.DrawSphere(point.position, 0.2f); // Spheres at each control point
            }

            // Draw the Bezier curve with red lines
            Gizmos.color = Color.red;
            Vector3 previousPoint = pathPoints[0].position;
            for (float t = 0; t <= 1; t += 0.05f)
            {
                Vector3 point = Bezier(t, pathPoints);
                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }
        }
    }
}
