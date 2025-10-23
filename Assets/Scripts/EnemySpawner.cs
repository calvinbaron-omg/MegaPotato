using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;  // assign all enemy prefabs here
    public Transform player;

    [Header("Spawn Settings")]
    public float minDistanceFromPlayer = 10f;
    public float spawnRangeX = 20f;
    public float spawnRangeZ = 20f;

    [Header("Spawn Timing")]
    public float spawnInterval = 5f;    // seconds between spawns
    public float startDelay = 2f;       // initial delay
    public int enemiesPerSpawn = 2;     // how many enemies to spawn each interval

    private float spawnTimer = 0f;

    void Start()
    {
        spawnTimer = -startDelay; // wait before first spawn
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                SpawnEnemyAtRandomPosition();
            }
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

        if (enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned!");
            return;
        }

        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
    }
}
