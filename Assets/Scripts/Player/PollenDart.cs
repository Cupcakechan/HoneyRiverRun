using UnityEngine;

/// A pooled pollen dart. Flies straight up and deactivates itself
/// (returning to the pool) once it leaves the top of the screen.
public class PollenDart : MonoBehaviour
{
    [SerializeField] private float speed = 14f;     // upward units/second
    [SerializeField] private float despawnY = 7f;   // deactivate above this Y (view top ≈ 5.6)

    private void Update()
    {
        transform.position += Vector3.up * (speed * Time.deltaTime);
        if (transform.position.y >= despawnY)
            gameObject.SetActive(false); // returns it to the pool
    }
}