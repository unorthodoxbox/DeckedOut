using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ThirdPersonController : MonoBehaviour
{
    private EntityStats playerStats;
    public Transform cameraTransform;
    public float gravity = -9.81f;
    public float rotationSpeed = 5f;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching = false;
    private float currentSpeed;

    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction lookAction;

    private float yaw;
    private float pitch;

    [Header("Weapons")]
    public GameObject[] weapons;
    private int currentWeaponIndex = 0;

    [HideInInspector] public float recoilX;
    [HideInInspector] public float recoilY;

    void Awake()
    {
        playerStats = GetComponent<EntityStats>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];
        lookAction = playerInput.actions["Look"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerStats.RefreshStats();
        EquipWeapon(currentWeaponIndex);
    }

    void Update()
    {
        HandleMovement();
        HandleCameraRotation();
        HandleWeaponSwitch();
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = cameraTransform.forward * input.y + cameraTransform.right * input.x;
        move.y = 0f;

        currentSpeed = sprintAction.IsPressed() && !isCrouching
            ? playerStats.sprintSpeed
            : isCrouching ? playerStats.crouchSpeed : playerStats.walkSpeed;

        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        if (jumpAction.triggered && isGrounded)
            velocity.y = Mathf.Sqrt(playerStats.jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(move * currentSpeed * Time.deltaTime + velocity * Time.deltaTime);
    }

    void HandleCameraRotation()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        yaw += lookInput.x * rotationSpeed * Time.deltaTime;
        pitch -= lookInput.y * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        float finalPitch = pitch + recoilX;
        float finalYaw = yaw + recoilY;

        transform.rotation = Quaternion.Euler(0f, finalYaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(finalPitch, 0f, 0f);
    }


    void HandleWeaponSwitch()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
            EquipWeapon(currentWeaponIndex);
        }
        else if (scroll < 0f)
        {
            currentWeaponIndex = (currentWeaponIndex - 1 + weapons.Length) % weapons.Length;
            EquipWeapon(currentWeaponIndex);
        }
    }

    void EquipWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(i == index);
    }

}