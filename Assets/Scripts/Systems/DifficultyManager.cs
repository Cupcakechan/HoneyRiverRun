using UnityEngine;

/// Ramps difficulty each time a Honeycomb Gate is passed. Resets at the start of
/// every run (recreated with the Gameplay scene) and clamps to HIGH caps so the
/// game keeps climbing through any realistic run without literally spawning every frame.
public class DifficultyManager : MonoBehaviour
{
    [Header("Per-gate increments")]
    [SerializeField] private float scrollStep     = 0.06f;   // +6%  river speed per gate
    [SerializeField] private float enemyRateStep  = 0.15f;   // +15% enemy frequency per gate
    [SerializeField] private float enemySpeedStep = 0.20f;   // +20% enemy own-movement per gate
    [SerializeField] private float orbRateStep    = 0.10f;   // -10% orb frequency per gate
    [SerializeField] private float islandRateStep = 0.18f;   // +18% island frequency per gate

    [Header("Caps (set high so a normal run never plateaus)")]
    [SerializeField] private float maxScroll     = 3.0f;
    [SerializeField] private float maxEnemyRate  = 5.0f;
    [SerializeField] private float maxEnemySpeed = 4.0f;
    [SerializeField] private float minOrbRate    = 0.30f;
    [SerializeField] private float maxIslandRate = 4.0f;

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
        Difficulty.ScrollMultiplier     = Mathf.Min(maxScroll,     Difficulty.ScrollMultiplier     + scrollStep);
        Difficulty.EnemyRateMultiplier  = Mathf.Min(maxEnemyRate,  Difficulty.EnemyRateMultiplier  + enemyRateStep);
        Difficulty.EnemySpeedMultiplier = Mathf.Min(maxEnemySpeed, Difficulty.EnemySpeedMultiplier + enemySpeedStep);
        Difficulty.OrbRateMultiplier    = Mathf.Max(minOrbRate,    Difficulty.OrbRateMultiplier    - orbRateStep);
        Difficulty.IslandRateMultiplier = Mathf.Min(maxIslandRate, Difficulty.IslandRateMultiplier + islandRateStep);
    }
}