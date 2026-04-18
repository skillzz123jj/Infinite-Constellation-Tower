using UnityEngine;
using System.Collections;

public class RangedEnemy : MonoBehaviour
{
    [Header("Target")]
    private Transform player;

    [Header("Movement Settings")]
    [SerializeField] float speed = 0.5f;

    [Header("Ground Detection")]
    [SerializeField] Transform rayCastTransform;
    float rayDistance = 0.2f;

    [Header("Shooting Settings")]
    [SerializeField] float maxAttackDistance = 10f;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float shootInterval = 1.5f;
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] float bulletLifetime = 4f;

    float shootTimer;
    Animator animator;
    Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shootTimer = shootInterval;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float facingDirection = Mathf.Sign(transform.localScale.x);

        //using negated localscale because the default model is facing x- instead of x+ as it should
        facingDirection = -facingDirection;

        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);

        bool isFacingPlayer = (directionToPlayer == facingDirection);
        bool isInRange = distanceToPlayer <= maxAttackDistance;

        // Fire only if facing the player, in range, and stop moving.
        if (isFacingPlayer && isInRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.Play("Mite_Shoot");
            // Shooting logic
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                Shoot(facingDirection);
                shootTimer = shootInterval;
            }
        }
        // Continue moving otherwise.
        else
        {
            animator.Play("Mite_Walk");
            // Movement logic
            rb.linearVelocity = new Vector2(facingDirection * speed, rb.linearVelocity.y);

            RaycastHit2D groundInfo = Physics2D.Raycast(rayCastTransform.position, Vector2.down, rayDistance);

            if (groundInfo.collider == null)
            {
                Flip();
            }
        }
    }

    private void Flip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
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
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayCastTransform.position, rayCastTransform.position + Vector3.down * rayDistance);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TakeDamage"))
        {
            collision.gameObject.GetComponentInParent<PlayerHealth>().TakeDamage((collision.transform.position - transform.position).normalized);
        }
    }
}
