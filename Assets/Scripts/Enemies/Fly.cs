using System.Collections;
using UnityEngine;

/// A Fly enemy — the Weaver. Descends while sweeping left/right in a sine wave, so
/// its horizontal path is hard to predict. Uses the same animation states as the Wasp
/// (FlySouth / FlyEast / FlyWest + Hurt), facing the way it's currently weaving.
/// Kills Bumble on contact; a dart hit flashes Hurt then returns it to the pool.
public class Fly : MonoBehaviour, ISpawnable, IDamageable
{
    private enum Facing { South, East, West }

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private float hurtAnimLength = 0.9f;

    [Header("Movement")]
    [SerializeField] private float selfDescendSpeed = 1.5f;  // independent descent, scaled by Difficulty.EnemySpeedMultiplier
    [SerializeField] private float weaveAmplitude = 2.5f;     // half-width of the zigzag (world units)
    [SerializeField] private float weaveSpeed = 3f;           // higher = tighter, faster zigzag
    [SerializeField] private float channelLimit = 4.5f;       // clamp so it never weaves into the banks
    [SerializeField] private float facingDeadzone = 0.25f;    // near a turning point it faces straight down

    [Header("Spawn placement")]
    [SerializeField] private float topY = 6.5f;

    [Header("Despawn bounds")]
    [SerializeField] private float despawnY = -7f;
    [SerializeField] private float despawnX = 6f;

    [Header("Scoring")]
    [SerializeField] private int scoreValue = 50;

    [Header("Audio")]
    [SerializeField] private AudioClip hitClip;

    private bool isHit;
    private Facing facing;
    private float weaveTime;
    private float centerX;
    private float phase;

    // ── ISpawnable ──
    public void OnSpawned()
    {
        isHit = false;
        weaveTime = 0f;
        phase = Random.Range(0f, Mathf.PI * 2f);   // desync flies from each other

        // keep the whole weave inside the channel
        float centerRange = Mathf.Max(0f, channelLimit - weaveAmplitude);
        centerX = Random.Range(-centerRange, centerRange);

        float startX = Mathf.Clamp(centerX + weaveAmplitude * Mathf.Sin(phase), -channelLimit, channelLimit);
        transform.position = new Vector3(startX, topY, 0f);

        facing = Facing.South;
        if (animator != null) animator.Play("FlySouth");
    }

    private void Update()
    {
        if (isHit) return;

        weaveTime += Time.deltaTime;

        Vector3 p = transform.position;

        // independent descent so it stays dangerous even when the river is throttled slow
        float descent = WorldScroll.Speed + selfDescendSpeed * Difficulty.EnemySpeedMultiplier;
        p.y -= descent * Time.deltaTime;

        // sine-wave sweep
        float t = weaveTime * weaveSpeed + phase;
        p.x = Mathf.Clamp(centerX + weaveAmplitude * Mathf.Sin(t), -channelLimit, channelLimit);
        UpdateFacing(Mathf.Cos(t));   // cos = horizontal direction of the weave

        transform.position = p;

        if (p.y < despawnY || p.x < -despawnX || p.x > despawnX)
            gameObject.SetActive(false);   // off-screen → back to the pool
    }

    private void UpdateFacing(float dir)
    {
        Facing want = Mathf.Abs(dir) < facingDeadzone ? Facing.South
                    : (dir > 0f ? Facing.East : Facing.West);
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

    /// Called by a Pollen Dart that strikes this Fly.
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
}