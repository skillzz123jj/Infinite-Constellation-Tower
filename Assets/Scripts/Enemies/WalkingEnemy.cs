using UnityEngine;

public class WalkingEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float speed = 2f;

    [Header("Ground Detection")]
    [SerializeField] Transform rayCastTransform;
    float rayDistance = 0.2f;

    void Update()
    {
        // Mathf.Sign returns 1 if scale is positive (right), and -1 if negative (left)
        float facingDirection = Mathf.Sign(transform.localScale.x);

        //using negated localscale because the default model is facing x- instead of x+ as it should
        facingDirection = -facingDirection;

        transform.Translate(Vector2.right * facingDirection * speed * Time.deltaTime);

        RaycastHit2D groundInfo = Physics2D.Raycast(rayCastTransform.position, Vector2.down, rayDistance);

        if (groundInfo.collider == null)
        {
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit! Insert damage logic.");
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayCastTransform.position, rayCastTransform.position + Vector3.down * rayDistance);
    }
}