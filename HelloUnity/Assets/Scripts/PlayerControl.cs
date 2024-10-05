using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    // Parameters for movement and rotation
    public float moveSpeed = 5f;
    public float turnSpeed = 100f;
    public float jumpSpeed = 5f;
    private float ySpeed;
    private CharacterController characterController;
    
    // Gravity constant
    private float gravity = -9.81f;

    // Animator component and walking/jumping state
    private Animator animator; 
    private bool isWalking = false;  // Boolean to track if the player is walking
    private bool isJumping = false;  // Boolean to track if the player is jumping

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();  // Get the CharacterController component
        animator = GetComponent<Animator>();  // Get the Animator component
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float moveDirection = 0f;
        float turnDirection = 0f;

        // Detect key presses for movement and rotation
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            moveDirection = Input.GetKey(KeyCode.W) ? 1f : -1f; // Move forward or backward
            isWalking = true; // Player is walking
        }
        else
        {
            isWalking = false; // Player stops walking
        }

        if (Input.GetKey(KeyCode.A))
        {
            turnDirection = -1f;  // Turn left
        }
        else if (Input.GetKey(KeyCode.D))
        {
            turnDirection = 1f;  // Turn right
        }

        // Apply rotation (turning the player left or right)
        transform.Rotate(Vector3.up, turnDirection * turnSpeed * Time.deltaTime);

        // Handle jumping when space is pressed and the player is grounded
        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            ySpeed = jumpSpeed;  // Apply jump force
            isJumping = true;  // Set the jumping state to true
            animator.SetBool("jumping", true);  // Trigger jump animation
        }

        // Apply gravity and ground check
        if (characterController.isGrounded && ySpeed < 0)
        {
            ySpeed = -0.5f;  // Small negative value to keep grounded
            isJumping = false;  // Player is no longer jumping
            animator.SetBool("jumping", false);  // Reset jump animation
        }
        ySpeed += gravity * Time.deltaTime;  // Gravity effect

        // Calculate movement vector
        Vector3 movement = transform.forward * moveDirection * moveSpeed;

        // Apply vertical movement (y-axis for jumping/falling)
        movement.y = ySpeed;

        // Move the player using CharacterController
        characterController.Move(movement * Time.deltaTime);

        // Update the animator's walking parameter
        animator.SetBool("walking", isWalking);
    }
}
