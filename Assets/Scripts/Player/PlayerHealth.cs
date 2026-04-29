using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] GameObject[] sunrays = new GameObject[5];
    private Coroutine currentHeal;
    [SerializeField] PlayerCombat playerCombat;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Animator animator;

    [SerializeField] GameObject healingVFX;
    [SerializeField] GameObject deathVFX;
    [SerializeField] GameObject damagedVFX;


    [SerializeField] Image sunface;
    [SerializeField] Sprite sunfaceNormal;
    [SerializeField] Sprite sunfaceHurt;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] float knockbackForce = 5f;

    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameObject firstSelectedButton;

     [SerializeField] AudioClip heal;

    [SerializeField] float invulnerableTime;
    public int health;
    bool invulnerable;

    private void Start()
    {
        // Initialize sunrays based on current health
        health = Mathf.Clamp(Gamedata.Instance.playerHealth, 0, sunrays.Length);

        for (int i = 0; i < sunrays.Length; i++)
        {
            var img = sunrays[i].GetComponent<Image>();
            if (img != null)
            {
                img.fillAmount = i < health ? 1f : 0f;
            }
        }
    }

    // Starts a coroutine to heal
    // Cancels heal if player lets go of the button 
    public void Heal(InputAction.CallbackContext context)
    {
        if (context.started && health < 5 && playerCombat.powerBarValue >= 10 && playerMovement.GetMoveInput().x == 0)
        {
            currentHeal = StartCoroutine(FillRay(2f, sunrays[health].GetComponent<Image>()));
            animator.SetBool("Healing", true);
            if (healingVFX)
            {
                healingVFX.GetComponent<VisualEffect>().Play();
            }
            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySfxClip(heal);
            }
        }

        if (context.canceled && health < 5)
        {
            if (currentHeal != null)
            {
                StopCoroutine(currentHeal);
                sunrays[health].GetComponent<Image>().fillAmount = 0f;
                animator.SetBool("Healing", false);
                healingVFX.GetComponent<VisualEffect>().Reinit();
            }
        }
       
    }
    // Fills in the sunray image when healing
    IEnumerator FillRay(float duration, Image ray)
    {
        float timer = 0f;
        ray.fillAmount = 0f;

        while (timer < duration)
        {
            if (playerMovement.GetMoveInput().x != 0)
            {
                ray.fillAmount = 0f;
                animator.SetBool("Healing", false);
                currentHeal = null;
                yield break; 
            }

            timer += Time.deltaTime;
            ray.fillAmount = timer / duration;

            yield return null;
        }

        playerCombat.DrainBar(10);
        ray.fillAmount = 1f;
        health++;
        currentHeal = null;
        animator.SetBool("Healing", false);
    }
    public void TakeDamage(Vector2 hitDirection = default)
    {
        if (health > 0 && !invulnerable)
        {
            health--;
            sunrays[health].GetComponent<Image>().fillAmount = 0;
            animator.SetTrigger("Hurt");
            damagedVFX.GetComponent<VisualEffect>().Play();

            if (hitDirection != Vector2.zero)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);

                playerMovement.limitMovement = true;
            }

            if (health <= 0)
            {
                ResetTimeScale();
                playerInput.SwitchCurrentActionMap("UI");
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
                animator.SetTrigger("Death");
                if (deathVFX)
                {
                    deathVFX.GetComponent<VisualEffect>().Play();
                }
            }
            else
            {
                Invoke("ResetTimeScale", 0.3f);
                Time.timeScale = 0.5f;
                sunface.sprite = sunfaceHurt;
                invulnerable = true;
                Invoke("ResetInvulnerability", invulnerableTime);
            }
        }
    }

    private void ResetInvulnerability()
    {
        invulnerable = false;
    }
    private void ResetTimeScale()
    {
        playerMovement.limitMovement = false;
        Time.timeScale = 1f;
        sunface.sprite = sunfaceNormal;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile")) //|| collision.CompareTag("Enemy"))
        {
            if (collision.CompareTag("Projectile"))
            {
                Destroy(collision.gameObject);
            }

            Vector2 direction = (transform.position - collision.transform.position).normalized;

            TakeDamage(direction);
        }
    }
}
