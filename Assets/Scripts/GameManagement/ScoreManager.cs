using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text timeSurvivedText;    // UI Text element to display time
    public TMP_Text enemiesKilledText;   // UI Text element to display kills

    private float timer = 600f;          // Start at 10 minutes (600 seconds)
    private bool isCountingDown = true;  // Starts by counting down
    private bool isGameOver = false;
    private int enemyKills = 0;

    void Update()
    {
        if (isGameOver) return;

        if (isCountingDown)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = 0f;
                isCountingDown = false;

                // 🔹 Tell EnemyScalingManager to switch to overtime mode
                if (EnemyScalingManager.Instance != null)
                    EnemyScalingManager.Instance.SetOvertime(true);
            }
        }
        else
        {
            timer += Time.deltaTime;
        }

        UpdateScoreDisplay();
    }


    public void AddKill()
    {
        enemyKills++;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (timeSurvivedText != null && enemiesKilledText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            string timeString = $"{minutes:00}:{seconds:00}";

            // Indicate whether we’re counting down or up
            string prefix = isCountingDown ? "Countdown" : "Overtime";

            timeSurvivedText.text = $"{prefix}: {timeString}";
            enemiesKilledText.text = $"Kills: {enemyKills}";
        }
    }

    public void StopTimer() => isGameOver = true;

    public void ResetTimer()
    {
        timer = 600f;           // Reset back to 10 minutes
        isCountingDown = true;  // Start counting down again
        enemyKills = 0;
        isGameOver = false;
        UpdateScoreDisplay();
    }

    public int GetKills() => enemyKills;

    public float GetTimeSurvived() => timer;
}
