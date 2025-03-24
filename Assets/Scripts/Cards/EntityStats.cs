using UnityEngine;

public class EntityStats : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float jumpHeight = 2f;

    [Header("MoveSpeed Multipliers")]
    public float walkSpeedMult = 1f;
    public float sprintSpeedMult = 2f;
    public float crouchSpeedMult = 0.5f;

    [HideInInspector]
    public float walkSpeed = 3f;
    [HideInInspector]
    public float sprintSpeed = 6f;
    [HideInInspector]
    public float crouchSpeed = 1.5f;

    [Header("Attack Settings")]
    public float attackSpeed = 1f;
    public float attackDamage = 5f;

    [Header("Body Settings")]
    public float maxHealth = 100f;
    public float currHealth = 100f;

    public void RefreshStats()
    {
        walkSpeed = walkSpeedMult * moveSpeed;
        sprintSpeed = sprintSpeedMult * moveSpeed;
        crouchSpeed = crouchSpeedMult * moveSpeed;
    }


}
