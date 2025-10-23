using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;    // UI Text element to display score
    private float timer = 0f;     // Time survived in seconds
    private bool isGameOver = false;
    private int enemyKills = 0;   // Total number of enemies defeated

    void Update()
    {
        // Only update timer if game is still active
        if (isGameOver) return;

        timer += Time.deltaTime;
        UpdateScoreDisplay();
    }

    // Call this when an enemy dies to increment kill count
    public void AddKill()
    {
        enemyKills++;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        // Update UI with current time survived and kill count
        if (scoreText != null)
        {
            int secondsSurvived = Mathf.FloorToInt(timer);
            scoreText.text = $"Time: {secondsSurvived}s\nKills: {enemyKills}";
        }
    }

    // Call this when game ends to stop timer
    public void StopTimer()
    {
        isGameOver = true;
    }

    // Call this when restarting game to reset stats
    public void ResetTimer()
    {
        timer = 0f;
        enemyKills = 0;
        isGameOver = false;
        UpdateScoreDisplay();
    }

    // Get current kill count (for game over screen or achievements)
    public int GetKills()
    {
        return enemyKills;
    }

    // Get time survived in seconds (for game over screen or high scores)
    public float GetTimeSurvived()
    {
        return timer;
    }
}