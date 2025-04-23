using UnityEngine;
using UnityEngine.AI;

public class NavMeshSpawner : MonoBehaviour
{
    [Tooltip("How far from the spawner's position to search for valid spawn points")]
    public float spawnRadius = 100f;

    [Tooltip("Max attempts per object to find a valid NavMesh position")]
    public int maxAttemptsPerObject = 50;

    /// Spawns a specified number of a prefab on the NavMesh at random positions within the radius.
    public void SpawnOnNavMesh(GameObject prefab, int quantity)
    {
        int spawned = 0;
        int attempts = 0;

        while (spawned < quantity && attempts < quantity * maxAttemptsPerObject)
        {
            if (TryGetRandomNavMeshPosition(spawnRadius, out Vector3 randomPos))
            {

                // Offset the Y position by half the height
                Vector3 spawnPos = randomPos + Vector3.up * (1 / 3f);
                Instantiate(prefab, spawnPos, Quaternion.identity);
                spawned++;
            }
            attempts++;
        }

        Debug.Log($"Spawned {spawned}/{quantity} of {prefab.name} in {attempts} attempts.");
    }

    /// Finds a valid random position on the NavMesh within a radius.
    private bool TryGetRandomNavMeshPosition(float radius, out Vector3 result)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection.y = 0f; // Stay horizontal
        Vector3 samplePos = transform.position + randomDirection;

        if (NavMesh.SamplePosition(samplePos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    // Draw the spawn radius in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
