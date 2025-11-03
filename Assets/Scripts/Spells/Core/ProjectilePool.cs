using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int initialSize = 10;
        [HideInInspector] public Queue<GameObject> objects = new Queue<GameObject>();
    }

    [SerializeField] private List<Pool> pools = new List<Pool>();
    private Dictionary<GameObject, Pool> prefabToPool = new Dictionary<GameObject, Pool>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var pool in pools)
        {
            if (pool.prefab == null) continue;
            prefabToPool[pool.prefab] = pool;

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                pool.objects.Enqueue(obj);
            }
        }
    }

    public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!prefabToPool.TryGetValue(prefab, out var pool))
        {
            pool = new Pool { prefab = prefab, initialSize = 0 };
            prefabToPool[prefab] = pool;
        }

        GameObject obj = pool.objects.Count > 0 ? pool.objects.Dequeue() : Instantiate(prefab);
        obj.transform.SetPositionAndRotation(pos, rot);
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject prefab, GameObject obj)
    {
        if (prefab == null || obj == null) return;

        obj.SetActive(false);
        if (prefabToPool.TryGetValue(prefab, out var pool))
            pool.objects.Enqueue(obj);
        else
            Destroy(obj);
    }
}
