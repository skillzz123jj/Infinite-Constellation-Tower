using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    Transform player;

    [Header("Movement Settings")]
    [SerializeField] float speed = 0.5f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Transform groundCheck, wallCheck;
    [SerializeField] float groundCheckRadius, wallCheckRadius;

    [Header("Shooting Settings")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float shootInterval = 1.5f;
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] float bulletLifetime = 4f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Transform pointA, pointB;



    int direction = 1;  // 1 = right, -1 = left
    float originalScaleX;

    float shootTimer;
    Animator animator;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScaleX = transform.localScale.x;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shootTimer = shootInterval;

    }

    void FixedUpdate()
    {
        bool hasLineOfSight = Physics2D.OverlapArea(pointA.position, pointB.position, playerLayer);

        // Fire only if player is in clear line-of-sight
        if (hasLineOfSight)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.Play("Mite_Shoot");
            // Shooting logic
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                Shoot(direction);
                shootTimer = shootInterval;
            }
        }
        // Continue moving otherwise.
        else
        {
            // Check if we hit wall or ground not in front
            CheckForFlip();
            animator.Play("Mite_Walk");
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

            // Reset shoot timer
            shootTimer = shootInterval;

        }
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

    private void Shoot(float direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(direction * bulletSpeed, 0);
        rb.gravityScale = 0;

        // Destroy after lifetime (burst)
        Destroy(bullet, bulletLifetime);
    }

    private void OnDrawGizmos()
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

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Vector3 center = (pointA.position + pointB.position) / 2f;
            Vector3 size = new Vector3(
                Mathf.Abs(pointA.position.x - pointB.position.x),
                Mathf.Abs(pointA.position.y - pointB.position.y),
                0f
            );
            Gizmos.DrawWireCube(center, size);
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
