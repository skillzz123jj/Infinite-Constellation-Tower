using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] int damage;
    public WeaponHit frontAttack;
    public WeaponHit upAttack;
    public WeaponHit downAttack;

    public Image fillImage;
    public int maxValue = 100;
    public int powerBarValue = 0;
    public bool isAttacking;

    private Coroutine barRoutine;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GameObject glowingBar;
    [SerializeField] Animator BarEffect;
    
    [SerializeField] AudioClip attack;
    [SerializeField] AudioClip hit;


    private void Start()
    {
        frontAttack.OnHit += OnEnemyHit;
        upAttack.OnHit += OnEnemyHit;
        downAttack.OnHit += OnEnemyHit;
        //powerBarValue = Gamedata.Instance.playerPowerbar;
        powerBarValue = 100;
        UpdateBar();
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            
            isAttacking = true;
            Vector2 direction = playerMovement.GetMoveInput();
            if (direction.y > 0.5f)
            {
                animator.SetTrigger("UpAttack");
            }
            else if (direction.y < -0.5f && !playerMovement.IsGrounded())
            {
                animator.SetTrigger("DownAttack");
            }
            else
            {
                animator.SetTrigger("FrontAttack");
            }

            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySfxClip(attack);
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void SpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           if (powerBarValue == 100)
           {
               animator.SetTrigger("SpecialAttack");
               DrainBar(50);
           }
           else
           {
               BarEffect.SetTrigger("Shake");
           }
        }
    }

    private void OnEnemyHit(Collider2D other)
    {
        // If a boss star is hit, use star damage flow (also damages boss through BossStar).
        BossStar bossStar = other.GetComponentInParent<BossStar>();
        if (bossStar != null)
        {
            FillBar(15);
            bossStar.TakeDamage(false);
            return;
        }

        // Boss body damage is handled by BossController (boss does not use EnemyHealth).
        BossController bossController = other.GetComponent<BossController>() ?? other.GetComponentInParent<BossController>();
        if (bossController != null)
        {
            FillBar(15);
            bossController.TakeWeaponHitDamage();
            return;
        }

        // Keep regular enemy damage behavior
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            FillBar(15);
            enemyHealth.TakeDamage(damage);
            Debug.Log(enemyHealth.GetHealth());
        }

            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySfxClip(hit);
            }
    }
  
    public void FillBar(int amount)
    {
        powerBarValue += amount;
        powerBarValue = Mathf.Clamp(powerBarValue, 0, maxValue);
        UpdateBar();
    }

    public void DrainBar(int amount)
    {
        powerBarValue -= amount;
        powerBarValue = Mathf.Clamp(powerBarValue, 0, maxValue);
        UpdateBar();
    }

    public void SetBarInstant(float fill)
    {
        fillImage.fillAmount = fill;
    }

    void UpdateBar()
    {
        float fillPercent = (float)powerBarValue / maxValue;

        if (powerBarValue == 100)
        {
            glowingBar.SetActive(true);
        }
        else
        {
            glowingBar.SetActive(false);
        }

        if (barRoutine != null)
            StopCoroutine(barRoutine);

        barRoutine = StartCoroutine(AnimateBar(fillPercent, 0.3f)); 
    }

    IEnumerator AnimateBar(float targetFill, float duration)
    {
        float startFill = fillImage.fillAmount;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            fillImage.fillAmount = Mathf.Lerp(startFill, targetFill, time / duration);
            yield return null;
        }

        fillImage.fillAmount = targetFill;

    }
}

