using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// Fires pooled pollen darts upward on a cooldown while the fire button is held.
public class PollenDartShooter : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference fireAction; // Player/Attack

    [Header("Prefab & Pool")]
    [SerializeField] private PollenDart dartPrefab;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private Transform muzzle;        // spawn point, just above Bumble

    [Header("Fire")]
    [SerializeField] private float fireCooldown = 0.25f; // seconds between shots
    [SerializeField] private Animator animator;          // optional: plays Fire anim

    private readonly List<PollenDart> pool = new();
    private float cooldownTimer;

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            PollenDart dart = Instantiate(dartPrefab);
            dart.gameObject.SetActive(false);   // inactive in the pool
            pool.Add(dart);
        }
    }

    private void OnEnable()  => fireAction.action.Enable();
    private void OnDisable() => fireAction.action.Disable();

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;
        if (fireAction.action.IsPressed() && cooldownTimer <= 0f)
        {
            Fire();
            cooldownTimer = fireCooldown;
        }
    }

    private void Fire()
    {
        PollenDart dart = GetPooledDart();
        dart.transform.SetPositionAndRotation(muzzle.position, Quaternion.identity);
        dart.gameObject.SetActive(true);

        if (animator != null) animator.SetTrigger("Fire");
    }

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