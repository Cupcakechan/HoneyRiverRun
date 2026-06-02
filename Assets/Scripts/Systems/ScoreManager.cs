using System;
using UnityEngine;

/// Owns the current run's score. Ticks up continuously with distance travelled
/// (driven by WorldScroll.Speed) and gains points when enemies are destroyed.
/// Lives in the Gameplay scene, so it's recreated fresh — and reset to 0 — each run.
/// Phase 11 will read Score at Game Over to feed the high-score system.
public class ScoreManager : MonoBehaviour
{
    /// Scene-scoped access for pooled prefabs (e.g. Wasp) that can't hold a scene reference.
    public static ScoreManager Instance { get; private set; }

    [Header("Distance Scoring")]
    [Tooltip("Points earned per world-unit travelled. Higher = faster score climb.")]
    [SerializeField] private float pointsPerUnit = 1f;

    /// Current run score.
    public int Score { get; private set; }

    /// Fired whenever the score changes, with the new total. UI subscribes to this.
    public event Action<int> OnScoreChanged;

    private float carry;   // fractional points not yet added as whole numbers

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        OnScoreChanged?.Invoke(Score);   // paint the starting 0
    }

    private void Update()
    {
        // Only score while actively playing (guards the Game Over frame / future pause).
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing)
            return;

        // Accumulate distance-based points; add whole points as they build up.
        carry += WorldScroll.Speed * Time.deltaTime * pointsPerUnit;

        int whole = (int)carry;
        if (whole > 0)
        {
            carry -= whole;
            AddScore(whole);
        }
    }

    /// Add points from any source (distance tick, enemy kills, later Gates/combos).
    public void AddScore(int amount)
    {
        if (amount <= 0) return;
        Score += amount;
        OnScoreChanged?.Invoke(Score);
    }
}