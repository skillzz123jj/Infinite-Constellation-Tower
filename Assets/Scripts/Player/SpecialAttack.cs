using UnityEngine;

public class SpecialAttack : MonoBehaviour
{
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float damagePerSecond = 10f;

    public Transform laserFirePoint;
    public LineRenderer lineRenderer;

    [SerializeField] PlayerMovement playerMovement;

    [SerializeField] bool isFiring;

    void Update()
    {
        if (isFiring)
        {
            ShootBeam();
        }
        else
        {
            StopBeam();
        }
    }

    public void StopBeam()
    {
        isFiring = false;
        lineRenderer.enabled = false;
    }

    void ShootBeam()
    {
        Vector2 direction = playerMovement.isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(
            laserFirePoint.position,
            direction,
            maxDistance
        );

        Vector2 endPoint;

        isFiring = true;
        lineRenderer.enabled = true;

        if (hit.collider != null)
        {
            endPoint = hit.point;

            var enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(1); // Temporarly set to 1 damage
                // enemy.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }
        else
        {
            endPoint = (Vector2)laserFirePoint.position +
           direction * maxDistance;
        }

        DrawBeam(laserFirePoint.position, endPoint);
    }

    void DrawBeam(Vector2 start, Vector2 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}
