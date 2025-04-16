using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isRanged = true;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public Transform firePoint;
    public float cooldown = 0.2f;
    private float lastShotTime = 0f;

    private EntityStats playerStats;
    private Camera cam;

    void Start()
    {
        playerStats = GetComponentInParent<EntityStats>();
        cam = Camera.main;
    }

    void Update()
    {
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
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of screen
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000f); // Just shoot straight forward
        }

        Vector3 direction = (targetPoint - firePoint.position).normalized;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }

        // Optional: call a method on the bullet to pass damage
        bullet.GetComponent<Bullet>().InitializeVariables(playerStats.attackDamage);
    }


    System.Collections.IEnumerator Swing()
    {
        GetComponent<BoxCollider>().enabled = true;
        yield return new WaitForSeconds(1f);
        GetComponent<BoxCollider>().enabled = false;
    }
}
