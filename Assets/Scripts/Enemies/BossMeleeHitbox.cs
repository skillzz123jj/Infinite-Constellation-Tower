using UnityEngine;

public class BossMeleeHitbox : MonoBehaviour
{
    private Collider2D hitboxCollider;

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();
        hitboxCollider.enabled = false;
    }

    public void EnableHitbox()
    {
        hitboxCollider.enabled = true;
    }

    public void DisableHitbox()
    {
        hitboxCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryDealDamage(collision);
    }

    private void TryDealDamage(Collider2D collision)
    {
        if (!collision.CompareTag("TakeDamage")) return;

        PlayerHealth playerHealth = collision.GetComponentInParent<PlayerHealth>();

        playerHealth.TakeDamage((playerHealth.transform.position - transform.position).normalized);
    }
}
