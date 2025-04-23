using UnityEngine;

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

        for (int i = 0; i < numAmmoCratesPerWave; i++)
        {
            spawner.SpawnOnNavMesh(RandomAmmoCrate(), 1);
        }
        
    }

    public GameObject RandomAmmoCrate()
    {
        int random = Random.Range(0, 3);
        if (random == 0)
        {
            return ammoCrateSmall;
        } else if (random == 1)
        {
            return ammoCrateMed;
        } else if (random == 2)
        {
            return ammoCrateBig;
        }
        return ammoCrateSmall;
    }
}
