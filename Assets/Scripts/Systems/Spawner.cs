using System.Collections.Generic;
using UnityEngine;

/// Generic, reusable spawner. Pools one prefab and activates a pooled instance on a
/// randomized interval, then calls OnSpawned() so the instance configures itself.
/// Reused later for Bloom Orbs and islands — just swap the prefab.
public class Spawner : MonoBehaviour
{
    [Header("Prefab & Pool")]
    [SerializeField] private GameObject prefab;     // must have a component implementing ISpawnable
    [SerializeField] private int poolSize = 12;

    [Header("Timing (seconds between spawns)")]
    [SerializeField] private float minInterval = 1.5f;
    [SerializeField] private float maxInterval = 2.5f;

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
        timer = Random.Range(minInterval, maxInterval);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        SpawnOne();
        timer = Random.Range(minInterval, maxInterval);
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

        // all busy → grow the pool
        GameObject extra = Instantiate(prefab, transform);
        extra.SetActive(false);
        pool.Add(extra);
        return extra;
    }
}