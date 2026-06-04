using System.Collections.Generic;
using UnityEngine;

public enum SpawnRateChannel { None, Enemy, Orb, Island }

/// Generic pooled spawner. Activates a pooled instance on a randomized interval
/// (scaled by difficulty) and calls OnSpawned(). Pauses while the river is in a
/// narrow checkpoint stretch so nothing spawns into the grass.
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
        obj.GetComponent<ISpawnable>()?.OnSpawned();
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