using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] BoxCollider2D attackFront;
    [SerializeField] BoxCollider2D attackUp;
    [SerializeField] Animator animator;
    [SerializeField] int damage;
    public WeaponHit weaponHitEnemy;

    public Image fillImage;
    public int maxValue = 100;

    [SerializeField] PlayerMovement playerMovement;

    private void Start()
    {
        weaponHitEnemy.OnHit += OnEnemyHit;
        UpdateBar();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 direction = playerMovement.GetMoveInput();
            if (direction.y > 0.5f)
            {
                animator.SetTrigger("UpAttack");
                attackUp.enabled = true;
                attackFront.enabled = false;
            }
            else
            {
                animator.SetTrigger("FrontAttack");
                attackFront.enabled = true;
                attackUp.enabled = false;
            }
        }
    }

    private void OnEnemyHit(Collider2D other)
    {
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            FillBar(5);
            enemyHealth.TakeDamage(damage);
            Debug.Log(enemyHealth.GetHealth());
        }
    }

    public void FillBar(int amount)
    {
        Gamedata.Instance.playerPowerbar += amount;
        Gamedata.Instance.playerPowerbar = Mathf.Clamp(Gamedata.Instance.playerPowerbar, 0, maxValue);
        UpdateBar();
    }

    public void DrainBar(int amount)
    {
        Gamedata.Instance.playerPowerbar -= amount;
        Gamedata.Instance.playerPowerbar = Mathf.Clamp(Gamedata.Instance.playerPowerbar, 0, maxValue);
        UpdateBar();
    }

    void UpdateBar()
    {
        float fillPercent = (float)Gamedata.Instance.playerPowerbar / maxValue;
        fillImage.fillAmount = fillPercent;
    }
}

