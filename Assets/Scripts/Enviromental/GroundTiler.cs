using UnityEngine;

public class GroundTiler : MonoBehaviour
{
    [Header("Ground Settings")]
    public GameObject groundTilePrefab;
    public float tileSize = 10f;
    public int worldSize = 10;
    public bool centerOnPlayer = true;
    public Transform player;

    [Header("Decoration Settings")]
    public GameObject[] treePrefabs;     // drag your tree prefabs here
    [Range(0f, 1f)] public float treeChance = 0.1f; // 10% chance per tile to spawn a tree
    public Vector2 treeOffsetRange = new Vector2(3f, 3f); // random position offset inside tile

    void Start()
    {
        if (groundTilePrefab == null)
        {
            Debug.LogError("GroundTiler: Missing groundTilePrefab reference!");
            return;
        }

        GenerateWorld();
    }

    void GenerateWorld()
    {
        Vector3 startPos = Vector3.zero;

        if (centerOnPlayer && player != null)
        {
            startPos = new Vector3(
                player.position.x - (worldSize / 2f) * tileSize,
                0f,
                player.position.z - (worldSize / 2f) * tileSize
            );
        }

        for (int x = 0; x < worldSize; x++)
        {
            for (int z = 0; z < worldSize; z++)
            {
                Vector3 pos = startPos + new Vector3(x * tileSize, 0f, z * tileSize);
                GameObject tile = Instantiate(groundTilePrefab, pos, Quaternion.identity, transform);

                TrySpawnTree(tile.transform, pos);
            }
        }

        Debug.Log($"Generated world with {worldSize * worldSize} tiles ({worldSize}x{worldSize})");
    }

    void TrySpawnTree(Transform parent, Vector3 tilePosition)
    {
        if (treePrefabs == null || treePrefabs.Length == 0) return;

        // Only spawn some tiles
        if (Random.value > treeChance) return;

        // Pick a random tree prefab
        GameObject chosenTree = treePrefabs[Random.Range(0, treePrefabs.Length)];

        // Random offset inside the tile
        float offsetX = Random.Range(-treeOffsetRange.x, treeOffsetRange.x);
        float offsetZ = Random.Range(-treeOffsetRange.y, treeOffsetRange.y);
        Vector3 spawnPos = new Vector3(tilePosition.x + offsetX, 0f, tilePosition.z + offsetZ);

        // Slight random rotation for variation
        Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        Instantiate(chosenTree, spawnPos, rot, parent);
    }
}
