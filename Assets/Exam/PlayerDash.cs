using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Dash Collision Settings")]
    public float dashHitForce = 20f;
    public Vector2 dashHitAngle = new Vector2(1f, 0.5f);

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private Vector2 dashDirection;
    private bool canDash = true;
    private bool isDashing = false;
    private float lastMoveDirection = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            lastMoveDirection = Input.GetAxisRaw("Horizontal");
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        dashDirection = new Vector2(lastMoveDirection, 0).normalized;

        if (playerMovement != null)
            playerMovement.enabled = false;

        rb.velocity = Vector2.zero;
        rb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = new Vector2(rb.velocity.x * 0.5f, rb.velocity.y);

        if (playerMovement != null)
            playerMovement.enabled = true;

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDashing) return;

        Rigidbody2D otherRb = collision.collider.attachedRigidbody;

        if (otherRb != null && !otherRb.isKinematic && otherRb != rb)
        {
            Vector2 forceDir = new Vector2(dashDirection.x, dashHitAngle.y).normalized;
            otherRb.AddForce(forceDir * dashHitForce, ForceMode2D.Impulse);
        }
    }

    public bool IsDashing() => isDashing;
    public bool CanDash() => canDash;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = canDash ? Color.cyan : Color.gray;
        Vector3 dashDir = new Vector3(lastMoveDirection, 0, 0);
        Gizmos.DrawRay(transform.position, dashDir * (dashForce * 0.1f));
    }
}
