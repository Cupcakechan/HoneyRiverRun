using System.Collections.Generic;
using UnityEngine;

/// Streams fixed-height river sections downward and recycles them. Mostly Wide,
/// with a Taper-In -> Checkpoint -> Taper-Out cycle on a distance cadence.
/// Publishes RiverState.InCheckpointStretch so spawners can pause when narrow.
public class RiverStreamer : MonoBehaviour
{
    [Header("Sections (all same 20x10 footprint)")]
    [SerializeField] private GameObject wideSection;
    [SerializeField] private GameObject taperInSection;
    [SerializeField] private GameObject checkpointSection;
    [SerializeField] private GameObject taperOutSection;

    [Header("Geometry")]
    [SerializeField] private float sectionHeight = 10f;

    [Header("Checkpoint cadence (world-units)")]
    [SerializeField] private float checkpointSpacing = 500f;
    [SerializeField] private float firstCheckpointAt = 80f;

    [Header("Fill / Recycle (world Y)")]
    [SerializeField] private float fillToY = 6f;
    [SerializeField] private float recycleBelowY = -6f;
    [SerializeField] private float startBottomY = -6f;

    private readonly List<Transform> active = new();
    private readonly Queue<GameObject> pending = new();
    private float stackTopY;
    private float distanceTravelled;
    private float nextCheckpointAt;

    private void Start()
    {
        if (wideSection == null || taperInSection == null ||
            checkpointSection == null || taperOutSection == null)
        {
            Debug.LogError("RiverStreamer: assign all four section prefabs.");
            enabled = false;
            return;
        }

        RiverState.InCheckpointStretch = false;
        nextCheckpointAt = firstCheckpointAt;
        stackTopY = startBottomY;
        FillUp();
    }

    private void Update()
    {
        float move = WorldScroll.Speed * Time.deltaTime;
        distanceTravelled += move;

        if (distanceTravelled >= nextCheckpointAt && pending.Count == 0)
        {
            pending.Enqueue(taperInSection);
            pending.Enqueue(checkpointSection);
            pending.Enqueue(taperOutSection);
            nextCheckpointAt += checkpointSpacing;
        }

        for (int i = 0; i < active.Count; i++)
            active[i].position += Vector3.down * move;
        stackTopY -= move;

        if (active.Count > 0)
        {
            Transform lowest = active[0];
            if (lowest.position.y + sectionHeight * 0.5f < recycleBelowY)
            {
                Destroy(lowest.gameObject);
                active.RemoveAt(0);
            }
        }

        FillUp();
    }

    private void FillUp()
    {
        while (stackTopY < fillToY)
            SpawnNextOnTop();
    }

    private GameObject NextPrefab() => pending.Count > 0 ? pending.Dequeue() : wideSection;

    private void SpawnNextOnTop()
    {
        GameObject prefab = NextPrefab();
        RiverState.InCheckpointStretch = (prefab != wideSection);   // narrow at the spawn line?

        GameObject obj = Instantiate(prefab, transform);
        float centerY = stackTopY + sectionHeight * 0.5f;
        obj.transform.position = new Vector3(0f, centerY, 0f);
        active.Add(obj.transform);
        stackTopY += sectionHeight;
    }
}