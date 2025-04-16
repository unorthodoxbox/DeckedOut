using UnityEngine;

public class AnimatorEventBridge : MonoBehaviour
{
    public EnemyAI enemyAI;

    public void AttackHit() => enemyAI?.AttackHit();
    public void EndAttack() => enemyAI?.EndAttack();
}
