using UnityEngine;
using System.Collections;

public class RangedEnemy : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

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

    private float shootTimer;

    void Start()
    {
        shootTimer = shootInterval;
    }

    void Update()
    {
        float facingDirection = Mathf.Sign(transform.localScale.x);
        
        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);

        bool isFacingPlayer = (directionToPlayer == facingDirection);
        bool isInRange = distanceToPlayer <= maxAttackDistance;

        // Fire only if facing the player, in range, and stop moving.
        if (isFacingPlayer && isInRange)
        {
            // Shooting logic
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                Shoot();
                shootTimer = shootInterval;
            }
        }
        // Continue moving otherwise.
        else
        {
            // Movement logic
            transform.Translate(Vector2.right * facingDirection * speed * Time.deltaTime);

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

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        float facingDirection = Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(facingDirection * bulletSpeed, 0);
        rb.gravityScale = 0;

        // Destroy after lifetime (burst)
        Destroy(bullet, bulletLifetime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayCastTransform.position, rayCastTransform.position + Vector3.down * rayDistance);
    }
}
