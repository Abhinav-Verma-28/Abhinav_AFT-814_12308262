using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayerMask;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool facingRight = true;
    private BoxCollider2D playerCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);

        if (moveInput.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && facingRight)
        {
            Flip();
        }
    }

    private void CheckGrounded()
    {
        Vector2 raycastOrigin = new Vector2(transform.position.x, playerCollider.bounds.min.y);
        float raycastDistance = groundCheckDistance;

        if (transform.localScale.y != 1f)
        {
            raycastDistance *= transform.localScale.y;
        }

        isGrounded = Physics2D.Raycast(raycastOrigin, Vector2.down, raycastDistance, groundLayerMask);
    }

    // ✅ These functions will be called by PlayerInput (Unity Events mode)
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCollider != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;

            Vector3 raycastOrigin = new Vector3(transform.position.x, playerCollider.bounds.min.y, transform.position.z);
            float raycastDistance = groundCheckDistance;

            if (transform.localScale.y != 1f)
            {
                raycastDistance *= transform.localScale.y;
            }

            Gizmos.DrawLine(raycastOrigin, raycastOrigin + Vector3.down * raycastDistance);
        }
    }
}
