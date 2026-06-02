using System.Collections;
using UnityEngine;

/// Captain Bumble's death handler — from a Wall, an empty tank, or an enemy.
/// Spends a jar, plays Death anim, then respawns with a brief invulnerability blink,
/// or triggers placeholder Game Over when the last jar is gone.
public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerThrottle throttle;
    [SerializeField] private PollenDartShooter shooter;
    [SerializeField] private HoneyTank tank;
    [SerializeField] private GameOverController gameOverController;

    [Header("Death")]
    [SerializeField] private float deathAnimLength = 1f;
    [SerializeField] private Vector2 respawnPosition = new(0f, -3.5f);

    [Header("Respawn Invulnerability")]
    [SerializeField] private float invulnDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.15f;

    [Header("Bank Re-check")]

    [Tooltip("Bumble's own Collider2D — used to re-test wall overlap when invulnerability ends. Auto-grabbed if left empty.")]
    [SerializeField] private Collider2D bodyCollider;

    private bool isDead;
    private bool isInvulnerable;

    private void Awake()
    {
        if (bodyCollider == null) bodyCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        if (tank != null) tank.OnEmpty += HandleEmpty;
    }
    private void OnDisable()
    {
        if (tank != null) tank.OnEmpty -= HandleEmpty;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall")) Die();
    }

    private void HandleEmpty() => Die();

    /// Public entry point so walls, the tank, and enemies can kill Bumble.
    public void Die()
    {
        if (isDead || isInvulnerable) return;
        StartCoroutine(DieAndRespawn());
    }
    private readonly Collider2D[] overlapResults = new Collider2D[8];

    private IEnumerator DieAndRespawn()
    {
        isDead = true;

        if (movement) movement.enabled = false;
        if (throttle) throttle.enabled = false;
        if (shooter)  shooter.enabled  = false;
        if (tank)     tank.enabled     = false;

        // spend a jar right away → the UI updates instantly
        bool stillAlive = GameManager.Instance == null || GameManager.Instance.LoseLife();

        if (animator) animator.SetTrigger("Death");
        yield return new WaitForSeconds(deathAnimLength);

        if (!stillAlive)
        {
            GameOver();
            yield break;   // no respawn — the run is over
        }

        // respawn
        transform.position = respawnPosition;

        if (tank)     { tank.enabled = true; tank.Refill(); }
        if (movement) movement.enabled = true;
        if (throttle) throttle.enabled = true;
        if (shooter)  shooter.enabled  = true;

        isDead = false;
        StartCoroutine(InvulnerabilityWindow());
    }

    private IEnumerator InvulnerabilityWindow()
    {
        isInvulnerable = true;

        float elapsed = 0f;
        while (elapsed < invulnDuration)
        {
            if (spriteRenderer) spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        if (spriteRenderer) spriteRenderer.enabled = true; // ensure visible
        isInvulnerable = false;
        // If she rode out invulnerability while sitting inside a bank, kill her now.
        if (IsInsideWall()) Die();
    }

    /// True if Bumble's collider currently overlaps a wall trigger.
    /// True if Bumble's collider currently overlaps a wall trigger.
    private bool IsInsideWall()
    {
        if (bodyCollider == null) return false;

        ContactFilter2D filter = ContactFilter2D.noFilter;
        filter.useTriggers = true;   // the banks are trigger colliders

        int count = bodyCollider.Overlap(filter, overlapResults);
        for (int i = 0; i < count; i++)
            if (overlapResults[i] != null && overlapResults[i].CompareTag("Wall"))
                return true;

        return false;
    }
    private void GameOver()
    {
        if (gameOverController != null)
            gameOverController.Show();           // overlay handles state, freeze, and score
        else
            SceneLoader.LoadMainMenu();          // fallback if the controller isn't wired
    }
}