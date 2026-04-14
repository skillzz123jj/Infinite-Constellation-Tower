using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] GameObject[] sunrays = new GameObject[6];
    private Coroutine currentHeal;
    [SerializeField] PlayerCombat playerCombat;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Animator animator;

    private void Start()
    {
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) // Temporary way to take damage
        {
            TakeDamage();
        }
    }

    // Starts a coroutine to heal
    // Cancels heal if player lets go of the button 
    public void Heal(InputAction.CallbackContext context)
    {
        if (context.started && Gamedata.Instance.playerHealth < 6 && Gamedata.Instance.playerPowerbar >= 10 && playerMovement.GetMoveInput().x == 0)
        {
            currentHeal = StartCoroutine(FillRay(2f, sunrays[Gamedata.Instance.playerHealth].GetComponent<Image>()));
            animator.SetBool("Healing", true);
        }

        if (context.canceled && Gamedata.Instance.playerHealth < 6)
        {
            if (currentHeal != null)
            {
                StopCoroutine(currentHeal);
                sunrays[Gamedata.Instance.playerHealth].GetComponent<Image>().fillAmount = 0f;
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

    public void TakeDamage()
    {
        if (Gamedata.Instance.playerHealth > 0)
        {
            Gamedata.Instance.playerHealth--;
            sunrays[Gamedata.Instance.playerHealth].GetComponent<Image>().fillAmount = 0;
            animator.SetTrigger("Hurt");


            if (Gamedata.Instance.playerHealth <= 0)
            {
                animator.SetTrigger("Death");
                Gamedata.Instance.playerHealth = 6;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile") || collision.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }
}
