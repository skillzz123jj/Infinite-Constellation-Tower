using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] GameObject[] sunrays = new GameObject[5];
    private Coroutine currentHeal;
    [SerializeField] PlayerCombat playerCombat;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Animator animator;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] float knockbackForce = 5f;

    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameObject firstSelectedButton;

    [SerializeField] float invulnerableTime;

    bool invulnerable;


    private void Start()
    {
        Gamedata.Instance.playerHealth = 5;
        // Initialize sunrays based on current health
        int health = Mathf.Clamp(Gamedata.Instance.playerHealth, 0, sunrays.Length);

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
        if (context.started && Gamedata.Instance.playerHealth < 5 && Gamedata.Instance.playerPowerbar >= 10 && playerMovement.GetMoveInput().x == 0)
        {
            currentHeal = StartCoroutine(FillRay(2f, sunrays[Gamedata.Instance.playerHealth].GetComponent<Image>()));
            animator.SetBool("Healing", true);
        }

        if (context.canceled && Gamedata.Instance.playerHealth < 5)
        {
            if (currentHeal != null)
            {
                StopCoroutine(currentHeal);
                sunrays[Gamedata.Instance.playerHealth].GetComponent<Image>().fillAmount = 0f;
                animator.SetBool("Healing", false);

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
        Gamedata.Instance.playerHealth++;
        currentHeal = null;
        animator.SetBool("Healing", false);
    }
    public void TakeDamage(Vector2 hitDirection = default)
    {
        if (Gamedata.Instance.playerHealth > 0 && !invulnerable)
        {
            Gamedata.Instance.playerHealth--;
            sunrays[Gamedata.Instance.playerHealth].GetComponent<Image>().fillAmount = 0;
            animator.SetTrigger("Hurt");

            if (hitDirection != Vector2.zero)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);

                playerMovement.limitMovement = true;
            }

            Invoke("ResetTimeScale", 0.3f);
            Time.timeScale = 0.5f;
            invulnerable = true;
            Invoke("ResetInvulnerability", invulnerableTime);


            if (Gamedata.Instance.playerHealth <= 0)
            {
                ResetTimeScale();
                playerInput.SwitchCurrentActionMap("UI");
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
                animator.SetTrigger("Death");
                Gamedata.Instance.playerHealth = 5;
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
