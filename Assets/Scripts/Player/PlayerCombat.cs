using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] BoxCollider2D attackFront;
    [SerializeField] BoxCollider2D attackUp;
    [SerializeField] Animator animator;
    public WeaponHit weaponHitEnemy;

    public Image fillImage;
    public int maxValue = 100;
    private int currentValue = 0;

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
            FillBar(5);
            Destroy(enemy);
        }
    }

    public void FillBar(int amount)
    {
        currentValue += amount;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        UpdateBar();
    }

    public void DrainBar(int amount)
    {
        currentValue = 0;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        UpdateBar();
    }

    void UpdateBar()
    {
        float fillPercent = (float)currentValue / maxValue;
        fillImage.fillAmount = fillPercent;
    }
}

