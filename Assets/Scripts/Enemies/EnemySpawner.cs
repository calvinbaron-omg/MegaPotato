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
    public float baseSpawnInterval = 5f;   // Base interval between spawns
    public float minSpawnInterval = 1.5f;  // Hard cap so it never gets too fast
    private float spawnInterval;
    public float startDelay = 2f;
    public int enemiesPerSpawn = 2;

    [Header("Elite Spawn Chance")]
    [Range(0f, 100f)]
    public float baseEliteSpawnChance = 5f; // Base 5% chance
    public float maxEliteChance = 50f;      // Hard cap at 50%

    private float spawnTimer = 0f;
    private float runStartTime;

    void Start()
    {
        spawnInterval = baseSpawnInterval;
        spawnTimer = -startDelay;
        runStartTime = Time.time;
    }

    void Update()
    {
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

            // 🔹 Adjust difficulty dynamically
            UpdateDifficultyScaling();
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
        float effectiveEliteChance = GetScaledEliteChance();

        // Roll for elite
        bool isElite = Random.Range(0f, 100f) < effectiveEliteChance;

        GameObject enemyToSpawn = null;
        if (isElite && eliteEnemyPrefabs.Length > 0)
        {
            enemyToSpawn = eliteEnemyPrefabs[Random.Range(0, eliteEnemyPrefabs.Length)];
        }
        else if (enemyPrefabs.Length > 0)
        {
            enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        }

        if (enemyToSpawn == null) return;

        GameObject enemyInstance = Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);

        // 🔹 Apply scaling to health & damage (if manager exists)
        if (EnemyScalingManager.Instance != null)
        {
            float mult = EnemyScalingManager.Instance.GetCurrentMultiplier();
            EnemyHealth eHealth = enemyInstance.GetComponent<EnemyHealth>();
            if (eHealth != null)
            {
                eHealth.maxHealth *= mult;
                eHealth.currentHealth = eHealth.maxHealth;
            }

            EnemyMovement eMovement = enemyInstance.GetComponent<EnemyMovement>();
            if (eMovement != null)
                eMovement.damageAmount *= mult;
        }
    }

    private void UpdateDifficultyScaling()
    {
        float elapsedMinutes = (Time.time - runStartTime) / 60f;

        // 🔹 If overtime, double the spawn scaling rate
        bool isOvertime = EnemyScalingManager.Instance != null && EnemyScalingManager.Instance.IsOvertime;

        float scaleFactor = isOvertime ? 2f : 1f;
        float spawnScale = Mathf.Clamp01((elapsedMinutes / 15f) * scaleFactor);
        spawnInterval = Mathf.Lerp(baseSpawnInterval, minSpawnInterval, spawnScale);
    }

    private float GetScaledEliteChance()
    {
        float elapsedMinutes = (Time.time - runStartTime) / 60f;
        bool isOvertime = EnemyScalingManager.Instance != null && EnemyScalingManager.Instance.IsOvertime;

        float overtimeBonus = isOvertime ? 3f : 2f; // elites rise +3%/min in overtime
        float scaledChance = baseEliteSpawnChance + (elapsedMinutes * overtimeBonus);

        if (playerStats != null)
        {
            float bonusMultiplier = 1f + playerStats.GetEliteSpawnChance();
            scaledChance *= bonusMultiplier;
        }
        return Mathf.Clamp(scaledChance, 0f, maxEliteChance);
    }


}
