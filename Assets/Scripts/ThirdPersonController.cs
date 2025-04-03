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
    //private InputAction clickAction;

    private float yaw;
    private float pitch;

    [Header("Weapon Settings")]
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private GameObject gun;
    [SerializeField]
    private GameObject secondGun;

    private int currentGunCount = 1; //Weapon wheel: 0 = empty hand, 1 = main gun, 2 = secondary gun
    private float lastAttackTime = 0f;  // Stores when the last attack happened
    private bool mainGunEquipped = true;

    void Awake()
    {
        playerStats = GetComponent<EntityStats>(); // Get PlayerStats on the same object
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"]; // Set the Jump action
        lookAction = playerInput.actions["Look"];

        //clickAction = playerInput.actions["Attack"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerStats.RefreshStats();

        secondGun.SetActive(false);
    }

    void Update()
    {
        HandleMovement();
        HandleCameraRotation();
        CameraFollow();
        HandleAttacking();
        WeaponEquip();
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
        {
            Debug.Log("Sprinting");
            currentSpeed = playerStats.sprintSpeed;
        }
        else if (isCrouching)
        {
            Debug.Log("Crouching");
            currentSpeed = playerStats.crouchSpeed;
        }
        else
        {
            currentSpeed = playerStats.walkSpeed;
        }
            

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

    void HandleAttacking()
    {
        float attackCooldown = 1f / playerStats.attackSpeed;    // Cooldown in seconds
        if (Input.GetMouseButton(0) && Time.time >= lastAttackTime + attackCooldown && playerStats.ammoInGun > 0 && mainGunEquipped)
        {
            playerStats.ammoInGun--; //Reduces player ammo

            lastAttackTime = Time.time;  // Update last attack time
            // Set spawn position and correct rotation
            Vector3 gunPos = gun.transform.position;
            Quaternion gunRot = cameraTransform.rotation; // Use the camera's rotation

            // Spawn bullet
            GameObject currentBullet = Instantiate(bullet, gunPos, gunRot);

            // Calculate shooting direction (forward from camera)
            Vector3 shootDirection = cameraTransform.forward.normalized;

            // Initialize bullet variables
            currentBullet.GetComponent<Bullet>().InitializeVariables(
                this.gameObject.tag,
                playerStats.attackDamage,
                playerStats.bulletSpeed,
                shootDirection
            );
        }

        //Handles reload from totalAmmo
        if (Input.GetKeyDown(KeyCode.E) && playerStats.totalAmmo > 0) {
            if (playerStats.totalAmmo >= 30) {

            }
            float temp = playerStats.clipSize - playerStats.ammoInGun;

            if (playerStats.totalAmmo >= temp) {
                playerStats.ammoInGun += temp;
                playerStats.totalAmmo -= temp;
            } else 
            {
                playerStats.ammoInGun += playerStats.totalAmmo;
                playerStats.totalAmmo = 0;
            }
        }
    }
    void WeaponEquip() {
        bool pressedC = Input.GetKeyDown(KeyCode.C);

        //Equips main gun
        if (pressedC && currentGunCount == 0) {
            gun.SetActive(true);
            pressedC = false;
            mainGunEquipped = true;
            currentGunCount = 1;
            Debug.Log("CurrentGunCount: 1");
        }

        //Equips secondary gun
        if (pressedC && currentGunCount == 1) {
            gun.SetActive(false);
            secondGun.SetActive(true);
            pressedC = false;
            mainGunEquipped = true;
            currentGunCount = 2;
            Debug.Log("CurrentGunCount: 2");
        }
        
        //Dequips guns
        if (pressedC && currentGunCount == 2) {
            secondGun.SetActive(false);
            //Debug.Log("inSetActive");
            pressedC = false;
            mainGunEquipped = false;
            currentGunCount = 0;
            Debug.Log("CurrentGunCount: 0");
        }
    }

    private void OnTriggerEnter(Collider other) {
        //Handles collisions with types of ammo crates
        if (other.gameObject.tag == "AmmoCrateSmall") {
            playerStats.totalAmmo += 10;
            if (playerStats.totalAmmo > playerStats.maxClipSize) {
                playerStats.totalAmmo = playerStats.maxClipSize;
            }
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "AmmoCrateMed") {
            playerStats.totalAmmo += 20;
            if (playerStats.totalAmmo > playerStats.maxClipSize) {
                playerStats.totalAmmo = playerStats.maxClipSize;
            }
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "AmmoCrateBig") {
            playerStats.totalAmmo += 30;
            if (playerStats.totalAmmo > playerStats.maxClipSize) {
                playerStats.totalAmmo = playerStats.maxClipSize;
            }
            Destroy(other.gameObject);
        }
    }

}