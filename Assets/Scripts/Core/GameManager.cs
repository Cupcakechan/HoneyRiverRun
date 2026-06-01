using System;
using UnityEngine;

public enum GameState { Menu, Playing, Paused, GameOver }

/// Persistent singleton. Holds shared game state, lives, and high score across scenes.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; } = GameState.Menu;

    [SerializeField] private int startingLives = 3;
    public int Lives { get; private set; }
    public event Action<int> OnLivesChanged;

    private const string HighScoreKey = "HighScore";
    public int HighScore { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        Lives = startingLives;
    }

    public void SetState(GameState newState) => State = newState;

    /// Begin a fresh run: reset lives and enter Playing.
    public void StartRun()
    {
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

    /// Returns true if this score became the new high score.
    public bool TrySetHighScore(int score)
    {
        if (score <= HighScore) return false;
        HighScore = score;
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
        PlayerPrefs.Save();
        return true;
    }
}