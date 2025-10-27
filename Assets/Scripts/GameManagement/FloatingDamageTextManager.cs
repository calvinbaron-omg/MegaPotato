using UnityEngine;

public class FloatingDamageTextManager : MonoBehaviour
{
    public static FloatingDamageTextManager Instance;
    [SerializeField] private GameObject floatingTextPrefab;

    void Awake()
    {

        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Spawn(Vector3 position, float damage, bool isCrit)
    {
        if (floatingTextPrefab == null) return;
        GameObject text = Instantiate(floatingTextPrefab, position, Quaternion.identity);
        text.GetComponent<FloatingDamageText>().Initialize(damage, isCrit);
    }
}
