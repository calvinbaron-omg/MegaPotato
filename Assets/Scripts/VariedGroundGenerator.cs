using UnityEngine;

public class VariedGroundGenerator : MonoBehaviour
{
    public int gridSize = 10;
    public Material[] groundMaterials; // Drag multiple materials here
    [Range(0f, 1f)] public float variationChance = 0.2f; // 20% chance for different material
    
    void Start()
    {
        GenerateVariedGrid();
    }
    
    void GenerateVariedGrid()
    {
        // Create parent object
        GameObject groundParent = new GameObject("VariedGroundGrid");
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Create plane
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.name = $"GroundTile_{x}_{z}";
                
                // Position the plane
                plane.transform.position = new Vector3(
                    x * 10f - (gridSize * 10f / 2) + 5f, 
                    0, 
                    z * 10f - (gridSize * 10f / 2) + 5f
                );
                
                // Choose material
                Material chosenMaterial = ChooseMaterial(x, z);
                plane.GetComponent<Renderer>().material = chosenMaterial;
                
                plane.transform.SetParent(groundParent.transform);
            }
        }
        
        Debug.Log($"Generated varied {gridSize}x{gridSize} ground grid!");
    }
    
    Material ChooseMaterial(int x, int z)
    {
        // If no materials or only one, just return the first
        if (groundMaterials == null || groundMaterials.Length == 0)
            return null;
        if (groundMaterials.Length == 1)
            return groundMaterials[0];
        
        // 80% chance for primary material, 20% for variation
        if (Random.value > variationChance)
        {
            return groundMaterials[0]; // Primary material (grass)
        }
        else
        {
            // Randomly pick from other materials (excluding the first one)
            int randomIndex = Random.Range(1, groundMaterials.Length);
            return groundMaterials[randomIndex];
        }
    }
}