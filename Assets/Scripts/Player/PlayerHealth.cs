using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int health = 6;
    [SerializeField] GameObject[] sunrays = new GameObject[6];
    private Coroutine currentHeal;

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
        if (context.started && health < 6)
        {
            currentHeal = StartCoroutine(FillRay(2f, sunrays[health].GetComponent<Image>()));
        }

        if (context.canceled && health < 6)
        {
            if (currentHeal != null)
            {
                StopCoroutine(currentHeal);
                sunrays[health].GetComponent<Image>().fillAmount = 0f;
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
        health++; 
    }

void TakeDamage()
    {
        if (health > 0)
        {
            health--;
            sunrays[health].GetComponent<Image>().fillAmount = 0;

            if (health <= 0)
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
