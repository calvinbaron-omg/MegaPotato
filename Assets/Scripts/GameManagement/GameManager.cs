using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text gameOverText;
    public GameObject damageMeterPanel; // Assign a Canvas or Panel prefab in Inspector
    public TMP_Text damageMeterText;    // Text inside the panel for displaying stats

    private bool isPaused = false;

    public void GameOver()
    {
        Time.timeScale = 0;

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);

        var scoreManager = FindAnyObjectByType<ScoreManager>();
        scoreManager?.StopTimer();

        var playerCurrency = FindAnyObjectByType<PlayerCurrency>();
        playerCurrency?.OnRunEnd();
    }

    void Update()
    {
        // 🔹 Restart shortcut
        if (Input.GetKeyDown(KeyCode.R))
            RestartGame();

        // 🔹 Pause / Resume on ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGameAndShowDamageMeter();
            else
                ResumeGame();
        }
    }

    private void PauseGameAndShowDamageMeter()
    {
        isPaused = true;
        Time.timeScale = 0;

        // Build the damage meter summary once
        if (damageMeterPanel != null)
            damageMeterPanel.SetActive(true);

        if (damageMeterText != null && DamageTracker.Instance != null)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("<b>Damage Summary</b>\n");

            foreach (var (src, total, dps) in DamageTracker.Instance.GetAll())
            {
                sb.AppendLine($"{src,-15}  {total,8:F0} dmg  |  {dps,6:F1} DPS");
            }

            damageMeterText.text = sb.ToString();
        }
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;

        if (damageMeterPanel != null)
            damageMeterPanel.SetActive(false);
    }

    public void RestartGame()
    {
        var playerCurrency = FindAnyObjectByType<PlayerCurrency>();
        playerCurrency?.OnRunStart();

        Time.timeScale = 1;
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        var scoreManager = FindAnyObjectByType<ScoreManager>();
        scoreManager?.ResetTimer();

        var pool = FindAnyObjectByType<SpellPoolManager>();
        pool?.ResetAllSpellsToBase();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TriggerGameOver() => GameOver();

    public void StartNewRun()
    {
        FloatingDamageTextManager.Instance.Spawn(new Vector3(0, 2, 0), 123, false);

        var playerCurrency = FindAnyObjectByType<PlayerCurrency>();
        playerCurrency?.OnRunStart();

        var stats = FindAnyObjectByType<PlayerStats>();
        stats?.ResetToBaseValues();

        Time.timeScale = 1;

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        var pool = FindAnyObjectByType<SpellPoolManager>();
        pool?.ResetAllSpellsToBase();

        var scoreManager = FindAnyObjectByType<ScoreManager>();
        scoreManager?.ResetTimer();
    }
}
