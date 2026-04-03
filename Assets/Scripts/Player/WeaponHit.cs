using System;
using UnityEngine;

public class WeaponHit : MonoBehaviour
{
    public Action<Collider2D> OnHit;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            OnHit?.Invoke(collision);
        }
    }
}
