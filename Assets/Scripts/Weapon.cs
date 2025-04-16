using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Type")]
    public bool isRanged = true;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public Transform firePoint;
    public float cooldown = 0.2f;
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
    private Vector3 targetLocalPos;
    private Quaternion targetLocalRot;


    void Start()
    {
        playerStats = GetComponentInParent<EntityStats>();
        controller = GetComponentInParent<ThirdPersonController>();
        cam = Camera.main;

        if (weaponVisual != null)
        {
            initialLocalPos = weaponVisual.localPosition;
            initialLocalRot = weaponVisual.localRotation;
            targetLocalPos = initialLocalPos;
            targetLocalRot = initialLocalRot;
        }
    }


    void Update()
    {
        HandleFiring();
        ApplyRecoil();
        if (weaponVisual != null)
        {
            weaponVisual.localPosition = Vector3.Lerp(weaponVisual.localPosition, targetLocalPos, visualRecoilRecovery * Time.deltaTime);
            weaponVisual.localRotation = Quaternion.Slerp(weaponVisual.localRotation, targetLocalRot, visualRecoilRecovery * Time.deltaTime);

            // Once close enough, reset the target to idle
            if (Vector3.Distance(weaponVisual.localPosition, targetLocalPos) < 0.01f)
            {
                targetLocalPos = initialLocalPos;
                targetLocalRot = initialLocalRot;
            }
        }

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
                playerStats.ammoInGun--;
                lastShotTime = Time.time;
            }
        }
        else if (!isRanged && Input.GetMouseButtonDown(0) && Time.time >= lastShotTime + cooldown)
        {
            StartCoroutine(Swing());
            lastShotTime = Time.time;
        }

        // Reload
        if (Input.GetKeyDown(KeyCode.E) && playerStats.totalAmmo > 0)
        {
            float needed = playerStats.clipSize - playerStats.ammoInGun;
            float reloadAmount = Mathf.Min(needed, playerStats.totalAmmo);
            playerStats.ammoInGun += reloadAmount;
            playerStats.totalAmmo -= reloadAmount;
        }
    }

    void Shoot()
    {
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

        bullet.GetComponent<Bullet>().InitializeVariables(playerStats.attackDamage);

        // Add recoil from animation curves
        targetRecoil.x += verticalRecoilCurve.Evaluate(shotsFired);
        targetRecoil.y += horizontalRecoilCurve.Evaluate(shotsFired);
        shotsFired++;
        timeSinceLastShot = 0f;

        if (weaponVisual != null)
        {
            // Recoil kickback (local Z push)
            targetLocalPos = initialLocalPos + recoilKickback;

            // Recoil rotation (local kick)
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
    }

    System.Collections.IEnumerator Swing()
    {
        GetComponent<BoxCollider>().enabled = true;
        yield return new WaitForSeconds(1f);
        GetComponent<BoxCollider>().enabled = false;
    }
}
