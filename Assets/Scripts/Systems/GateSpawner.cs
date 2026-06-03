using System.Collections.Generic;
using UnityEngine;

/// Spawns a Honeycomb Gate every `sectionDistance` world-units travelled — each gate
/// is a section divider. Pools the gate prefab and activates one via ISpawnable.
public class GateSpawner : MonoBehaviour
{
    [Header("Prefab & Pool")]
    [SerializeField] private GameObject gatePrefab;     // must implement ISpawnable
    [SerializeField] private int poolSize = 2;

    [Header("Section length (world-units)")]
    [SerializeField] private float firstGateDistance = 25f;  // distance before the first gate
    [SerializeField] private float sectionDistance = 40f;    // distance between gates

    private readonly List<GameObject> pool = new();
    private float distanceTravelled;
    private float nextGateAt;

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject g = Instantiate(gatePrefab, transform);
            g.SetActive(false);
            pool.Add(g);
        }
        nextGateAt = firstGateDistance;
    }

    private void Update()
    {
        distanceTravelled += WorldScroll.Speed * Time.deltaTime;
        if (distanceTravelled < nextGateAt) return;

        SpawnGate();
        nextGateAt += sectionDistance;
    }

    private void SpawnGate()
    {
        GameObject g = GetInactive();
        g.SetActive(true);
        g.GetComponent<ISpawnable>()?.OnSpawned();
    }

    private GameObject GetInactive()
    {
        foreach (var g in pool)
            if (!g.activeSelf) return g;

        GameObject extra = Instantiate(gatePrefab, transform);
        extra.SetActive(false);
        pool.Add(extra);
        return extra;
    }
}