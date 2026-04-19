using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WalkingEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 2f;

    [Header("Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;

    [SerializeField] float groundCheckRadius = 0.15f;
    [SerializeField] float wallCheckRadius = 0.15f;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask enemyLayer;

    Rigidbody2D rb;

    int direction = 1;        // 1 = right, -1 = left
    float originalScaleX;     // Stores starting scale to avoid drift

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScaleX = transform.localScale.x;
    }

    void FixedUpdate()
    {
        CheckForFlip();
        Move();
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    void CheckForFlip()
    {
        // Don't flip if falling/mid-air (prevents flip on spawn)
        if (!rb.IsTouchingLayers(groundLayer)) return;

        // Is there ground ahead
        bool hasGround = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Is there a wall in front
        bool hitWall = Physics2D.OverlapCircle(
            wallCheck.position,
            wallCheckRadius,
            groundLayer
        );

        // Is there an enemy in front
        bool hitEnemy = Physics2D.OverlapCircle(
            wallCheck.position,
            wallCheckRadius,
            enemyLayer
);

        // Flip if about to fall OR hit a wall
        if (!hasGround || hitWall || hitEnemy)
        {
            Flip();
        }
    }

    void Flip()
    {
        direction *= -1;

        transform.localScale = new Vector3(
            originalScaleX * direction,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TakeDamage"))
        {
            collision.gameObject.GetComponentInParent<PlayerHealth>().TakeDamage((collision.transform.position - transform.position).normalized);
        }
    }
}