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

    [Header("Death")]
    [SerializeField] private float deathAnimLength = 1f;
    [SerializeField] private Vector2 respawnPosition = new(0f, -3.5f);

    [Header("Respawn Invulnerability")]
    [SerializeField] private float invulnDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.15f;

    private bool isDead;
    private bool isInvulnerable;

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
    }

    private void GameOver()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetState(GameState.GameOver);
        SceneLoader.LoadMainMenu();   // placeholder — Phase 11 adds the real Game Over screen
    }
}