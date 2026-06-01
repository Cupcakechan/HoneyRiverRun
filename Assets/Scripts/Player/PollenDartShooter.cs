using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// Fires pooled pollen darts upward on a cooldown, with the Fire animation
/// driven by an "is firing" bool that's held for a full clip per shot.
public class PollenDartShooter : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference fireAction; // Player/Attack

    [Header("Prefab & Pool")]
    [SerializeField] private PollenDart dartPrefab;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private Transform muzzle;        // spawn point, just above Bumble

    [Header("Fire")]
    [SerializeField] private float fireCooldown = 0.2f;     // seconds between darts (gameplay rate)
    [SerializeField] private Animator animator;
    [SerializeField] private float fireAnimLength = 0.8f;   // your Fire clip length (8 frames @ 10 fps)

    private readonly List<PollenDart> pool = new();
    private float cooldownTimer;
    private float animHoldTimer;

    // ── KEEP: builds the pool ──
    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            PollenDart dart = Instantiate(dartPrefab);
            dart.gameObject.SetActive(false);   // inactive in the pool
            pool.Add(dart);
        }
    }

    // ── KEEP: enable/disable the fire action ──
    private void OnEnable()  => fireAction.action.Enable();
    private void OnDisable() => fireAction.action.Disable();

    // ── UPDATED ──
    private void Update()
    {
        cooldownTimer -= Time.deltaTime;
        if (animHoldTimer > 0f) animHoldTimer -= Time.deltaTime;

        bool firePressed = fireAction.action.IsPressed();

        if (firePressed && cooldownTimer <= 0f)
        {
            Fire();
            cooldownTimer = fireCooldown;
            animHoldTimer = fireAnimLength;   // guarantees a full Fire cycle from this shot
        }

        // Fire pose stays while held OR while a shot's animation is still playing out
        if (animator != null)
            animator.SetBool("IsFiring", firePressed || animHoldTimer > 0f);
    }

    // ── KEEP (no animator call inside) ──
    private void Fire()
    {
        PollenDart dart = GetPooledDart();
        dart.transform.SetPositionAndRotation(muzzle.position, Quaternion.identity);
        dart.gameObject.SetActive(true);
    }

    // ── KEEP ──
    private PollenDart GetPooledDart()
    {
        foreach (var d in pool)
            if (!d.gameObject.activeInHierarchy) return d;

        // all busy → grow the pool
        PollenDart extra = Instantiate(dartPrefab);
        extra.gameObject.SetActive(false);
        pool.Add(extra);
        return extra;
    }
}