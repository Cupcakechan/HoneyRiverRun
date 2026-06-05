using System.Collections;
using UnityEngine;

/// A Honeycomb Gate baked into the Checkpoint section. Non-lethal: the player can
/// either shoot it down with darts OR simply fly through it — either way it collapses,
/// banks a checkpoint (ramping difficulty), awards points, and hides. It rides with
/// its parent section's scroll - it does NOT move itself.
public class HoneycombGate : MonoBehaviour, IDamageable
{
    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private float destroyAnimLength = 0.8f;

    [Header("Health (dart hits to pop it early)")]
    [SerializeField] private int maxHits = 2;

    [Header("Scoring")]
    [SerializeField] private int scoreValue = 500;

    [Header("Collider")]
    [SerializeField] private Collider2D bodyCollider;

    [Header("Audio")]
    [SerializeField] private AudioClip destroyClip;

    private int hitsRemaining;
    private bool isCollapsed;

    private void Start()
    {
        hitsRemaining = maxHits;
        isCollapsed = false;
        if (bodyCollider != null) bodyCollider.enabled = true;
        if (animator != null) animator.Play("Intact");
    }

    // Flying through it (the player clears the gate) collapses it — no damage.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (isCollapsed) return;
        if (other.GetComponent<PlayerHealth>() != null)
            Collapse();
    }

    // IDamageable: darts can pop it early.
    public void TakeHit()
    {
        if (isCollapsed) return;
        hitsRemaining--;
        if (hitsRemaining <= 0) Collapse();
    }

    private void Collapse()
    {
        isCollapsed = true;
        if (bodyCollider != null) bodyCollider.enabled = false;

        AudioManager.Instance?.PlaySfx(destroyClip);
        ScoreManager.Instance?.AddScore(scoreValue);
        if (GameManager.Instance != null) GameManager.Instance.RegisterGatePassed();

        if (animator != null) animator.Play("Destroy");
        StartCoroutine(HideAfterAnim());
    }

    private IEnumerator HideAfterAnim()
    {
        yield return new WaitForSeconds(destroyAnimLength);
        gameObject.SetActive(false);
    }
}