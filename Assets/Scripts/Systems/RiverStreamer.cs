using System.Collections.Generic;
using UnityEngine;

/// Streams fixed-height river sections downward and recycles them. Deterministic:
/// mostly Wide, with a Taper-In -> Checkpoint -> Taper-Out cycle injected on a
/// distance cadence. All sections must share the same 20x10 footprint.
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
    [Tooltip("Distance of river between checkpoint cycles.")]
    [SerializeField] private float checkpointSpacing = 500f;
    [Tooltip("Distance before the FIRST checkpoint cycle.")]
    [SerializeField] private float firstCheckpointAt = 80f;

    [Header("Fill / Recycle (world Y)")]
    [SerializeField] private float fillToY = 6f;
    [SerializeField] private float recycleBelowY = -6f;
    [SerializeField] private float startBottomY = -6f;

    private readonly List<Transform> active = new();
    private readonly Queue<GameObject> pending = new();   // upcoming non-wide sections
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

        nextCheckpointAt = firstCheckpointAt;
        stackTopY = startBottomY;
        FillUp();
    }

    private void Update()
    {
        float move = WorldScroll.Speed * Time.deltaTime;
        distanceTravelled += move;

        // Queue a checkpoint cycle when due (and not already mid-cycle).
        if (distanceTravelled >= nextCheckpointAt && pending.Count == 0)
        {
            pending.Enqueue(taperInSection);
            pending.Enqueue(checkpointSection);
            pending.Enqueue(taperOutSection);
            nextCheckpointAt += checkpointSpacing;
        }

        // Scroll the stack down.
        for (int i = 0; i < active.Count; i++)
            active[i].position += Vector3.down * move;
        stackTopY -= move;

        // Recycle the lowest once it's fully below the view.
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
        GameObject obj = Instantiate(NextPrefab(), transform);
        float centerY = stackTopY + sectionHeight * 0.5f;
        obj.transform.position = new Vector3(0f, centerY, 0f);
        active.Add(obj.transform);
        stackTopY += sectionHeight;
    }
}