using System.Collections.Generic;
using UnityEngine;

/// Tracks where things were recently spawned so spawners don't drop objects on top
/// of one another. Shared across all spawners (Wasps, Islands, Bloom Orbs, and any
/// future enemy). Entries expire after a short hold, by which time they've scrolled
/// clear of the spawn line.
public static class SpawnTraffic
{
    private struct Entry { public Vector2 pos; public float expiry; }
    private static readonly List<Entry> recent = new();

    /// Drop all records (call at the start of a run).
    public static void Clear() => recent.Clear();

    /// True if pos is at least minGap from every still-active recent spawn.
    public static bool IsClear(Vector2 pos, float minGap)
    {
        float now = Time.time;
        recent.RemoveAll(e => e.expiry <= now);

        float sqr = minGap * minGap;
        foreach (var e in recent)
            if ((e.pos - pos).sqrMagnitude < sqr) return false;
        return true;
    }

    /// Record a spawn so later spawns keep their distance for holdSeconds.
    public static void Register(Vector2 pos, float holdSeconds)
    {
        recent.Add(new Entry { pos = pos, expiry = Time.time + holdSeconds });
    }
}