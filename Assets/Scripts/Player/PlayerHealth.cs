using System.Collections;
using UnityEngine;

/// Handles Captain Bumble's death. For now: hit a Wall → play Death anim →
/// placeholder respawn at channel center. Lives/Game Over arrive in later phases.
public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerThrottle throttle;
    [SerializeField] private PollenDartShooter shooter;

    [Header("Death")]
    [SerializeField] private float deathAnimLength = 1f;          // your Death clip length
    [SerializeField] private Vector2 respawnPosition = new(0f, -3.5f);

    private bool isDead;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        if (other.CompareTag("Wall"))
            StartCoroutine(DieAndRespawn());
    }

    private IEnumerator DieAndRespawn()
    {
        isDead = true;

        if (movement) movement.enabled = false;
        if (throttle) throttle.enabled = false;
        if (shooter)  shooter.enabled  = false;

        if (animator) animator.SetTrigger("Death");

        yield return new WaitForSeconds(deathAnimLength);

        // placeholder respawn
        transform.position = respawnPosition;

        if (movement) movement.enabled = true;
        if (throttle) throttle.enabled = true;   // re-enabling resets scroll speed to baseline
        if (shooter)  shooter.enabled  = true;

        isDead = false;
    }
}