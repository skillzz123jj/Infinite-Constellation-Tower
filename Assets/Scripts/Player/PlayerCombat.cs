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

    private Coroutine barRoutine;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GameObject glowingBar;
    [SerializeField] Animator BarEffect;

    private void Start()
    {
        frontAttack.OnHit += OnEnemyHit;
        upAttack.OnHit += OnEnemyHit;
        downAttack.OnHit += OnEnemyHit;
        Gamedata.Instance.playerPowerbar = 100;
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
            }
            else if (direction.y < -0.5f)
            {
                animator.SetTrigger("DownAttack");
            }
            else
            {
                animator.SetTrigger("FrontAttack");
            }
        }
    }
    public void SpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           if (Gamedata.Instance.playerPowerbar == 100)
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
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            FillBar(50);
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

    public void SetBarInstant(float fill)
    {
        fillImage.fillAmount = fill;
    }

    void UpdateBar()
    {
        float fillPercent = (float)Gamedata.Instance.playerPowerbar / maxValue;

        if (Gamedata.Instance.playerPowerbar == 100)
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

