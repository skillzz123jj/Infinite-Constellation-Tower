using System;
using UnityEngine;

public class WeaponHit : MonoBehaviour
{
    public Action<Collider2D> OnHit;
    [SerializeField] bool applyForce;

    // Damage targets are detected by components (enemy, boss body, boss star), not by tag.
    private bool IsValidDamageTarget(Collider2D collision)
    {
        return collision.GetComponentInParent<EnemyHealth>() != null
            || collision.GetComponentInParent<BossController>() != null
            || collision.GetComponentInParent<BossStar>() != null;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsValidDamageTarget(collision))
        {
            OnHit?.Invoke(collision);
            if (collision.GetComponentInParent<BossController>() != null)
            {
                // Instantiate hit VFX for boss
                BossController boss = collision.GetComponentInParent<BossController>();
                if (boss != null && boss.bossHitVFX != null)
                {
                    var hitPoint = collision.ClosestPoint(transform.position);
                    Instantiate(boss.bossHitVFX, hitPoint, Quaternion.identity);
                }
            }
            if (applyForce)
            {
                StartCoroutine(gameObject.GetComponentInParent<PlayerMovement>().ApplyForceOnHit());
            }
        }
    }
}
