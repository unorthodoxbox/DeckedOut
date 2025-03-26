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

    [Header("Body Settings")]
    public float maxHealth = 50f;
    public float currHealth = 100f;

    [Header("UI Settings")]
    [SerializeField]
    public HealthBar healthBar;

    public void Awake()
    {
        RefreshStats();
        currHealth = maxHealth;
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
        if (currHealth <= 0)
        {
            die();
        }
        UpdateHealthBar();

    }

    private void UpdateHealthBar()
    {
        // Update health bar fill based on current health
        if (healthBar != null)
        {
            healthBar.SetHealth((int)currHealth);
        }
    }

    private void die()
    {
        Debug.Log(gameObject.name + " has died");
        Destroy(this.gameObject);
    }

}
