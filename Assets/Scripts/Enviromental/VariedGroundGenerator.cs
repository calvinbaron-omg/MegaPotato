using UnityEngine;

public class VariedGroundGenerator : MonoBehaviour
{
    public int gridSize = 10;                            // Size of ground grid (10x10 planes)
    public Material[] groundMaterials;                   // Array of ground materials for variation
    [Range(0f, 1f)] public float variationChance = 0.2f; // 20% chance for alternative materials
    
    void Start()
    {
        GenerateVariedGrid();
    }
    
    void GenerateVariedGrid()
    {
        // Create parent object to organize all ground tiles
        GameObject groundParent = new GameObject("VariedGroundGrid");
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Create individual ground plane
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.name = $"GroundTile_{x}_{z}";
                
                // Position plane in grid layout (centered around origin)
                plane.transform.position = new Vector3(
                    x * 10f - (gridSize * 10f / 2) + 5f, 
                    0, 
                    z * 10f - (gridSize * 10f / 2) + 5f
                );
                
                // Assign random material based on variation chance
                Material chosenMaterial = ChooseMaterial(x, z);
                plane.GetComponent<Renderer>().material = chosenMaterial;
                
                // Organize under parent object
                plane.transform.SetParent(groundParent.transform);
            }
        }
    }
    
    Material ChooseMaterial(int x, int z)
    {
        // Safety checks for material array
        if (groundMaterials == null || groundMaterials.Length == 0)
            return null;
        if (groundMaterials.Length == 1)
            return groundMaterials[0];
        
        // Primary material used 80% of the time, variations 20%
        if (Random.value > variationChance)
        {
            return groundMaterials[0]; // Primary material (e.g., grass)
        }
        else
        {
            // Randomly select from alternative materials
            int randomIndex = Random.Range(1, groundMaterials.Length);
            return groundMaterials[randomIndex];
        }
    }
}