using System.Collections.Generic;
using UnityEngine;

/// Streams the river as a vertical queue of chunk prefabs of any height. Each chunk
/// reports its height via ChunkInfo, so tall and short chunks stack and recycle
/// seamlessly. Replaces the old fixed-height streamer.
public class ChunkStreamer : MonoBehaviour
{
    [Header("Chunks")]
    [SerializeField] private GameObject[] chunkPrefabs;   // each root needs a ChunkInfo

    [Header("Fill / Recycle (world Y)")]
    [Tooltip("Keep spawning until the stack's top edge reaches this Y (above the view).")]
    [SerializeField] private float fillToY = 12f;
    [Tooltip("Recycle a chunk once its TOP edge drops below this (below the view).")]
    [SerializeField] private float recycleBelowY = -7f;
    [Tooltip("Bottom edge of the first chunk, at/just below the view bottom.")]
    [SerializeField] private float startBottomY = -7f;

    private readonly List<ChunkInfo> active = new();   // ordered lowest → highest
    private float stackTopY;                           // world Y of the highest chunk's top edge

    private void Start()
    {
        if (chunkPrefabs == null || chunkPrefabs.Length == 0)
        {
            Debug.LogError("ChunkStreamer: no chunk prefabs assigned.");
            enabled = false;
            return;
        }

        stackTopY = startBottomY;
        FillUp();
    }

    private void Update()
    {
        float move = WorldScroll.Speed * Time.deltaTime;

        for (int i = 0; i < active.Count; i++)
            active[i].transform.position += Vector3.down * move;
        stackTopY -= move;

        // Recycle the lowest chunk once its top edge is fully below the view.
        if (active.Count > 0)
        {
            ChunkInfo lowest = active[0];
            float lowestTopEdge = lowest.transform.position.y + lowest.Height * 0.5f;
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
        GameObject prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
        GameObject obj = Instantiate(prefab, transform);     // Awake runs → ChunkInfo.Height ready
        ChunkInfo info = obj.GetComponent<ChunkInfo>();

        float centerY = stackTopY + info.Height * 0.5f;      // bottom edge sits on the current stack top
        obj.transform.position = new Vector3(0f, centerY, 0f);

        active.Add(info);
        stackTopY += info.Height;
    }
}