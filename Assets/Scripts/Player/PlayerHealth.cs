using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] GameObject[] sunrays = new GameObject[6];
    private Coroutine currentHeal;

    private void Start()
    {
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
            Debug.Log("Player took damage");
            TakeDamage();
        }
    }

    // Starts a coroutine to heal
    // Cancels heal if player lets go of the button 
    public void Heal(InputAction.CallbackContext context)
    {
        if (context.started && Gamedata.Instance.playerHealth < 6)
        {
            currentHeal = StartCoroutine(FillRay(2f, sunrays[Gamedata.Instance.playerHealth].GetComponent<Image>()));
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
            timer += Time.deltaTime;
            ray.fillAmount = timer / duration;
            yield return null;
        }

        ray.fillAmount = 1f;
        Gamedata.Instance.playerHealth++;
    }

    void TakeDamage()
    {
        if (Gamedata.Instance.playerHealth > 0)
        {
            Gamedata.Instance.playerHealth--;
            sunrays[Gamedata.Instance.playerHealth].GetComponent<Image>().fillAmount = 0;
            Debug.Log("Player health: " + Gamedata.Instance.playerHealth);
            if (Gamedata.Instance.playerHealth <= 0)
            {
                Debug.Log("Game over");
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Add implementation 
        //if (collision.gameObject.CompareTag("Enemy"))
        //{
        //    health--;
        //}
    }
}
