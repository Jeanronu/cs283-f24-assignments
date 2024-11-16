using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using BTAI; // Behavior Tree AI library

public class BehaviorMinion : MonoBehaviour
{
    // Public variables to be set in the Unity Inspector
    public Transform player;                      // Reference to the player object
    public Vector3 fixedCenter = new Vector3(129, 0, 166); // Center of the movement area
    public float fixedRadius = 10f;               // Radius of the movement area
    public float checkInterval = 0.5f;            // Interval to check player distance
    public float attackRange = 1.5f;              // Range within which NPC attacks the player
    public bool moving = false;                   // Tracks if the NPC is moving
    public bool attack = false;                   // Tracks if the NPC is in attack range

    private Root m_btRoot = BT.Root();            // Root node for behavior tree
    private NavMeshAgent agent;                   // Reference to the NavMeshAgent component for movement
    private Animator animator;                    // Reference to the Animator component for animation control

    void Start()
    {
        // Get the NavMeshAgent and Animator components attached to the NPC
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Create a behavior tree node to follow the player if in range
        BTNode followPlayer = BT.RunCoroutine(FollowPlayerIfInRange);

        // Create a sequence of nodes in the behavior tree
        Sequence sequence = BT.Sequence();
        sequence.OpenBranch(followPlayer); // Add the followPlayer behavior to the sequence
        m_btRoot.OpenBranch(sequence); // Open the behavior tree branch

        // The NPC will execute the behavior tree in Update
    }

    void Update()
    {
        // Execute the behavior tree
        m_btRoot.Tick();

        // Update the animator with the current moving and attack states
        if (animator != null)
        {
            animator.SetBool("moving", moving); // Update moving state in the animator
            animator.SetBool("attack", attack); // Update attack state in the animator
        }

        // If the NPC is attacking, make sure it faces the player
        if (attack)
        {
            FacePlayer(); // Call the method to face the player during an attack
        }
    }

    // Method to make the NPC face the player smoothly
    void FacePlayer()
    {
        // Calculate the direction from the NPC to the player
        Vector3 direction = (player.position - transform.position).normalized;

        // Create a rotation that looks in the direction of the player
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Smoothly rotate the NPC towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Coroutine to follow the player if within the specified radius
    IEnumerator<BTState> FollowPlayerIfInRange()
    {
        while (true)
        {
            // Calculate the distance from the player to the center of the movement area
            float distanceToCenter = Vector3.Distance(player.position, fixedCenter);

            // Check if the player is within the fixed radius
            if (distanceToCenter <= fixedRadius)
            {
                // Player is within the radius; start chasing the player
                agent.SetDestination(player.position); // Set the player's position as the destination for the NPC
                moving = true; // NPC is now moving

                // Keep following while the player is within the radius
                while (distanceToCenter <= fixedRadius)
                {
                    // Continuously update the destination to the player's position
                    agent.SetDestination(player.position);

                    // Update the distance to the center of the movement area
                    distanceToCenter = Vector3.Distance(player.position, fixedCenter);

                    // Calculate the distance from the NPC to the player to check for attack range
                    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                    // If the player is within the attack range, stop moving and start attacking
                    if (distanceToPlayer <= attackRange)
                    {
                        attack = true; // Player is close enough to attack
                        moving = false; // Stop moving while attacking
                        agent.ResetPath(); // Clear the path to stop the NPC from moving
                    }
                    else
                    {
                        attack = false; // Player is out of attack range
                        moving = true; // NPC continues to chase the player
                    }

                    yield return BTState.Continue; // Continue the coroutine and recheck on the next frame
                }
            }
            else
            {
                // Player is outside the radius; NPC idles or stops
                agent.ResetPath(); // Reset the NavMeshAgent path
                moving = false; // NPC is no longer moving
                attack = false; // NPC is not attacking
            }

            // Wait for the specified interval before checking again
            float elapsed = 0f;
            while (elapsed < checkInterval)
            {
                elapsed += Time.deltaTime; // Track elapsed time
                yield return BTState.Continue; // Continue the coroutine until the interval has passed
            }
        }
    }
}