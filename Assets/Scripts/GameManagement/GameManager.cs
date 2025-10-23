using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TMPro.TMP_Text gameOverText;

    public void GameOver()
    {
        // Pause game and show game over UI
        Time.timeScale = 0;

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);

        // Stop score timer
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.StopTimer();
        }
    }

    void Update()
    {
        // Restart game when R key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        // Resume game time and reset state
        Time.timeScale = 1;

        // Hide game over text
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        // Reset score timer
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null)
            scoreManager.ResetTimer();

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TriggerGameOver()
    {
        // Public method to trigger game over from other scripts
        GameOver();
    }
}