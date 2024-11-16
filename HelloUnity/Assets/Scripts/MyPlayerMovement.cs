using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpSpeed = 5f;
    public float jumpButtonGracePeriod = 0.2f;

    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip jumpSound;

    private AudioSource audioSource;
    private Animator animator;
    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private bool isWalking = false;
    private bool isRunning = false;
    private bool isJumping = false;  // New variable to control jump animation state

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        animator.SetBool("running", isRunning);
        animator.SetBool("walking", verticalInput != 0 || horizontalInput != 0);

        Vector3 movementDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * currentSpeed;

        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;

            if (isJumping)
            {
                isJumping = false;
                animator.SetBool("jumping", false);
                animator.SetBool("runningjump", false);
            }
        }

        // Jump handling
        if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
            jumpButtonPressedTime = Time.time;
            ySpeed = jumpSpeed;

            audioSource.PlayOneShot(jumpSound);

            isJumping = true;
            if (verticalInput != 0 || isRunning)
            {
                animator.SetBool("runningjump", true);
            }
            else
            {
                animator.SetBool("jumping", true);
            }
        }

        // Jump grace period
        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0;
        }

        Vector3 velocity = movementDirection * magnitude;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        HandleMovementSounds(verticalInput, horizontalInput);
    }

    private void HandleMovementSounds(float verticalInput, float horizontalInput)
    {
        if (verticalInput != 0 || horizontalInput != 0)
        {
            if (isRunning)
            {
                if (audioSource.clip != runSound)
                {
                    audioSource.clip = runSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource.clip != walkSound)
                {
                    audioSource.clip = walkSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
