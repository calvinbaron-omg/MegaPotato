using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool isGameOver = false;
    public TMPro.TMP_Text gameOverText;

    public void GameOver()
    {
        Time.timeScale = 0; // Pause game
        isGameOver = true;

    if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);

        // Stop the timer
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
            scoreManager.StopTimer();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Resume time
        Time.timeScale = 1;
        isGameOver = false;

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
            scoreManager.ResetTimer();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void TriggerGameOver()
    {
        GameOver();
    }

}
