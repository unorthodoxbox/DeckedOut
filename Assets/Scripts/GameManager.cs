using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public EntityStats playerStats;
    [SerializeField] private NavMeshSpawner spawner;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject chest;
    [SerializeField] private GameObject ammoCrate;

    // Wave mechanics
    public int numEnemies = 0;
    public int waveSize = 5;
    public int waveGrowthSize = 5;

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
        newWave();
    }
    public void PlayAgain()
    {
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void newWave()
    {
        waveSize += waveGrowthSize;
        numEnemies = waveSize;
        spawner.SpawnOnNavMesh(enemy, waveSize);

        spawner.SpawnOnNavMesh(chest, numChestsPerWave);

        spawner.SpawnOnNavMesh(ammoCrate, numAmmoCratesPerWave);
    }
}
