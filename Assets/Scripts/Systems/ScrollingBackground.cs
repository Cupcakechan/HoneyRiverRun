using UnityEngine;

/// Moves stacked background segments downward at WorldScroll.Speed.
/// When a segment drops fully below the camera, it snaps back above the highest one.
public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] private Transform[] segments;     // the two river segments
    [SerializeField] private float segmentHeight = 12f; // must match each segment's height

    private float resetThreshold; // y at which a segment is fully off-screen (snap point)

    private void Start()
    {
        float camBottom = -Camera.main.orthographicSize;          // -5 at ortho size 5
        resetThreshold = camBottom - segmentHeight * 0.5f;        // -11
    }

    private void Update()
    {
        float move = WorldScroll.Speed * Time.deltaTime;

        foreach (var seg in segments)
        {
            seg.position += Vector3.down * move;

            if (seg.position.y <= resetThreshold)
            {
                // place this segment directly above the current highest one
                float highestY = float.MinValue;
                foreach (var other in segments)
                    if (other != seg && other.position.y > highestY)
                        highestY = other.position.y;

                seg.position = new Vector3(seg.position.x, highestY + segmentHeight, seg.position.z);
            }
        }
    }
}