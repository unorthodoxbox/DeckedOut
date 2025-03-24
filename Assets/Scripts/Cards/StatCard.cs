using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Card", menuName = "Cards/StatCard")]
public class StatCard : Card
{
    public StatType statType;  // The stat this card affects
    public float value;        // The amount to add/multiply
    public bool isMultiplicative; // True = multiply, False = add
    public bool stacksMultiplicatively; // Determines stacking behavior

    public override void ApplyEffect(EntityStats entity)
    {
        if (entity == null) return;

        switch (statType)
        {
            case StatType.maxHealth:
                ApplyStat(ref entity.maxHealth);
                break;
            case StatType.moveSpeed:
                ApplyStat(ref entity.moveSpeed);
                break;
            case StatType.attackDamage:
                ApplyStat(ref entity.attackDamage);
                break;
            /*
            case StatType.Defense:
                ApplyStat(ref entity.defense);
                break;
            */
        }
        entity.RefreshStats();
    }

    private void ApplyStat(ref float stat)
    {
        if (isMultiplicative)
        {
            if (stacksMultiplicatively)
                stat *= value; // Multiplicative stacking
            else
                stat *= (1 + value); // Regular multiplier
        }
        else
        {
            stat += value; // Additive increase
        }
    }
}
