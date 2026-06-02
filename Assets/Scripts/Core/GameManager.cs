using System;
using UnityEngine;

public enum GameState { Menu, Playing, Paused, GameOver }

/// Persistent singleton. Holds shared game state, lives, and the high score
/// (with initials) across scenes.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; } = GameState.Menu;

    [SerializeField] private int startingLives = 3;
    public int Lives { get; private set; }
    public event Action<int> OnLivesChanged;

    private const string HighScoreKey = "HighScore";
    private const string HighScoreInitialsKey = "HighScoreInitials";
    private const string DefaultInitials = "AAA";

    public int HighScore { get; private set; }
    public string HighScoreInitials { get; private set; } = DefaultInitials;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        HighScoreInitials = PlayerPrefs.GetString(HighScoreInitialsKey, DefaultInitials);
        Lives = startingLives;
    }

    public void SetState(GameState newState) => State = newState;

    /// Begin a fresh run: reset lives, unfreeze time, and enter Playing.
    public void StartRun()
    {
        Time.timeScale = 1f;     // ensure we never start a run frozen
        Lives = startingLives;
        State = GameState.Playing;
        OnLivesChanged?.Invoke(Lives);
    }

    /// Spend a life. Returns true if at least one life remains.
    public bool LoseLife()
    {
        Lives = Mathf.Max(0, Lives - 1);
        OnLivesChanged?.Invoke(Lives);
        return Lives > 0;
    }

    /// True if this score beats the stored high score (does not save).
    public bool IsHighScore(int score) => score > HighScore;

    /// Commit a new high score and its initials to PlayerPrefs.
    public void SaveHighScore(int score, string initials)
    {
        HighScore = score;
        HighScoreInitials = string.IsNullOrWhiteSpace(initials) ? DefaultInitials : initials.ToUpper();
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
        PlayerPrefs.SetString(HighScoreInitialsKey, HighScoreInitials);
        PlayerPrefs.Save();
    }
}