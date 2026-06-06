using System.Collections;
using UnityEngine;

/// "Ant on a Boat" — the Slow Tank. Drifts downstream with the current only (no diving,
/// chasing, or weaving) and soaks several dart hits before going down. Only has a South
/// idle animation plus Hurt. Kills Bumble on contact until destroyed. Pooled + placed by
/// the shared Spawner.
public class AntBoat : MonoBehaviour, ISpawnable, IDamageable
{
    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private float hurtAnimLength  = 0.9f;   // death Hurt clip length
    [SerializeField] private float hurtFlashLength = 0.3f;   // brief flash on a non-lethal hit

    [Header("Health")]
    [SerializeField] private int maxHits = 3;                // durable — takes several darts

    [Header("Spawn placement")]
    [SerializeField] private float topY = 6.5f;
    [SerializeField] private float channelMinX = -3.5f;
    [SerializeField] private float channelMaxX = 3.5f;

    [Header("Despawn")]
    [SerializeField] private float despawnY = -7f;

    [Header("Scoring")]
    [SerializeField] private int scoreValue = 80;

    [Header("Audio")]
    [SerializeField] private AudioClip hitClip;

    [Header("Movement")]
[SerializeField] private float driftSpeed = 1.5f;   // a little speed over the current so it visibly floats downstream

    private int hitsRemaining;
    private bool isDead;
    private Coroutine flashRoutine;

    // ── ISpawnable ──
    public void OnSpawned()
    {
        isDead = false;
        hitsRemaining = maxHits;
        flashRoutine = null;
        transform.position = new Vector3(Random.Range(channelMinX, channelMaxX), topY, 0f);
        if (animator != null) animator.Play("FlySouth");
    }

    private void Update()
{
    // drift downstream: the river current PLUS a little of its own, so it visibly floats down
    Vector3 p = transform.position;
    p.y -= (WorldScroll.Speed + driftSpeed) * Time.deltaTime;
    transform.position = p;

    if (p.y < despawnY)
        gameObject.SetActive(false);   // off-screen → back to the pool
}

    private void OnTriggerEnter2D(Collider2D other)
{
    if (isDead) return;

    PlayerHealth player = other.GetComponent<PlayerHealth>();
    if (player != null) { player.Die(); return; }   // contact kills until destroyed

    if (other.GetComponent<Island>() != null)
        Wreck();   // a drifting boat hits land and breaks up
}

    /// Called by a Pollen Dart that strikes the boat.
    public void TakeHit()
    {
        if (isDead) return;

        hitsRemaining--;
        AudioManager.Instance?.PlaySfx(hitClip);

        if (hitsRemaining <= 0)
        {
            Die();
            return;
        }

        // non-lethal hit: quick Hurt flash, then back to drifting
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(HurtFlash());
    }

    private IEnumerator HurtFlash()
    {
        if (animator != null) animator.Play("Hurt");
        yield return new WaitForSeconds(hurtFlashLength);
        if (!isDead && animator != null) animator.Play("FlySouth");
    }

    private void Die()
    {
        isDead = true;
        if (flashRoutine != null) StopCoroutine(flashRoutine);

        ScoreManager.Instance?.AddScore(scoreValue);
        if (animator != null) animator.Play("Hurt");
        StartCoroutine(DeathThenDespawn());
    }

    /// Crashed into an island — break up and despawn. No score (the player didn't do it).
private void Wreck()
{
    isDead = true;
    if (flashRoutine != null) StopCoroutine(flashRoutine);
    if (animator != null) animator.Play("Hurt");
    StartCoroutine(DeathThenDespawn());
}

    private IEnumerator DeathThenDespawn()
    {
        yield return new WaitForSeconds(hurtAnimLength);
        gameObject.SetActive(false);   // back to the pool
    }
}