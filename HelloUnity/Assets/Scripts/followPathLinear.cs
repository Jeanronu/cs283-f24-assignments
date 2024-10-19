using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathLinear : MonoBehaviour
{
    public List<Transform> pathPoints;  // List of points for the path
    public float speed = 5.0f;          // Speed that character moves

    private int currentPoint = 0;       // Current point on the path
    private bool isMoving = false;      // Whether the character is moving

    void Start()
    {
        if (pathPoints.Count > 1)
        {
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        while (true)
        {
            if (!isMoving && currentPoint < pathPoints.Count - 1)
            {
                isMoving = true;
                yield return StartCoroutine(MoveToNextPoint(pathPoints[currentPoint], pathPoints[currentPoint + 1]));
                currentPoint++;
            }
            yield return null;
        }
    }

    IEnumerator MoveToNextPoint(Transform start, Transform end)
    {
        float distance = Vector3.Distance(start.position, end.position);
        float journeyLength = distance / speed;
        float journeyTime = 0f;

        while (journeyTime < journeyLength)
        {
            journeyTime += Time.deltaTime;
            float t = journeyTime / journeyLength;
            transform.position = Vector3.Lerp(start.position, end.position, t);
            transform.rotation = Quaternion.Slerp(start.rotation, end.rotation, t);
            yield return null;
        }

        isMoving = false;
    }

    // For visualizing the path in the Unity Editor
    void OnDrawGizmos()
    {
        if (pathPoints.Count > 1)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
            }
        }
    }
}
