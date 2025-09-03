using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownKeyboardController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) moveInput = ctx.ReadValue<Vector2>();
        if (ctx.canceled) moveInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }
}