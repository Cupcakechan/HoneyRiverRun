using System.Collections.Generic;
using UnityEngine;

public enum SpawnRateChannel { None, Enemy, Orb, Island }

/// Generic pooled spawner. Activates a pooled instance on a randomized interval
/// (scaled by difficulty) and calls OnSpawned(). Pauses while the river is narrow,
/// and avoids dropping objects on top of recent spawns (shared via SpawnTraffic).
public class Spawner : MonoBehaviour
{
    [Header("Prefab & Pool")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 12;

    [Header("Timing (seconds between spawns)")]
    [SerializeField] private float minInterval = 1.5f;
    [SerializeField] private float maxInterval = 2.5f;

    [Header("Difficulty")]
    [SerializeField] private SpawnRateChannel rateChannel = SpawnRateChannel.None;

    [Header("Checkpoint")]
    [Tooltip("Skip spawning while the river is in a narrow taper/checkpoint stretch.")]
    [SerializeField] private bool pauseDuringCheckpoint = true;

    [Header("Introduction")]

    [Tooltip("Don't spawn until this many checkpoints have been passed. 0 = from the start.")]
    [SerializeField] private int unlockAtGate = 0;

    [Header("Overlap avoidance")]
    [Tooltip("Re-roll (then skip) a spawn that lands too close to another recent spawn.")]
    [SerializeField] private bool avoidOverlap = true;
    [Tooltip("Minimum world-unit distance between spawns.")]
    [SerializeField] private float minSpawnGap = 2f;
    [Tooltip("How many positions to try before giving up this spawn.")]
    [SerializeField] private int placementTries = 4;
    [Tooltip("How long a spawn reserves its spot (seconds).")]
    [SerializeField] private float overlapHoldSeconds = 1.2f;

    private readonly List<GameObject> pool = new();
    private float timer;

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Add(obj);
        }
        timer = NextInterval();
    }

    private void Update()
{
    timer -= Time.deltaTime;
    if (timer > 0f) return;

    timer = NextInterval();   // reset every cycle so spawns never "stack up"

    // Hold this enemy type until enough checkpoints have been passed.
    if (GameManager.Instance != null && GameManager.Instance.GatesPassed < unlockAtGate) return;

    if (pauseDuringCheckpoint && RiverState.InCheckpointStretch) return;

    SpawnOne();
}

    private float NextInterval()
    {
        float baseInterval = Random.Range(minInterval, maxInterval);
        return baseInterval / Mathf.Max(0.01f, RateMultiplier());
    }

    private float RateMultiplier()
    {
        switch (rateChannel)
        {
            case SpawnRateChannel.Enemy:  return Difficulty.EnemyRateMultiplier;
            case SpawnRateChannel.Orb:    return Difficulty.OrbRateMultiplier;
            case SpawnRateChannel.Island: return Difficulty.IslandRateMultiplier;
            default:                      return 1f;
        }
    }

    private void SpawnOne()
    {
        GameObject obj = GetInactive();
        obj.SetActive(true);

        ISpawnable spawnable = obj.GetComponent<ISpawnable>();

        for (int attempt = 0; attempt < placementTries; attempt++)
        {
            spawnable?.OnSpawned();   // positions (and randomizes) the object

            if (!avoidOverlap) return;   // avoidance off → accept first placement

            Vector2 pos = obj.transform.position;
            if (SpawnTraffic.IsClear(pos, minSpawnGap))
            {
                SpawnTraffic.Register(pos, overlapHoldSeconds);
                return;
            }
        }

        obj.SetActive(false);   // no clear spot found → skip this spawn
    }

    private GameObject GetInactive()
    {
        foreach (var o in pool)
            if (!o.activeSelf) return o;

        GameObject extra = Instantiate(prefab, transform);
        extra.SetActive(false);
        pool.Add(extra);
        return extra;
    }
}