using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f; // Speed when running
    public float rotationSpeed = 120f; // Speed at which the character rotates
    public float jumpSpeed = 5f;
    public float jumpButtonGracePeriod = 0.2f;

    private Animator animator;
    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Check if the Shift key is pressed to toggle between walking and running
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Update Animator parameters
        animator.SetBool("running", isRunning);

        // Continuous rotation without locking the direction
        if (horizontalInput != 0)
        {
            // Rotate around the y-axis continuously
            transform.Rotate(0, horizontalInput * rotationSpeed * Time.deltaTime, 0);
        }

        // Movement always in the direction the character is currently facing
        Vector3 movementDirection = transform.forward * verticalInput;
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * currentSpeed;

        ySpeed += Physics.gravity.y * Time.deltaTime;

        // Ground check
        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;

            // Reset jump animations when landing
            animator.SetBool("jumping", false);
            animator.SetBool("runningjump", false);
        }

        // Check if the jump button is pressed and if the character is grounded
        if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
            jumpButtonPressedTime = Time.time;
            ySpeed = jumpSpeed;  // Apply jump force

            // Check if the character is moving to differentiate jump animations
            if (verticalInput != 0 || isRunning)
            {
                animator.SetBool("runningjump", true);  // Running jump animation
            }
            else
            {
                animator.SetBool("jumping", true);  // Stationary jump animation
            }
        }

        // Handle the jump grace period logic
        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;  // Continue to apply jump speed
                jumpButtonPressedTime = null;  // Reset jump button press time
                lastGroundedTime = null;  // Reset last grounded time
            }
        }
        else
        {
            characterController.stepOffset = 0;
        }

        Vector3 velocity = movementDirection * magnitude;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        // Set animator's walking parameter based on movement
        animator.SetBool("walking", verticalInput != 0);
    }
}
