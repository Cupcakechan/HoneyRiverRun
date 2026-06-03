using System.Collections;
using UnityEngine;

/// A Honeycomb Gate — a section divider that spans the river and scrolls down with
/// the world. Lethal on contact until destroyed; takes several dart hits, then plays
/// its destruction animation, banks a checkpoint, awards points, and despawns.
public class HoneycombGate : MonoBehaviour, ISpawnable, IDamageable
{
    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private float destroyAnimLength = 0.8f;   // your Destroy clip length

    [Header("Health")]
    [SerializeField] private int maxHits = 5;

    [Header("Scoring")]
    [SerializeField] private int scoreValue = 500;   // GDD §11: gate = 500

    [Header("Placement")]
    [SerializeField] private float topY = 7f;        // spawn just above the view
    [SerializeField] private float despawnY = -8f;   // below the view → recycle

    [Header("Collider")]
    [SerializeField] private Collider2D bodyCollider; // disabled once destroyed

    private int hitsRemaining;
    private bool isDestroyed;

    // ── ISpawnable: called by the GateSpawner right after activation ──
    public void OnSpawned()
    {
        hitsRemaining = maxHits;
        isDestroyed = false;
        if (bodyCollider != null) bodyCollider.enabled = true;
        transform.position = new Vector3(0f, topY, 0f);   // centered across the channel
        if (animator != null) animator.Play("Intact");
    }

    private void Update()
    {
        transform.position += Vector3.down * (WorldScroll.Speed * Time.deltaTime);

        if (transform.position.y <= despawnY)
            gameObject.SetActive(false);   // off-screen (e.g. player died, gate passed) → pool
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroyed) return;
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null) player.Die();   // contact kills until the gate is destroyed
    }

    // ── IDamageable: a Pollen Dart struck the gate ──
    public void TakeHit()
    {
        if (isDestroyed) return;

        hitsRemaining--;
        if (hitsRemaining <= 0) Demolish();
    }

    private void Demolish()
    {
        isDestroyed = true;
        if (bodyCollider != null) bodyCollider.enabled = false;   // no more contact or hits

        ScoreManager.Instance?.AddScore(scoreValue);
        if (GameManager.Instance != null) GameManager.Instance.RegisterGatePassed();

        if (animator != null) animator.Play("Destroy");
        StartCoroutine(DespawnAfterAnim());
    }

    private IEnumerator DespawnAfterAnim()
    {
        yield return new WaitForSeconds(destroyAnimLength);
        gameObject.SetActive(false);   // back to the pool
    }
}