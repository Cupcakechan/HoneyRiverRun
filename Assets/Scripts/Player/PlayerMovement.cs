using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction; // assign Player/Move (Vector2)

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;          // horizontal units/second
    [SerializeField] private float horizontalLimit = 4.5f;  // clamp |x| inside the channel

    private Rigidbody2D rb;
    private float horizontalInput;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void OnEnable()  => moveAction.action.Enable();
    private void OnDisable() => moveAction.action.Disable();

    private void Update()
    {
        // X axis of the Move action: -1 (left) … +1 (right)
        horizontalInput = moveAction.action.ReadValue<Vector2>().x;
    }

    private void FixedUpdate()
    {
        Vector2 pos = rb.position;
        pos.x += horizontalInput * moveSpeed * Time.fixedDeltaTime;
        pos.x = Mathf.Clamp(pos.x, -horizontalLimit, horizontalLimit);
        rb.MovePosition(pos);
    }
}