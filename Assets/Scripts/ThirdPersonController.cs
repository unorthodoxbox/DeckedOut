using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Threading;



public class ThirdPersonController : MonoBehaviour
{
    [Header("Post-Processing")]
    public Volume postProcessingVolume;
    public GameObject UI;
    public GameObject deathUI;

    private ColorAdjustments colorAdjustments;
    private DepthOfField depthOfField;


    private EntityStats playerStats;
    public Transform cameraTransform;
    public float gravity = -9.81f;
    public float rotationSpeed = 5f;

    private CharacterController controller;
    public PlayerInput playerInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching = false;
    private float currentSpeed;
    private float currJumps;

    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction lookAction;

    [Header("Zoom / Aim Settings")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float zoomedFOV = 40f;
    public float zoomSpeed = 10f;

    private bool isAiming = false;
    private InputAction aimAction;


    private float yaw;
    private float pitch;

    [Header("Weapons")]
    public GameObject[] weapons;
    public int currentWeaponIndex = 0;
    public GameObject weaponContainer;

    [HideInInspector] public float recoilX;
    [HideInInspector] public float recoilY;

    private bool isDead = false;
    private Quaternion deathRotation;
    private float deathFallSpeed = 2f;
    private float deathTiltAmount = 70f;

    [Header("Sounds")]
    private SoundPlayer locomotionPlayer;
    public double stepDelay = 5f;
    private double stepTimer = .1;

    void Awake()
    {
        playerStats = GetComponent<EntityStats>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        aimAction = playerInput.actions["Aim"];

        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];
        lookAction = playerInput.actions["Look"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerStats.RefreshStats();
        EquipWeapon(currentWeaponIndex);

    }
    void Start()
    {
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments.saturation.value = 0f; // Normal color
        }
        if (postProcessingVolume.profile.TryGet(out depthOfField))
        {
            depthOfField.active = false;
        }

        locomotionPlayer = AudioManager.GetSoundPlayer("Locomotion");

    }
    IEnumerator FadeToGrayscale(float duration = 1f)
    {
        if (colorAdjustments == null) yield break;

        float startSat = colorAdjustments.saturation.value;
        float targetSat = -100f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            colorAdjustments.saturation.value = Mathf.Lerp(startSat, targetSat, t);
            yield return null;
        }

        colorAdjustments.saturation.value = targetSat;
    }
    IEnumerator FadeToBlur(float duration = 1f)
    {
        if (depthOfField == null) yield break;

        depthOfField.active = true;

        float startFocus = 10f;
        float endFocus = 0.1f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            depthOfField.focusDistance.value = Mathf.Lerp(startFocus, endFocus, t);
            yield return null;
        }

        depthOfField.focusDistance.value = endFocus;
    }


    public void TriggerDeath()
    {
        deathUI.SetActive(true); // Show death UI
        UI.SetActive(false); // Hide UI
        StartCoroutine(FadeToGrayscale(0.5f)); // You can tweak the duration
        StartCoroutine(FadeToBlur(0.5f));
        isDead = true;
        controller.enabled = false; // Disable movement
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; // Show cursor
        weaponContainer.SetActive(false); // Hide weapon container

        // Calculate a random dramatic tilt direction
        Vector3 tiltAxis = Random.value > 0.5f ? Vector3.right : Vector3.left;
        deathRotation = Quaternion.Euler(deathTiltAmount, 0f, 0f) * cameraTransform.localRotation;
    }


    void Update()
    {
        if (isDead)
        {
            cameraTransform.localRotation = Quaternion.Slerp(
                cameraTransform.localRotation,
                deathRotation,
                deathFallSpeed * Time.deltaTime
            );
            return;
        }
        
        HandleMovement();
        HandleCameraRotation();
        HandleWeaponSwitch();
        HandleZoom();
    }

    void HandleZoom()
    {
        isAiming = aimAction.IsPressed();

        float targetFOV = isAiming ? zoomedFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
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
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (isGrounded) currJumps = playerStats.numJumps; // Reset num of jumps if grounded
        if (jumpAction.triggered && currJumps > 0)
        {
            locomotionPlayer.Play("Player Jump");
            velocity.y = Mathf.Sqrt(playerStats.jumpHeight * -2f * gravity);
            currJumps--;

            stepTimer = 0;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(move * currentSpeed * Time.deltaTime + velocity * Time.deltaTime);

        if(isGrounded) {
            stepTimer -= currentSpeed * Time.deltaTime;
            if(stepTimer <= 0 ) {
                stepTimer = stepDelay;
                locomotionPlayer.Play();
            }
        } else {
            stepTimer = .1;
        }

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
    private void OnTriggerEnter(Collider other) {
        //Handles collisions with types of ammo crates
        
        if (other.gameObject.tag == "AmmoCrateMed") {
            playerStats.totalAmmo += 20;
            if (playerStats.totalAmmo > playerStats.maxClipSize) {
                playerStats.totalAmmo = playerStats.maxClipSize;
            }
            Destroy(other.gameObject);
        }
    }

}