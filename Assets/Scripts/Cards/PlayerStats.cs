using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float attackDamage = 10f;
    public float health = 100f;

    public void ApplyCard(Card card)
    {
        card.ApplyEffect(this);
    }
}
