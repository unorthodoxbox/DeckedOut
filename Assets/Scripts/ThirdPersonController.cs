using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    private EntityStats playerStats;

    [Header("Movement Settings")]
    public float gravity = -9.81f; // Gravity force

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float rotationSpeed = 5f;
    public Vector3 cameraOffset = new Vector3(0.75f, 1.5f, -3f); // Over the right shoulder

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching = false;
    private float currentSpeed;

    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction; // Jump action
    private InputAction lookAction;

    private float yaw;
    private float pitch;

    void Awake()
    {
        playerStats = GetComponent<EntityStats>(); // Get PlayerStats on the same object
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"]; // Set the Jump action
        lookAction = playerInput.actions["Look"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerStats.RefreshStats();
    }

    void Update()
    {
        HandleMovement();
        HandleCameraRotation();
        CameraFollow();
    }

    void HandleMovement()
    {
        // Ground check
        isGrounded = controller.isGrounded;

        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        moveDirection = cameraTransform.forward * moveDirection.z + cameraTransform.right * moveDirection.x;
        moveDirection.y = 0f;

        if (sprintAction.IsPressed() && !isCrouching)
            currentSpeed = playerStats.sprintSpeed;
        else if (isCrouching)
            currentSpeed = playerStats.crouchSpeed;
        else
            currentSpeed = playerStats.walkSpeed;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset y velocity when grounded
        }

        // Jumping logic
        if (jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(playerStats.jumpHeight * -2f * gravity); // Jump formula
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(moveDirection * currentSpeed * Time.deltaTime + velocity * Time.deltaTime);
    }

    void HandleCameraRotation()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        yaw += lookInput.x * rotationSpeed * Time.deltaTime;
        pitch -= lookInput.y * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -45f, 45f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    void CameraFollow()
    {
        cameraTransform.position = transform.position + Quaternion.Euler(0, yaw, 0) * cameraOffset;
        cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
