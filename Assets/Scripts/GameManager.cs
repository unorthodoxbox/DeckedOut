using UnityEngine;
using System.Collections; // Needed for IEnumerator

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public EntityStats playerStats;
    [SerializeField] private NavMeshSpawner spawner;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject chest;
    [SerializeField] private GameObject ammoCrateSmall;
    [SerializeField] private GameObject ammoCrateMed;
    [SerializeField] private GameObject ammoCrateBig;

    public int timeBetweenWaves = 5;

    // Wave mechanics
    public int numEnemies = 0;
    public int waveSize = 5;
    public int waveGrowthSize = 5;
    public int currentWave = 1;

    // Chest mechanics
    public int numChestsPerWave = 5;

    // Ammo Crate mechanics
    public int numAmmoCratesPerWave = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
        StartCoroutine(NewWaveCoroutine()); // Start coroutine instead
    }

    public void PlayAgain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator NewWaveCoroutine()
    {
        waveSize += waveGrowthSize;
        numEnemies = waveSize;

        for (int i = 0; i < numAmmoCratesPerWave; i++)
        {
            spawner.SpawnOnNavMesh(RandomAmmoCrate(), 1);
        }

        spawner.SpawnOnNavMesh(chest, numChestsPerWave);
        Debug.Log("Sleeping before spawning");
        // Sleep here before spawning enemies
        yield return new WaitForSeconds(timeBetweenWaves);
        Debug.Log("Spawning enemies");
        spawner.SpawnOnNavMesh(enemy, waveSize);
    }

    public void newWave()
    {
        currentWave++;
        StartCoroutine(NewWaveCoroutine()); // Call coroutine from here too
    }

    public GameObject RandomAmmoCrate()
    {
        int random = Random.Range(0, 3);
        if (random == 0)
        {
            return ammoCrateSmall;
        }
        else if (random == 1)
        {
            return ammoCrateMed;
        }
        else
        {
            return ammoCrateBig;
        }
    }

    public void UpdateNumEnemies()
    {
        numEnemies--;
        if (numEnemies <= 0)
        {
            newWave();
        }
    }
}
