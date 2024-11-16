using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wander : MonoBehaviour
{
    public float wanderRadius = 10f; // Radius within which the NPC will pick a new random destination
    public float minDistanceToTarget = 1f; // Minimum distance to consider the target "reached"

    public NavMeshAgent agent;

    void Start()
    {
        // Get the NavMeshAgent component attached to this object
        agent = GetComponent<NavMeshAgent>();

        // Set the initial random destination
        SetNewDestination();
    }

    void Update()
    {
        // Check if the agent has reached its current destination
        if (!agent.pathPending && agent.remainingDistance <= minDistanceToTarget)
        {
            SetNewDestination(); // Pick a new random destination
        }
    }

    public void SetNewDestination()
    {
        // Generate a random direction within the specified radius
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        // Sample a valid point on the NavMesh near the random direction
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            // Set the agent's destination to this valid point on the NavMesh
            agent.SetDestination(hit.position);
        }
    }
}
