using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] BoxCollider2D attackFront;
    [SerializeField] BoxCollider2D attackUp;
    [SerializeField] Animator animator;
    public WeaponHit weaponHitEnemy;

    private void Start()
    {
        weaponHitEnemy.OnHit += OnEnemyHit; 
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger("Attack");
        }
    }

    private void OnEnemyHit(Collider2D other) 
    {
        GameObject enemy = other.gameObject;
        if (enemy != null)
        {
            Destroy(enemy);
        }
    }

    private void FillBar()
    {

    }
}
