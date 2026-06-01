using System.Collections;
using UnityEngine;

/// Handles Captain Bumble's death — from clipping a Wall OR running the honey tank dry.
/// Plays Death anim → placeholder respawn at channel center, refilling the tank.
/// Lives/Game Over arrive in later phases.
public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerThrottle throttle;
    [SerializeField] private PollenDartShooter shooter;
    [SerializeField] private HoneyTank tank;

    [Header("Death")]
    [SerializeField] private float deathAnimLength = 1f;
    [SerializeField] private Vector2 respawnPosition = new(0f, -3.5f);

    private bool isDead;

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
        if (isDead) return;
        if (other.CompareTag("Wall"))
            Die();
    }

    private void HandleEmpty() => Die();

    /// Public entry point so other systems (e.g. the honey tank) can kill the player.
    public void Die()
    {
        if (isDead) return;
        StartCoroutine(DieAndRespawn());
    }

    private IEnumerator DieAndRespawn()
    {
        isDead = true;

        if (movement) movement.enabled = false;
        if (throttle) throttle.enabled = false;
        if (shooter)  shooter.enabled  = false;
        if (tank)     tank.enabled     = false;   // stop draining during death

        if (animator) animator.SetTrigger("Death");

        yield return new WaitForSeconds(deathAnimLength);

        transform.position = respawnPosition;      // placeholder respawn

        if (tank)     { tank.enabled = true; tank.Refill(); }  // back to a full tank
        if (movement) movement.enabled = true;
        if (throttle) throttle.enabled = true;     // re-enabling resets scroll speed to baseline
        if (shooter)  shooter.enabled  = true;

        isDead = false;
    }
}