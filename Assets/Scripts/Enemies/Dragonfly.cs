using System.Collections;
using UnityEngine;

/// A Dragonfly enemy — the Tracker. Spawns at the top and eases its X toward the
/// player while descending, so it chases. Uses the same animation states as the Wasp
/// (FlySouth / FlyEast / FlyWest + Hurt). Kills Bumble on contact; a dart hit makes it
/// flash Hurt then return to the pool. Pooled + placed by the shared Spawner.
public class Dragonfly : MonoBehaviour, ISpawnable, IDamageable
{
    private enum Facing { South, East, West }

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private float hurtAnimLength = 0.9f;

    [Header("Movement")]
    [SerializeField] private float selfDescendSpeed = 1f;   // independent descent, scaled by Difficulty.EnemySpeedMultiplier
    [SerializeField] private float trackSpeed = 3f;          // horizontal homing speed toward the player
    [SerializeField] private float facingDeadzone = 0.3f;    // X gap within which it faces straight down

    [Header("Spawn placement")]
    [SerializeField] private float topY = 6.5f;
    [SerializeField] private float channelMinX = -4f;
    [SerializeField] private float channelMaxX = 4f;

    [Header("Despawn bounds")]
    [SerializeField] private float despawnY = -7f;
    [SerializeField] private float despawnX = 6f;

    [Header("Scoring")]
    [SerializeField] private int scoreValue = 60;

    [Header("Audio")]
    [SerializeField] private AudioClip hitClip;

    private bool isHit;
    private Facing facing;
    private static Transform playerTransform;

    // ── ISpawnable ──
    public void OnSpawned()
    {
        isHit = false;
        transform.position = new Vector3(Random.Range(channelMinX, channelMaxX), topY, 0f);
        facing = Facing.South;
        if (animator != null) animator.Play("FlySouth");
    }

    private void Update()
    {
        if (isHit) return;

        Vector3 p = transform.position;

        // independent descent so it stays dangerous even when the river is throttled slow
        float descent = WorldScroll.Speed + selfDescendSpeed * Difficulty.EnemySpeedMultiplier;
        p.y -= descent * Time.deltaTime;

        // home horizontally toward the player
        Transform player = GetPlayer();
        if (player != null)
        {
            float targetX = player.position.x;
            p.x = Mathf.MoveTowards(p.x, targetX, trackSpeed * Time.deltaTime);
            UpdateFacing(targetX - p.x);   // remaining gap → which way it's chasing
        }

        transform.position = p;

        if (p.y < despawnY || p.x < -despawnX || p.x > despawnX)
            gameObject.SetActive(false);   // off-screen → back to the pool
    }

    private void UpdateFacing(float diff)
    {
        Facing want = Mathf.Abs(diff) < facingDeadzone ? Facing.South
                    : (diff > 0f ? Facing.East : Facing.West);
        if (want == facing) return;
        facing = want;
        if (animator == null) return;
        animator.Play(want == Facing.East ? "FlyEast" : want == Facing.West ? "FlyWest" : "FlySouth");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isHit) return;
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null) player.Die();   // contact kills Bumble
    }

    /// Called by a Pollen Dart that strikes this Dragonfly.
    public void TakeHit()
    {
        if (isHit) return;
        isHit = true;

        ScoreManager.Instance?.AddScore(scoreValue);
        AudioManager.Instance?.PlaySfx(hitClip);

        if (animator != null) animator.Play("Hurt");
        StartCoroutine(HurtThenDespawn());
    }

    private IEnumerator HurtThenDespawn()
    {
        yield return new WaitForSeconds(hurtAnimLength);
        gameObject.SetActive(false);   // back to the pool
    }

    // Cached once; re-finds automatically after a scene reload (destroyed ref reads as null).
    private static Transform GetPlayer()
    {
        if (playerTransform == null)
        {
            PlayerHealth ph = FindAnyObjectByType<PlayerHealth>();
            playerTransform = ph != null ? ph.transform : null;
        }
        return playerTransform;
    }
}