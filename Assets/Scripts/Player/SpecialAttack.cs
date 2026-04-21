using UnityEngine;
using UnityEngine.VFX;

public class SpecialAttack : MonoBehaviour
{
    [SerializeField] private float maxDistance = 100f;

    public Transform laserFirePoint;
    public LineRenderer lineRenderer;
    [SerializeField] float beamHeight = 1.5f;

    [SerializeField] PlayerMovement playerMovement;

    [SerializeField] bool isFiring;

    [SerializeField] private VisualEffect laserVFX;

    void Update()
    {
        if (isFiring)
        {
            ShootBeam();
            playerMovement.limitMovement = true;

        }
        else
        {
            StopBeam();
            playerMovement.limitMovement = false;
        }
    }

    public void StopBeam()
    {
        isFiring = false;
        lineRenderer.enabled = false;
        laserVFX.gameObject.SetActive(false);
    }

    void ShootBeam()
    {
        //Laser direction to match players direction
        Vector2 direction = playerMovement.isFacingRight ? Vector2.right : Vector2.left;
       
        lineRenderer.enabled = true;

        //Check for wall collision to limit beam length
        RaycastHit2D wallHit = Physics2D.Raycast(
            laserFirePoint.position,
            direction,
            maxDistance,
            LayerMask.GetMask("Ground") 
        );

        float beamLength = maxDistance;

        //If the beam hits a wall, adjust the length to end at the wall
        if (wallHit.collider != null)
        {
            beamLength = Vector2.Distance(laserFirePoint.position, wallHit.point);
        }

        Vector2 boxCenter = (Vector2)laserFirePoint.position + direction * (beamLength / 2f);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            boxCenter,
            new Vector2(beamLength, beamHeight),
            0f,
            direction,
            0f
        );

        //Deal damage to all enemies along the path
        foreach (var hit in hits)
        {
            var enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }
        }

        //Set the end point of the beam based on the calculated length
        Vector2 endPoint = (Vector2)laserFirePoint.position + direction * beamLength;
        DrawBeam(laserFirePoint.position, endPoint);

        //Set the beam size
        lineRenderer.startWidth = beamHeight;
        lineRenderer.endWidth = beamHeight;

        laserVFX.gameObject.SetActive(true);
        laserVFX.SetVector2("StartPos", laserFirePoint.position);
        laserVFX.SetVector2("EndPos", endPoint);
    }
    void DrawBeam(Vector2 start, Vector2 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}
