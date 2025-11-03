using System.Collections.Generic;
using UnityEngine;

public class DamageTracker : MonoBehaviour
{
    public static DamageTracker Instance { get; private set; }

    private class DamageEntry
    {
        public string source;
        public float totalDamage;
        public float firstHitTime;
    }

    private readonly Dictionary<string, DamageEntry> damageLog = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterDamage(string source, float amount)
    {
        if (string.IsNullOrEmpty(source)) source = "Unknown";
        if (!damageLog.TryGetValue(source, out var entry))
        {
            entry = new DamageEntry
            {
                source = source,
                totalDamage = 0f,
                firstHitTime = Time.time
            };
            damageLog[source] = entry;
        }

        entry.totalDamage += amount;
    }

    public float GetTotalDamage(string source)
    {
        return damageLog.TryGetValue(source, out var entry) ? entry.totalDamage : 0f;
    }

    public float GetDPS(string source)
    {
        if (!damageLog.TryGetValue(source, out var entry)) return 0f;
        float elapsed = Mathf.Max(Time.time - entry.firstHitTime, 0.1f);
        return entry.totalDamage / elapsed;
    }

    public IEnumerable<(string source, float total, float dps)> GetAll()
    {
        foreach (var e in damageLog.Values)
        {
            float elapsed = Mathf.Max(Time.time - e.firstHitTime, 0.1f);
            yield return (e.source, e.totalDamage, e.totalDamage / elapsed);
        }
    }

    public void ResetLog() => damageLog.Clear();
}
