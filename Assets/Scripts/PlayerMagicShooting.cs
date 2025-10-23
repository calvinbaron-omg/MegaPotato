using UnityEngine;

public class PlayerMagicShooting : MonoBehaviour
{
    public GameObject magicProjectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 15f;
    public float attackCooldown = 0.5f;
    
    private float lastShotTime = 0f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Rotate player toward mouse position
        RotateTowardMouse();
        
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            if (Time.time >= lastShotTime + attackCooldown)
            {
                ShootTowardMouse();
                lastShotTime = Time.time;
            }
        }
    }

    void RotateTowardMouse()
    {
        // Get mouse position in world coordinates
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(rayDistance);
            
            // Rotate player to look at mouse position (only Y rotation)
            Vector3 direction = mouseWorldPosition - transform.position;
            direction.y = 0; // Keep the rotation horizontal only
            
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    void ShootTowardMouse()
    {
        if (magicProjectilePrefab == null)
        {
            Debug.LogError("No magic projectile prefab assigned!");
            return;
        }
        
        // Get mouse position for shooting direction
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, firePoint.position);
        
        Vector3 shootDirection;
        
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(rayDistance);
            shootDirection = (mouseWorldPosition - firePoint.position).normalized;
        }
        else
        {
            // Fallback: shoot forward from player
            shootDirection = transform.forward;
        }
        
        // Create magic projectile
        GameObject magic = Instantiate(magicProjectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = magic.GetComponent<Rigidbody>();
        
        // Shoot toward mouse position
        rb.linearVelocity = shootDirection * projectileSpeed;
        
        // Optional: Rotate projectile to face shooting direction
        magic.transform.rotation = Quaternion.LookRotation(shootDirection);
        
        Debug.Log("Magic fired toward mouse!");
    }

    
    void OnDrawGizmos()
    {
        if (Application.isPlaying && mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            
            if (groundPlane.Raycast(ray, out float rayDistance))
            {
                Vector3 mouseWorldPosition = ray.GetPoint(rayDistance);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(mouseWorldPosition, 0.2f);
            }
        }
    }
}