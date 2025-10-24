using UnityEngine;
using TMPro;
using System.Collections;

public class LevelUpUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject levelUpPanel;
    public TMP_Text levelUpText;

    [Header("Settings")]
    [Tooltip("Which key/button resumes the game after leveling up.")]
    public KeyCode continueKey = KeyCode.Space;

    private Coroutine waitRoutine;

    void OnEnable()
    {
        PlayerLevel.OnLevelUp += ShowLevelUp;
    }

    void OnDisable()
    {
        PlayerLevel.OnLevelUp -= ShowLevelUp;
    }

    void ShowLevelUp(int newLevel)
    {
        if (levelUpPanel == null || levelUpText == null)
            return;

        // Pause the game
        Time.timeScale = 0f;

        // Show panel and update text
        levelUpPanel.SetActive(true);
        levelUpText.text = $"LEVEL UP!\nLevel {newLevel}";

        // Stop any existing coroutine (safety)
        if (waitRoutine != null)
            StopCoroutine(waitRoutine);

        // Start listening for input
        waitRoutine = StartCoroutine(WaitForPlayerInput());
    }

    IEnumerator WaitForPlayerInput()
    {
        // Wait until player presses the continue key
        while (!Input.GetKeyDown(continueKey))
            yield return null;

        // Hide the overlay and resume the game
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;

        waitRoutine = null;
    }
    public void OnContinueButtonPressed()
{
    // Hide the panel and resume the game
    if (levelUpPanel != null)
        levelUpPanel.SetActive(false);

    Time.timeScale = 1f;
}
}
