using UnityEngine;

[CreateAssetMenu(fileName = "New Speed Boost Card", menuName = "Cards/SpeedBoostCard")]
public class SpeedBoostCard : Card
{
    public float speedIncrease;

    public override void ApplyEffect(EntityStats player)
    {
        player.moveSpeed += speedIncrease;
    }
}
