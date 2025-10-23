using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    private float timer = 0f;
    private bool isGameOver = false;
    private int enemyKills = 0;

    void Update()
    {
        if (isGameOver) return;

        timer += Time.deltaTime;
        UpdateScoreDisplay();
    }

    // Call this when an enemy dies
    public void AddKill()
    {
        enemyKills++;
        UpdateScoreDisplay();
        Debug.Log($"Enemy killed! Total kills: {enemyKills}");
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            int secondsSurvived = Mathf.FloorToInt(timer);
            scoreText.text = $"Time: {secondsSurvived}s\nKills: {enemyKills}";
        }
    }

    // Call this to stop the timer
    public void StopTimer()
    {
        isGameOver = true;
    }

    // Call this to reset the timer when restarting
    public void ResetTimer()
    {
        timer = 0f;
        enemyKills = 0;
        isGameOver = false;
        UpdateScoreDisplay();
    }

    // Optional: Get current stats
    public int GetKills()
    {
        return enemyKills;
    }

    public float GetTimeSurvived()
    {
        return timer;
    }
}