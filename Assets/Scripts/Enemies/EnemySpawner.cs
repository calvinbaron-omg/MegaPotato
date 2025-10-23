using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;  // Assign all enemy prefabs here
    public Transform player;

    [Header("Spawn Settings")]
    public float minDistanceFromPlayer = 10f;  // Minimum distance from player to spawn
    public float spawnRangeX = 20f;            // X-axis spawn area
    public float spawnRangeZ = 20f;            // Z-axis spawn area

    [Header("Spawn Timing")]
    public float spawnInterval = 5f;    // Seconds between spawn waves
    public float startDelay = 2f;       // Initial delay before first spawn
    public int enemiesPerSpawn = 2;     // How many enemies to spawn each interval

    private float spawnTimer = 0f;

    void Start()
    {
        // Set initial timer to wait before first spawn
        spawnTimer = -startDelay;
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        // Spawn enemies when timer reaches interval
        if (spawnTimer >= spawnInterval)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                SpawnEnemyAtRandomPosition();
            }
            spawnTimer = 0f; // Reset timer for next spawn wave
        }
    }

    void SpawnEnemyAtRandomPosition()
    {
        Vector3 spawnPos;
        int attempts = 0;

        // Find a spawn position far enough from player
        do
        {
            float x = Random.Range(-spawnRangeX, spawnRangeX);
            float z = Random.Range(-spawnRangeZ, spawnRangeZ);
            spawnPos = new Vector3(x, 0.5f, z);
            attempts++;
        } while (Vector3.Distance(spawnPos, player.position) < minDistanceFromPlayer && attempts < 100);

        // Safety check for assigned prefabs
        if (enemyPrefabs.Length == 0) return;

        // Spawn random enemy from available prefabs
        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
    }
}