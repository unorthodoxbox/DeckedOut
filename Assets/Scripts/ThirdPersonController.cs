using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ThirdPersonController : MonoBehaviour
{
    private EntityStats playerStats;

    [Header("Movement Settings")]
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
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

    [Header("Weapon Settings")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject secondGun;
    [SerializeField] private GameObject crowbar;

    private int currentGunCount = 1;
    private float lastAttackTime = 0f;
    private bool mainGunEquipped = true;

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

        secondGun.SetActive(false);
        crowbar.SetActive(false);
    }

    void Update()
    {
        HandleMovement();
        HandleCameraRotation();
        HandleAttacking();
        WeaponEquip();
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

        if (jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(playerStats.jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(move * currentSpeed * Time.deltaTime + velocity * Time.deltaTime);
    }

    void HandleCameraRotation()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        yaw += lookInput.x * rotationSpeed * Time.deltaTime;
        pitch -= lookInput.y * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleAttacking()
    {
        float attackCooldown = 1f / playerStats.attackSpeed;

        if (Input.GetMouseButton(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            if (mainGunEquipped && playerStats.ammoInGun > 0)
            {
                if (currentGunCount == 1)
                    playerStats.ammoInGun--;

                lastAttackTime = Time.time;

                Vector3 gunPos = gun.transform.position;
                Quaternion gunRot = cameraTransform.rotation;

                GameObject currentBullet = Instantiate(bullet, gunPos, gunRot);
                Vector3 shootDir = cameraTransform.forward.normalized;

                currentBullet.GetComponent<Bullet>().InitializeVariables(
                    this.gameObject.tag,
                    playerStats.attackDamage,
                    playerStats.bulletSpeed,
                    shootDir
                );
            }
            else if (!mainGunEquipped)
            {
                StartCoroutine(waiter());
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && playerStats.totalAmmo > 0)
        {
            float needed = playerStats.clipSize - playerStats.ammoInGun;

            if (playerStats.totalAmmo >= needed)
            {
                playerStats.ammoInGun += needed;
                playerStats.totalAmmo -= needed;
            }
            else
            {
                playerStats.ammoInGun += playerStats.totalAmmo;
                playerStats.totalAmmo = 0;
            }
        }
    }

    IEnumerator waiter()
    {
        crowbar.GetComponent<BoxCollider>().enabled = true;
        yield return new WaitForSeconds(1);
        crowbar.GetComponent<BoxCollider>().enabled = false;
    }

    void WeaponEquip()
    {
        bool pressedC = Input.GetKeyDown(KeyCode.C);

        if (pressedC && currentGunCount == 0)
        {
            gun.SetActive(true);
            mainGunEquipped = true;
            currentGunCount = 1;
            playerStats.attackSpeed = 5f;
        }
        else if (pressedC && currentGunCount == 1)
        {
            gun.SetActive(false);
            secondGun.SetActive(true);
            mainGunEquipped = true;
            currentGunCount = 2;
            playerStats.attackSpeed = 3f;
        }
        else if (pressedC && currentGunCount == 2)
        {
            secondGun.SetActive(false);
            crowbar.SetActive(true);
            mainGunEquipped = false;
            currentGunCount = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "AmmoCrateMed")
        {
            playerStats.totalAmmo += Random.Range(10, 30);
            if (playerStats.totalAmmo > playerStats.maxClipSize)
                playerStats.totalAmmo = playerStats.maxClipSize;

            Destroy(other.gameObject);
        }
    }

}