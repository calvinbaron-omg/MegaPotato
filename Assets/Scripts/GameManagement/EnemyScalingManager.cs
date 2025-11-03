using UnityEngine;

public class EnemyScalingManager : MonoBehaviour
{
    public static EnemyScalingManager Instance { get; private set; }

    [Header("Scaling Settings")]
    public float baseHealthMultiplier = 1f;     // Starting value
    public float healthIncreaseRate = 0.1f;     // +10% per minute (normal phase)
    public float overtimeHealthRate = 0.25f;    // +25% per minute (overtime)
    public float maxMultiplier = 10f;

    private float startTime;
    private bool inOvertime = false;


    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        startTime = Time.time;
    }

    public void SetOvertime(bool state)
    {
        inOvertime = state;
    }

    // ✅ Add this getter
    public bool IsOvertime => inOvertime;

    public float GetCurrentMultiplier()
    {
        float elapsedMinutes = (Time.time - startTime) / 60f;
        float rate = inOvertime ? overtimeHealthRate : healthIncreaseRate;
        float mult = baseHealthMultiplier + (rate * elapsedMinutes);
        return Mathf.Min(mult, maxMultiplier);
    }

}
