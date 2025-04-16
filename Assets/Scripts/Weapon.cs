using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isRanged = true;
    public GameObject bulletPrefab;
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
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Vector3 shootDir = cam.transform.forward.normalized;

        bullet.GetComponent<Bullet>().InitializeVariables(
            gameObject.tag,
            playerStats.attackDamage,
            playerStats.bulletSpeed,
            shootDir
        );
    }

    System.Collections.IEnumerator Swing()
    {
        GetComponent<BoxCollider>().enabled = true;
        yield return new WaitForSeconds(1f);
        GetComponent<BoxCollider>().enabled = false;
    }
}
