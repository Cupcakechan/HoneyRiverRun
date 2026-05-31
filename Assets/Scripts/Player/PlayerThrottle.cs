using UnityEngine;
using UnityEngine.InputSystem;

/// Reads vertical throttle input (W/S) and eases WorldScroll.Speed between a
/// slow and fast bound, returning to a baseline cruise when released.
public class PlayerThrottle : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction; // Player/Move — uses the Y axis (W/S)

    [Header("Speeds (world units / second)")]
    [SerializeField] private float baselineSpeed = 3f;  // neutral cruise (matches the river's start speed)
    [SerializeField] private float minSpeed = 1.5f;     // full slow  (hold S) — never fully stops
    [SerializeField] private float maxSpeed = 6f;       // full fast  (hold W)

    [Header("Feel")]
    [SerializeField] private float acceleration = 6f;   // how quickly speed eases toward the target

    private void OnEnable()
    {
        moveAction.action.Enable();
        WorldScroll.Speed = baselineSpeed; // always start a run at cruise
    }
    private void OnDisable() => moveAction.action.Disable();

    private void Update()
    {
        float input = moveAction.action.ReadValue<Vector2>().y; // -1 (S) … +1 (W)

        // map input to a target speed: neutral = baseline, up = faster, down = slower
        float target = input >= 0f
            ? Mathf.Lerp(baselineSpeed, maxSpeed, input)
            : Mathf.Lerp(baselineSpeed, minSpeed, -input);

        // ease the world's scroll speed toward that target
        WorldScroll.Speed = Mathf.MoveTowards(WorldScroll.Speed, target, acceleration * Time.deltaTime);
    }
}