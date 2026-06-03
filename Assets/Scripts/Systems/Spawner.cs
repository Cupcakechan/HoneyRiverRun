using System.Collections.Generic;
using UnityEngine;

/// Which difficulty lever this spawner's rate follows.
public enum SpawnRateChannel { None, Enemy, Orb }

/// Generic, reusable pooled spawner. Activates a pooled instance on a randomized
/// interval and calls OnSpawned(). The interval scales with difficulty based on its
/// SpawnRateChannel, so new enemy spawners inherit difficulty with no other changes.
public class Spawner : MonoBehaviour
{
    [Header("Prefab & Pool")]
    [SerializeField] private GameObject prefab;     // must implement ISpawnable
    [SerializeField] private int poolSize = 12;

    [Header("Timing (seconds between spawns)")]
    [SerializeField] private float minInterval = 1.5f;
    [SerializeField] private float maxInterval = 2.5f;

    [Header("Difficulty")]
    [SerializeField] private SpawnRateChannel rateChannel = SpawnRateChannel.None;

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

        SpawnOne();
        timer = NextInterval();
    }

    private float NextInterval()
    {
        float baseInterval = Random.Range(minInterval, maxInterval);
        return baseInterval / Mathf.Max(0.01f, RateMultiplier());   // higher multiplier = sooner
    }

    private float RateMultiplier()
    {
        switch (rateChannel)
        {
            case SpawnRateChannel.Enemy: return Difficulty.EnemyRateMultiplier;
            case SpawnRateChannel.Orb:   return Difficulty.OrbRateMultiplier;
            default:                     return 1f;
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