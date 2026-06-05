using TMPro;
using UnityEngine;

/// Drives the Game Over overlay in the Gameplay scene. Freezes the run, shows the
/// final score, and — when it beats the stored high score — collects 3-letter
/// initials before saving. Retry restarts a run; Menu returns to the Main Menu.
public class GameOverController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;            // the full-screen overlay (starts inactive)

    [Header("Score Display")]
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text highScoreText;

    [Header("Initials Entry (new high score only)")]
    [SerializeField] private GameObject initialsGroup;    // label + input + submit
    [SerializeField] private TMP_InputField initialsInput;

    [Header("Navigation")]
    [SerializeField] private GameObject navGroup;         // Retry + Menu buttons

    [Header("Audio")]
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private bool loopGameOverMusic = true; 
    private int finalScore;

    /// Called by PlayerHealth when the last life is spent.
    public void Show()
    {
        finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0;

        if (GameManager.Instance != null)
            GameManager.Instance.SetState(GameState.GameOver);

        Time.timeScale = 0f;                 // freeze the world behind the overlay
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(gameOverMusic, loopGameOverMusic);   // NEW
        if (panel != null) panel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "SCORE " + finalScore.ToString("D7");

        bool isNewHigh = GameManager.Instance != null && GameManager.Instance.IsHighScore(finalScore);

        if (isNewHigh)
        {
            // Force initials entry before the player can leave.
            if (initialsGroup != null) initialsGroup.SetActive(true);
            if (navGroup != null) navGroup.SetActive(false);
            if (initialsInput != null)
            {
                initialsInput.text = "";
                initialsInput.ActivateInputField();   // focus so they can type immediately
            }
        }
        else
        {
            if (initialsGroup != null) initialsGroup.SetActive(false);
            if (navGroup != null) navGroup.SetActive(true);
        }

        ShowBestLine();
    }

    /// Wired to the Submit button's OnClick.
    public void OnSubmitInitials()
    {
        string initials = initialsInput != null ? initialsInput.text.Trim() : "";
        if (string.IsNullOrEmpty(initials)) initials = "AAA";

        if (GameManager.Instance != null)
            GameManager.Instance.SaveHighScore(finalScore, initials);

        if (initialsGroup != null) initialsGroup.SetActive(false);
        if (navGroup != null) navGroup.SetActive(true);
        ShowBestLine();                                // now reflects the new record
    }

    /// Wired to the Retry button's OnClick.
    public void OnRetry()
    {
        if (GameManager.Instance != null) GameManager.Instance.StartRun();  // also resets timeScale
        else Time.timeScale = 1f;
        SceneLoader.LoadGameplay();
    }

    /// Wired to the Menu button's OnClick.
    public void OnMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null) GameManager.Instance.SetState(GameState.Menu);
        SceneLoader.LoadMainMenu();
    }

    private void ShowBestLine()
    {
        if (highScoreText == null || GameManager.Instance == null) return;
        highScoreText.text = $"BEST {GameManager.Instance.HighScoreInitials} {GameManager.Instance.HighScore:D7}";
    }
}