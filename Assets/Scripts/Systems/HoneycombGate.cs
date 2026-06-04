using System.Collections;
using UnityEngine;

/// A Honeycomb Gate baked into the Checkpoint section. Lethal on contact until
/// destroyed; takes several dart hits, then plays its destruction animation, banks
/// a checkpoint (which ramps difficulty), awards points, and hides itself.
/// It rides with its parent section's scroll - it does NOT move itself.
public class HoneycombGate : MonoBehaviour, IDamageable
{
    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private float destroyAnimLength = 0.8f;   // your Destroy clip length

    [Header("Health")]
    [SerializeField] private int maxHits = 2;

    [Header("Scoring")]
    [SerializeField] private int scoreValue = 500;   // GDD §11: gate = 500

    [Header("Collider")]
    [SerializeField] private Collider2D bodyCollider; // disabled once destroyed

    private int hitsRemaining;
    private bool isDestroyed;

    // Fresh instance each checkpoint (the streamer instantiates a new section),
    // so initialising in Start resets it automatically.
    private void Start()
    {
        hitsRemaining = maxHits;
        isDestroyed = false;
        if (bodyCollider != null) bodyCollider.enabled = true;
        if (animator != null) animator.Play("Intact");
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
        StartCoroutine(HideAfterAnim());
    }

    private IEnumerator HideAfterAnim()
    {
        yield return new WaitForSeconds(destroyAnimLength);
        gameObject.SetActive(false);   // hide; the section recycles the rest
    }
}