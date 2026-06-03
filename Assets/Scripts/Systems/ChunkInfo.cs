using UnityEngine;
using UnityEngine.Tilemaps;

/// Tells the streamer how tall this chunk is, so chunks of different heights stack
/// seamlessly. Auto-measures from the assigned Tilemap; falls back to a manual value.
public class ChunkInfo : MonoBehaviour
{
    [Tooltip("Assign this chunk's Tilemap to measure its height automatically.")]
    [SerializeField] private Tilemap tilemap;
    [Tooltip("Used only if no Tilemap is assigned.")]
    [SerializeField] private float manualHeight = 12f;

    public float Height { get; private set; }

    private void Awake()
    {
        if (tilemap != null)
        {
            tilemap.CompressBounds();
            Height = tilemap.localBounds.size.y;   // exact painted height in world units
        }
        else
        {
            Height = manualHeight;
        }
    }
}