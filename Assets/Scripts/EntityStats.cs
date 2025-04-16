using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EntityStats : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float jumpHeight = 2f;

    [Header("MoveSpeed Multipliers")]
    [SerializeField]
    private float walkSpeedMult = 1f;
    [SerializeField]
    private float sprintSpeedMult = 2f;
    [SerializeField]
    private float crouchSpeedMult = 0.5f;

    [HideInInspector]
    public float walkSpeed = 3f;
    [HideInInspector]
    public float sprintSpeed = 6f;
    [HideInInspector]
    public float crouchSpeed = 1.5f;

    [Header("Attack Settings")]
    public float attackDamage = 10f;
    public float attackSpeed = 5f; // Number of attacks per second
    public float bulletSpeed = 20f;
    public float clipSize = 30f; //Max amount of ammo the gun can hold
    public float maxClipSize = 90f; //Max amount of additional ammo you can hold
    public float totalAmmo = 90f; //Extra ammo the player is currently holding
    public float ammoInGun = 30f; //Ammo currently left in the gun

    [Header("Body Settings")]
    public float maxHealth = 50f;
    public float currHealth = 100f;

    [Header("UI Settings")]
    [SerializeField]
    public HealthBar healthBar;

    [Header("Player Settings")]
    public bool isPlayer = false;
    public Material lowHPMaterial;

    private Coroutine flashRoutine;
    private bool isDead = false;

    public void Awake()
    {
        RefreshStats();
        currHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth((int)maxHealth);
        }
        if (isPlayer)
        {
            lowHPMaterial.SetFloat("_Alpha", 0);
        }
    }
    public void RefreshStats()
    {
        walkSpeed = walkSpeedMult * moveSpeed;
        sprintSpeed = sprintSpeedMult * moveSpeed;
        crouchSpeed = crouchSpeedMult * moveSpeed;
    }

    public void takeDamage(float damage)
    {

        currHealth -= damage;
        Debug.Log(gameObject.name + " health is now " + currHealth);
        UpdateHealthBar();
        if (isPlayer && !isDead)
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }
            flashRoutine = StartCoroutine(DamageFlash());
            if (currHealth <= 0)
            {
                isDead = true;
                GetComponent<ThirdPersonController>().TriggerDeath();
                // Optionally play a death SFX, screen fade, etc.
            }
        }
    }

    private IEnumerator DamageFlash()
    {
        float flashInDuration = 0.1f;
        float flashOutDuration = 0.2f;
        float elapsed = 0f;

        // Fade in
        while (elapsed < flashInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / flashInDuration);
            lowHPMaterial.SetFloat("_Alpha", alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        lowHPMaterial.SetFloat("_Alpha", 1f);

        // Fade out
        elapsed = 0f;
        while (elapsed < flashOutDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / flashOutDuration);
            lowHPMaterial.SetFloat("_Alpha", alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        lowHPMaterial.SetFloat("_Alpha", 0f);

        flashRoutine = null;
    }



    private void UpdateHealthBar()
    {
        // Update health bar fill based on current health
        if (healthBar != null)
        {
            healthBar.SetHealth((int)currHealth);
        }
    }

}
