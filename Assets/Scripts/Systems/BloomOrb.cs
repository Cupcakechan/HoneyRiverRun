using UnityEngine;

/// A Bloom Orb pickup. Scrolls down with the world and refuels the honey tank when
/// Bumble flies over it. Spawned and pooled by the shared Spawner (ISpawnable).
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BloomOrb : MonoBehaviour, ISpawnable
{
    [Header("Placement")]
    [SerializeField] private float topY = 7f;     // spawn just above the view
    [SerializeField] private float despawnY = -7f; // below the view → recycle
    [SerializeField] private float minX = -4f;     // keep inside the river channel
    [SerializeField] private float maxX = 4f;

    [Header("Pickup VFX")]
    [SerializeField] private GameObject pickupBurstPrefab;   // PollenBurst one-shot

    [SerializeField] private AudioClip pickupClip;   // refuel chime
    public void OnSpawned()
    {
        transform.position = new Vector3(Random.Range(minX, maxX), topY, transform.position.z);
    }

    private void Update()
    {
        transform.position += Vector3.down * (WorldScroll.Speed * Time.deltaTime);
        if (transform.position.y <= despawnY)
            gameObject.SetActive(false);   // off-screen → back to the pool
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    HoneyTank tank = other.GetComponent<HoneyTank>();
    if (tank == null) return;

    tank.Refuel();
    AudioManager.Instance?.PlaySfx(pickupClip);   // your existing pickup SFX

    if (pickupBurstPrefab != null)
        Instantiate(pickupBurstPrefab, transform.position, Quaternion.identity);

    gameObject.SetActive(false);   // consumed → back to the pool
}
}