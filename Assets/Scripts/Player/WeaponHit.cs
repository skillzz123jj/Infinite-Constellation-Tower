using System;
using UnityEngine;

public class WeaponHit : MonoBehaviour
{
    public Action<Collider2D> OnHit;
    [SerializeField] bool applyForce;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            OnHit?.Invoke(collision);
            if (applyForce)
            {
                StartCoroutine(gameObject.GetComponentInParent<PlayerMovement>().ApplyForceOnHit());

            }
        }
    }
}
