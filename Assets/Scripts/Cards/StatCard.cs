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
            case StatType.moveSpeed:
                ApplyStat(ref entity.moveSpeed);
                break;
            case StatType.jumpHeight:
                ApplyStat(ref entity.jumpHeight);
                break;
            case StatType.numJumps:
                ApplyStat(ref entity.numJumps);
                break;
            case StatType.attackSpeed:
                ApplyStat(ref entity.attackSpeed);
                break;
            case StatType.attackDamage:
                ApplyStat(ref entity.attackDamage);
                break;
            case StatType.clipSize:
                ApplyStat(ref entity.clipSize);
                break;
            case StatType.maxClipSize:
                ApplyStat(ref entity.maxClipSize);
                break;
            case StatType.maxHealth:
                ApplyStat(ref entity.maxHealth);
                break;
            default:
                Debug.LogWarning("Unhandled StatType: " + statType);
                break;


        }
        entity.RefreshStats();
        entity.statCards.Add(this);
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
