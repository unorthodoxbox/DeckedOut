using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Vector3 recoilOffset;

    [Header("Idle Breathing Sway")]
    public float idleSwaySpeed = 1f;
    public float idleSwayAmount = 0.01f;

    [Header("ADS Settings")]
    public bool isAiming = false; // Will be set externally (optional)
    public float adsSwayMultiplier = 0.3f;

    private Vector3 idleSwayOffset;
    private float idleTime;

    [Header("Sway Settings")]
    public float swayAmount = 0.02f;
    public float swayMaxAmount = 0.05f;
    public float swaySmooth = 6f;
    private Vector3 swayOffset;
    private Vector3 currentSwayPos;

    [Header("Weapon Type")]
    public bool isRanged = true;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public Transform firePoint;
    public float cooldown = 0.2f;
    public float damage = 10f;
    private float lastShotTime = 0f;

    [Header("Recoil Settings")]
    public AnimationCurve verticalRecoilCurve;
    public AnimationCurve horizontalRecoilCurve;
    public float recoilRecoverySpeed = 8f;

    private float timeSinceLastShot = 0f;
    private int shotsFired = 0;
    private Vector2 currentRecoil;
    private Vector2 targetRecoil;

    private EntityStats playerStats;
    private ThirdPersonController controller;
    private Camera cam;

    [Header("Visual Recoil")]
    public Transform weaponVisual; // Drag the gun model here
    public Vector3 recoilKickback = new Vector3(0f, 0f, -0.1f); // Local Z kickback
    public Vector3 recoilRotation = new Vector3(-5f, 2f, 2f);   // Rotation kick
    public float visualRecoilRecovery = 10f;

    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;
    private Quaternion targetLocalRot;

    [Header("Sound Settings")]
    public SoundPlayer weaponSoundPlayer; // Serialized because it might change
    public string soundPrefix;


    void Start()
    {
        playerStats = GetComponentInParent<EntityStats>();
        controller = GetComponentInParent<ThirdPersonController>();
        cam = Camera.main;

        if (weaponVisual != null)
        {
            initialLocalPos = weaponVisual.localPosition;
            initialLocalRot = weaponVisual.localRotation;
            targetLocalRot = initialLocalRot;
        }
    }


    void Update()
    {
        // In Weapon.cs Update() or FixedUpdate()
        isAiming = controller.playerInput.actions["Aim"].IsPressed();
        HandleFiring();
        ApplyRecoil();
        CalculateWeaponSway();
        if (weaponVisual != null)
        {
            // --- Combine All Position Effects ---
            Vector3 targetPosition =
                initialLocalPos +      // Rest pose
                recoilOffset +         // Kickback
                currentSwayPos +       // Look-based sway
                idleSwayOffset;        // Breathing sway

            weaponVisual.localPosition = Vector3.Lerp(
                weaponVisual.localPosition,
                targetPosition,
                visualRecoilRecovery * Time.deltaTime
            );

            // --- Rotation Recoil Recovery ---
            weaponVisual.localRotation = Quaternion.Slerp(
                weaponVisual.localRotation,
                targetLocalRot,
                visualRecoilRecovery * Time.deltaTime
            );

            // If we're close to resting, reset rotation target
            if (Quaternion.Angle(weaponVisual.localRotation, targetLocalRot) < 0.1f)
                targetLocalRot = initialLocalRot;
        }


    }

    private void CalculateWeaponSway()
    {
        if (controller == null || controller.playerInput == null) return;

        // --- LOOK SWAY ---
        Vector2 lookInput = controller.playerInput.actions["Look"].ReadValue<Vector2>();

        float swayMult = isAiming ? adsSwayMultiplier : 1f;

        swayOffset.x = Mathf.Clamp(-lookInput.x * swayAmount * swayMult, -swayMaxAmount, swayMaxAmount);
        swayOffset.y = Mathf.Clamp(-lookInput.y * swayAmount * swayMult, -swayMaxAmount, swayMaxAmount);
        currentSwayPos = Vector3.Lerp(currentSwayPos, swayOffset, Time.deltaTime * swaySmooth);

        // --- IDLE SWAY (breathing) ---
        idleTime += Time.deltaTime;
        float idleX = Mathf.Sin(idleTime * idleSwaySpeed) * idleSwayAmount;
        float idleY = Mathf.Cos(idleTime * idleSwaySpeed * 0.7f) * idleSwayAmount;
        idleSwayOffset = new Vector3(idleX, idleY, 0f);
    }



    void HandleFiring()
    {
        timeSinceLastShot += Time.deltaTime;

        if (timeSinceLastShot > 0.5f)
            shotsFired = 0;

        if (isRanged && Input.GetMouseButton(0) && Time.time >= lastShotTime + cooldown)
        {
            if (playerStats.ammoInGun > 0)
            {
                Shoot();
                if (controller.currentWeaponIndex != 1) {
                    playerStats.ammoInGun--;
                }
                lastShotTime = Time.time;
            }
        }
        else if (!isRanged && Input.GetMouseButtonDown(0) && Time.time >= lastShotTime + cooldown)
        {
            StartCoroutine(Swing());
            lastShotTime = Time.time;
        }

        // Reload
        if (Input.GetKeyDown(KeyCode.R) && playerStats.totalAmmo > 0)
        {
            weaponSoundPlayer.Play(soundPrefix + " Reload"); 
            
            float needed = playerStats.clipSize - playerStats.ammoInGun;
            float reloadAmount = Mathf.Min(needed, playerStats.totalAmmo);
            playerStats.ammoInGun += reloadAmount;
            playerStats.totalAmmo -= reloadAmount;
        }
    }

    void Shoot()
    {
        weaponSoundPlayer.Play(soundPrefix + " Shot");

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Vector3 targetPoint;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(1000f);

        Vector3 direction = (targetPoint - firePoint.position).normalized;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = direction * bulletSpeed;

        bullet.GetComponent<Bullet>().InitializeVariables(damage);

        // Add recoil from animation curves
        targetRecoil.x += verticalRecoilCurve.Evaluate(shotsFired);
        targetRecoil.y += horizontalRecoilCurve.Evaluate(shotsFired);
        shotsFired++;
        timeSinceLastShot = 0f;

        if (weaponVisual != null)
        {
            recoilOffset = recoilKickback;

            Quaternion rotOffset = Quaternion.Euler(recoilRotation);
            targetLocalRot = initialLocalRot * rotOffset;
        }


    }

    void ApplyRecoil()
    {
        targetRecoil = Vector2.Lerp(targetRecoil, Vector2.zero, recoilRecoverySpeed * Time.deltaTime);
        currentRecoil = Vector2.Lerp(currentRecoil, targetRecoil, recoilRecoverySpeed * Time.deltaTime);

        if (controller != null)
        {
            controller.recoilX = currentRecoil.x;
            controller.recoilY = currentRecoil.y;
        }
        // Smoothly decay recoil kickback
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, visualRecoilRecovery * Time.deltaTime);

    }

    System.Collections.IEnumerator Swing()
    {
        weaponSoundPlayer.Play(soundPrefix + "Swing");
        GetComponent<BoxCollider>().enabled = true;
        yield return new WaitForSeconds(1f);
        GetComponent<BoxCollider>().enabled = false;
    }
}
