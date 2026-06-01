using UnityEngine;

/// A Bloom Orb pickup. Scrolls down with the world and refuels Captain Bumble's
/// honey tank when she flies over it. Self-recycles to the top for Phase 7 testing;
/// Phase 8's spawner will take over lifecycle later.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BloomOrb : MonoBehaviour
{
    [Header("Recycle bounds")]
    [SerializeField] private float despawnY = -7f;   // below the view → recycle
    [SerializeField] private float respawnY = 7f;    // above the view → reappear
    [SerializeField] private float minX = -4f;       // keep inside the river channel
    [SerializeField] private float maxX = 4f;

    private void Update()
    {
        transform.position += Vector3.down * (WorldScroll.Speed * Time.deltaTime);

        if (transform.position.y <= despawnY)
            Recycle();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HoneyTank tank = other.GetComponent<HoneyTank>();
        if (tank == null) return;

        tank.Refuel();
        Recycle();   // consumed → send it back to the top
    }

    private void Recycle()
    {
        transform.position = new Vector3(Random.Range(minX, maxX), respawnY, transform.position.z);
    }
}