using System.Collections.Generic;
using UnityEngine;

/// Streams fixed-height river sections downward at WorldScroll.Speed and recycles
/// them seamlessly. Deterministic: for now it streams a single Wide section type.
/// The checkpoint sequence (taper-in -> checkpoint -> taper-out) is added later.
public class RiverStreamer : MonoBehaviour
{
    [Header("Sections (all share the same width and height)")]
    [SerializeField] private GameObject wideSection;     // Section_Wide prefab

    [Header("Geometry")]
    [Tooltip("Exact height of every section, in world units. Must match the prefab.")]
    [SerializeField] private float sectionHeight = 10f;

    [Header("Fill / Recycle (world Y)")]
    [Tooltip("Keep the stack filled until its top edge reaches this Y (above the view).")]
    [SerializeField] private float fillToY = 6f;
    [Tooltip("Recycle a section once its TOP edge drops below this (below the view).")]
    [SerializeField] private float recycleBelowY = -6f;
    [Tooltip("Bottom edge of the very first section at start (just below the view bottom).")]
    [SerializeField] private float startBottomY = -6f;

    private readonly List<Transform> active = new();   // ordered lowest -> highest
    private float stackTopY;                           // world Y of the highest section's top edge

    private void Start()
    {
        if (wideSection == null)
        {
            Debug.LogError("RiverStreamer: no wide section assigned.");
            enabled = false;
            return;
        }

        stackTopY = startBottomY;
        FillUp();
    }

    private void Update()
    {
        float move = WorldScroll.Speed * Time.deltaTime;

        // Scroll the whole stack down.
        for (int i = 0; i < active.Count; i++)
            active[i].position += Vector3.down * move;
        stackTopY -= move;

        // Recycle the lowest section once its top edge is fully below the view.
        if (active.Count > 0)
        {
            Transform lowest = active[0];
            float lowestTopEdge = lowest.position.y + sectionHeight * 0.5f;
            if (lowestTopEdge < recycleBelowY)
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

    private void SpawnNextOnTop()
    {
        GameObject obj = Instantiate(wideSection, transform);
        float centerY = stackTopY + sectionHeight * 0.5f;   // bottom edge sits on the current stack top
        obj.transform.position = new Vector3(0f, centerY, 0f);

        active.Add(obj.transform);
        stackTopY += sectionHeight;
    }
}