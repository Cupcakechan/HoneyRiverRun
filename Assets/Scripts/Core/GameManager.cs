using UnityEngine;

public enum GameState { Menu, Playing, Paused, GameOver }

/// Persistent singleton. Holds shared game state + high score across scenes.
/// Lives in the MainMenu scene (scene 0), so it's created at boot and survives into Gameplay.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; } = GameState.Menu;

    private const string HighScoreKey = "HighScore";
    public int HighScore { get; private set; }

    private void Awake()
    {
        // Singleton guard: if one already exists (e.g. we returned to MainMenu), destroy this duplicate.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public void SetState(GameState newState) => State = newState;

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