using UnityEngine;

/// Ramps difficulty each time a Honeycomb Gate is passed. Resets at the start of
/// every run (it's recreated with the Gameplay scene) and clamps to caps so the
/// game escalates without becoming impossible.
public class DifficultyManager : MonoBehaviour
{
    [Header("Per-gate increments")]
    [SerializeField] private float scrollStep    = 0.08f;   // +8%  river speed per gate
    [SerializeField] private float enemyRateStep  = 0.12f;   // +12% enemy frequency per gate
    [SerializeField] private float orbRateStep     = 0.10f;   // -10% orb frequency per gate

    [Header("Caps")]
    [SerializeField] private float maxScroll     = 2.0f;    // up to 2.0x river speed
    [SerializeField] private float maxEnemyRate   = 2.5f;    // up to 2.5x enemy density
    [SerializeField] private float minOrbRate      = 0.4f;    // down to 40% orb frequency

    private void OnEnable()
    {
        Difficulty.Reset();   // every run starts at baseline
        if (GameManager.Instance != null)
            GameManager.Instance.OnGatePassed += HandleGatePassed;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGatePassed -= HandleGatePassed;
    }

    private void HandleGatePassed(int gatesPassed)
    {
        Difficulty.ScrollMultiplier    = Mathf.Min(maxScroll,    Difficulty.ScrollMultiplier    + scrollStep);
        Difficulty.EnemyRateMultiplier = Mathf.Min(maxEnemyRate, Difficulty.EnemyRateMultiplier + enemyRateStep);
        Difficulty.OrbRateMultiplier   = Mathf.Max(minOrbRate,   Difficulty.OrbRateMultiplier   - orbRateStep);
    }
}