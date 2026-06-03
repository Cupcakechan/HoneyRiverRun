using System.Collections;
using UnityEngine;

public enum WaspMode { SouthDive, DriftEast, DriftWest, Static }

/// One enemy Wasp. Configures itself on spawn (variant + position + animation),
/// moves per its variant while the world scroll carries it down, kills Bumble on
/// contact, and — when struck by a dart — flashes Hurt then returns to the pool.
public class Wasp : MonoBehaviour, ISpawnable, IDamageable
{
    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private float hurtAnimLength = 0.9f;   // 9 frames @ 10 fps

    [Header("Movement")]
    [SerializeField] private float diveBonusSpeed = 2.5f;   // extra downward for SouthDive
    [SerializeField] private float driftSpeed = 2.5f;       // horizontal speed for E/W

    [Header("Spawn placement")]
    [SerializeField] private float topY = 6.5f;             // above the view (south/static)
    [SerializeField] private float channelMinX = -4f;       // top-spawn x range
    [SerializeField] private float channelMaxX = 4f;
    [SerializeField] private float sideX = 5f;              // side-spawn x (channel edge)
    [SerializeField] private float sideMinY = 0f;           // side-spawn y band
    [SerializeField] private float sideMaxY = 4f;

    [Header("Despawn bounds")]
    [SerializeField] private float despawnY = -7f;
    [SerializeField] private float despawnX = 6f;

    [Header("Spawn weights (relative)")]
    [SerializeField] private float weightSouth  = 0.4f;
    [SerializeField] private float weightEast   = 0.2f;
    [SerializeField] private float weightWest   = 0.2f;
    [SerializeField] private float weightStatic = 0.2f;

    [Header("Scoring")]
    [SerializeField] private int scoreValue = 40;   // GDD §11: Wasp = 40   

    private WaspMode mode;
    private bool isHit;

    // ── ISpawnable: called by the Spawner right after activation ──
    public void OnSpawned()
    {
        isHit = false;
        mode = RollMode();
        PlaceForMode();
        PlayFlyAnim();
    }

    private void Update()
    {
        if (isHit) return;

        Vector3 p = transform.position;
        p.y -= WorldScroll.Speed * Time.deltaTime;   // carried down by the river

        switch (mode)
        {
            case WaspMode.SouthDive: p.y -= diveBonusSpeed * Time.deltaTime; break;
            case WaspMode.DriftEast: p.x += driftSpeed * Time.deltaTime;     break;
            case WaspMode.DriftWest: p.x -= driftSpeed * Time.deltaTime;     break;
            case WaspMode.Static:                                            break;
        }

        transform.position = p;

        if (p.y < despawnY || p.x < -despawnX || p.x > despawnX)
            gameObject.SetActive(false);   // off-screen → back to the pool
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isHit) return;
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null) player.Die();   // contact kills Bumble
    }

    /// Called by a Pollen Dart that strikes this Wasp.
   /// Called by a Pollen Dart that strikes this Wasp.
    public void TakeHit()
    {
        if (isHit) return;
        isHit = true;                        // harmless from this instant

        ScoreManager.Instance?.AddScore(scoreValue);   // award points for the kill

        if (animator) animator.Play("Hurt");
        StartCoroutine(HurtThenDespawn());
    }

    private IEnumerator HurtThenDespawn()
    {
        yield return new WaitForSeconds(hurtAnimLength);
        gameObject.SetActive(false);         // back to the pool
    }

    private WaspMode RollMode()
    {
        float total = weightSouth + weightEast + weightWest + weightStatic;
        float r = Random.value * total;
        if ((r -= weightSouth) < 0f) return WaspMode.SouthDive;
        if ((r -= weightEast)  < 0f) return WaspMode.DriftEast;
        if ((r -= weightWest)  < 0f) return WaspMode.DriftWest;
        return WaspMode.Static;
    }

    private void PlaceForMode()
    {
        switch (mode)
        {
            case WaspMode.SouthDive:
            case WaspMode.Static:
                transform.position = new Vector3(Random.Range(channelMinX, channelMaxX), topY, 0f);
                break;
            case WaspMode.DriftEast:   // enters LEFT edge, flies east toward center
                transform.position = new Vector3(-sideX, Random.Range(sideMinY, sideMaxY), 0f);
                break;
            case WaspMode.DriftWest:   // enters RIGHT edge, flies west toward center
                transform.position = new Vector3(sideX, Random.Range(sideMinY, sideMaxY), 0f);
                break;
        }
    }

    private void PlayFlyAnim()
    {
        if (animator == null) return;
        string state = mode switch
        {
            WaspMode.DriftEast => "FlyEast",
            WaspMode.DriftWest => "FlyWest",
            _ => "FlySouth"   // SouthDive + Static face the player
        };
        animator.Play(state);
    }
}