using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;
    public GameObject[] eliteEnemyPrefabs;

    [Header("References")]
    public Transform player;
    public PlayerStats playerStats;

    [Header("Spawn Settings")]
    public float minDistanceFromPlayer = 10f;
    public float spawnRangeX = 20f;
    public float spawnRangeZ = 20f;

    [Header("Spawn Timing")]
    public float spawnInterval = 5f;
    public float startDelay = 2f;
    public int enemiesPerSpawn = 2;

    [Header("Elite Spawn Chance")]
    [Range(0f, 100f)]
    public float baseEliteSpawnChance = 5f; // Base 5%

    private float spawnTimer = 0f;

    void Start()
    {
        spawnTimer = -startDelay;
    }

    void Update()
    {
        // 🔍 Always check for valid player + stats
        if (player == null || playerStats == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerStats = playerObj.GetComponent<PlayerStats>();
            }
        }

        if (player == null) return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
                SpawnEnemyAtRandomPosition();

            spawnTimer = 0f;
        }
    }

    void SpawnEnemyAtRandomPosition()
    {
        Vector3 spawnPos;
        int attempts = 0;

        do
        {
            float x = Random.Range(-spawnRangeX, spawnRangeX);
            float z = Random.Range(-spawnRangeZ, spawnRangeZ);
            spawnPos = new Vector3(x, 0.5f, z);
            attempts++;
        } while (Vector3.Distance(spawnPos, player.position) < minDistanceFromPlayer && attempts < 100);

        if (enemyPrefabs.Length == 0 && eliteEnemyPrefabs.Length == 0)
            return;

        // 🎯 Calculate effective elite chance
        float effectiveEliteChance = baseEliteSpawnChance;

        if (playerStats != null)
        {
            // Player bonus adds as % of base (0.1 = +10%, 1 = +100%, etc.)
            float bonusMultiplier = 1f + playerStats.GetEliteSpawnChance();
            effectiveEliteChance = baseEliteSpawnChance * bonusMultiplier;
        }

        // Clamp to avoid exceeding 100%
        effectiveEliteChance = Mathf.Clamp(effectiveEliteChance, 0f, 100f);

        // Roll for elite
        bool isElite = Random.Range(0f, 100f) < effectiveEliteChance;

        if (isElite && eliteEnemyPrefabs.Length > 0)
        {
            GameObject eliteEnemyToSpawn = eliteEnemyPrefabs[Random.Range(0, eliteEnemyPrefabs.Length)];
            Instantiate(eliteEnemyToSpawn, spawnPos, Quaternion.identity);
        }
        else if (enemyPrefabs.Length > 0)
        {
            GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
        }
    }
}
