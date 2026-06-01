using UnityEngine;

/// A pooled pollen dart. Flies straight up, deactivates above the screen, and
/// deactivates on striking a Wasp (telling the Wasp it's been hit).
public class PollenDart : MonoBehaviour
{
    [SerializeField] private float speed = 14f;
    [SerializeField] private float despawnY = 7f;

    private void Update()
    {
        transform.position += Vector3.up * (speed * Time.deltaTime);
        if (transform.position.y >= despawnY)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Wasp wasp = other.GetComponent<Wasp>();
        if (wasp == null) return;

        wasp.Hit();
        gameObject.SetActive(false);   // consumed → back to the pool
    }
}