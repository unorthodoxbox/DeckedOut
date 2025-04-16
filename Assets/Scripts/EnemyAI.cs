using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum BehaviorMode { Patrol, Wander, Both }
    [Header("Behavior Mode")]
    public BehaviorMode mode = BehaviorMode.Patrol;

    [Header("Chase Settings")]
    public float detectionRange = 20f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float damage = 10f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;

    [Header("Wander Settings")]
    public float wanderRadius = 10f;
    public float wanderInterval = 5f;

    private int currentPatrolIndex = 0;
    private float lastAttackTime;
    private float lastWanderTime;
    private Transform player;
    private NavMeshAgent agent;
    private EntityStats playerStats;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerStats = player?.GetComponent<EntityStats>();
        agent = GetComponent<NavMeshAgent>();
        lastAttackTime = -attackCooldown;
        lastWanderTime = Time.time;

        if (mode == BehaviorMode.Patrol || (mode == BehaviorMode.Both && patrolPoints.Length > 0))
        {
            agent.SetDestination(patrolPoints[0].position);
        }
        else if (mode == BehaviorMode.Wander || mode == BehaviorMode.Both)
        {
            Wander();
        }
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            agent.SetDestination(player.position);

            if (distance <= attackRange)
            {
                FaceTarget();
                TryAttack();
            }
        }
        else
        {
            if (mode == BehaviorMode.Patrol || (mode == BehaviorMode.Both && patrolPoints.Length > 0))
                Patrol();
            else if (mode == BehaviorMode.Wander || mode == BehaviorMode.Both)
                WanderCheck();
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f && patrolPoints.Length > 0)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void WanderCheck()
    {
        if (Time.time >= lastWanderTime + wanderInterval)
        {
            Wander();
            lastWanderTime = Time.time;
        }
    }

    void Wander()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log("Bug attacks player!");

            if (playerStats)
            {
                playerStats.currHealth -= damage;
                playerStats.currHealth = Mathf.Max(playerStats.currHealth, 0);
            }

            lastAttackTime = Time.time;
        }
    }

    void FaceTarget()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (Application.isPlaying && player)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= detectionRange)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position + Vector3.up * 1f, player.position + Vector3.up * 1f);
            }
        }

        if ((mode == BehaviorMode.Patrol || mode == BehaviorMode.Both) && patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawWireSphere(patrolPoints[i].position, 0.4f);
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                }
            }
        }

        if (mode == BehaviorMode.Wander || mode == BehaviorMode.Both)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, wanderRadius);
        }
    }
}
