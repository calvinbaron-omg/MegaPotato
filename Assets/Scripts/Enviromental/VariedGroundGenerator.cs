using UnityEngine;

public class VariedGroundGenerator : MonoBehaviour
{
    public int gridSize = 10;
    public Material[] groundMaterials;
    [Range(0f, 1f)] public float variationChance = 0.2f;
    public string groundLayerName = "Ground";
    public bool addColliders = true;
    
    [Tooltip("Creates a single large collider underneath all ground tiles to prevent gaps between tiles that can cause falling through the world")]
    public bool createBackupGround = true;

    void Awake()
    {
        GenerateVariedGrid();
    }
    
    void GenerateVariedGrid()
    {
        GameObject groundParent = new GameObject("VariedGroundGrid");
        
        // Get or create the ground layer
        int groundLayer = LayerMask.NameToLayer(groundLayerName);
        if (groundLayer == -1)
        {
            Debug.LogError($"Ground layer '{groundLayerName}' does not exist! Please create it in Project Settings > Tags and Layers");
            return;
        }
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.name = $"GroundTile_{x}_{z}";
                
                plane.transform.position = new Vector3(
                    x * 10f - (gridSize * 10f / 2) + 5f, 
                    0, 
                    z * 10f - (gridSize * 10f / 2) + 5f
                );
                
                // Set the ground layer
                plane.layer = groundLayer;
                
                // Remove collider if disabled (planes have MeshCollider by default)
                if (!addColliders)
                {
                    Collider existingCollider = plane.GetComponent<Collider>();
                    if (existingCollider != null)
                        DestroyImmediate(existingCollider);
                }
                
                Material chosenMaterial = ChooseMaterial(x, z);
                plane.GetComponent<Renderer>().material = chosenMaterial;
                
                plane.transform.SetParent(groundParent.transform);
            }
        }
        
        // Create backup ground collider to prevent gaps between tiles
        // This ensures consistent ground detection even if there are small gaps in the tile grid
        if (createBackupGround)
        {
            CreateBackupGroundCollider(groundParent);
        }
    }
    
    void CreateBackupGroundCollider(GameObject groundParent)
    {
        GameObject backupGround = new GameObject("BackupGroundCollider");
        backupGround.transform.SetParent(groundParent.transform);
        backupGround.layer = LayerMask.NameToLayer(groundLayerName);
        
        BoxCollider collider = backupGround.AddComponent<BoxCollider>();
        
        // Calculate size and position to cover entire grid
        float totalSize = gridSize * 10f;
        collider.center = new Vector3(0f, -0.1f, 0f); // Slightly below the visible ground
        collider.size = new Vector3(totalSize, 0.1f, totalSize); // Cover entire grid area
    }
    
    Material ChooseMaterial(int x, int z)
    {
        if (groundMaterials == null || groundMaterials.Length == 0)
            return null;
        if (groundMaterials.Length == 1)
            return groundMaterials[0];
        
        if (Random.value > variationChance)
        {
            return groundMaterials[0];
        }
        else
        {
            int randomIndex = Random.Range(1, groundMaterials.Length);
            return groundMaterials[randomIndex];
        }
    }
}