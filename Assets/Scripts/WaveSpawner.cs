using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    public GameObject enemy;
    public Transform player;
    public GameObject ammoCrate;

    public float spawnRadius = 25f;
    public float ammoSpawnRadius = 15f;
    public float timeBetweenWaves = 3f;
    public int minEnemies = 5;
    public int moreEnemiesPerWave = 2;

    private int waveNumber = 0;
    private bool isSpawning = false;

    // Update is called once per frame
    void Update()
    {
        //check if all enemeis are dead and if spawner isn't spawning anymore
        if (!isSpawning && GameObject.FindGameObjectsWithTag("Enemy").Length <= 0)
        {
            StartCoroutine(SpawnWave());
        } //if
    } //Update

    //spawns wave based on timeBetweenWaves
    IEnumerator SpawnWave()
    {
        isSpawning = true;
        yield return new WaitForSeconds(timeBetweenWaves);

        waveNumber++;
        int numEnemies = minEnemies + (moreEnemiesPerWave * waveNumber);
        int spawned = 0;
        int enemyAttempts = 0;
        int ammoAttempts = 0;

        //tries to spawn enemy; will not spawn after several attempts to avoid infinite loop
        while (spawned < numEnemies && enemyAttempts < numEnemies * 5)
        {
            if (TryToSpawnOnNavMesh())
            {
                spawned++;
                enemyAttempts++;
                yield return null;
            } //if     
        } //while

        //tries to spawn ammo crate a bunch of times
        while (!SpawnAmmoCrate() && ammoAttempts < 20)
        {
            ammoAttempts++;
        } //while

        isSpawning = false;
    } //SpawnWave

    bool TryToSpawnOnNavMesh()
    {
        //makes a spawn location within a ring based on the spawn radius 
        Vector2 spawnOffset = Random.insideUnitCircle.normalized * Random.Range(spawnRadius * 0.5f, spawnRadius);
        Vector3 targetPosition = player.position + new Vector3(spawnOffset.x, 0, spawnOffset.y);

        //reject spawns too close to the player
        if (Vector3.Distance(targetPosition, player.position) < 10f)
        {
            return false;
        } //if

        //check NavMesh to see if the spawn location is valid
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 5f, NavMesh.AllAreas))
        {
            Instantiate(enemy, hit.position, Quaternion.identity);
            return true;
        } //if

        return false;
    } //TryToSpawnOnNavMesh

    bool SpawnAmmoCrate()
    {
        //makes a spawn location within a ring based on the spawn radius 
        Vector2 crateOffset = Random.insideUnitCircle.normalized * Random.Range(ammoSpawnRadius * 0.5f, ammoSpawnRadius); 
        Vector3 targetPosition = player.position + new Vector3(crateOffset.x, 0, crateOffset.y);

        //check NavMesh to see if the spawn location is valid
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 5f, NavMesh.AllAreas))
        {
            Instantiate(ammoCrate, hit.position, Quaternion.identity);
            return true;
        } //if
        return false;
    }

} // WaveSpawner
