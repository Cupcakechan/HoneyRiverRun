using UnityEngine;

/// A scrolling island hazard. Drifts down with the world and kills Bumble on contact
/// (like the banks), but is dodged rather than shot — no IDamageable. Pooled and
/// placed by the shared Spawner via ISpawnable.
public class Island : MonoBehaviour, ISpawnable
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;   // for the random flip

    [Header("Placement")]
    [SerializeField] private float topY = 7f;       // spawn just above the view
    [SerializeField] private float despawnY = -8f;  // below the view → recycle
    [SerializeField] private float minX = -3f;      // keep a passable lane on at least one side
    [SerializeField] private float maxX = 3f;

    public void OnSpawned()
    {
        transform.position = new Vector3(Random.Range(minX, maxX), topY, 0f);
        if (spriteRenderer != null) spriteRenderer.flipX = (Random.value < 0.5f);   // cheap variety
    }

    private void Update()
    {
        transform.position += Vector3.down * (WorldScroll.Speed * Time.deltaTime);
        if (transform.position.y <= despawnY)
            gameObject.SetActive(false);   // off-screen → back to the pool
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null) player.Die();   // contact kills, same as the banks
    }
}