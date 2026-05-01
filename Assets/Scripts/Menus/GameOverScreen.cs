using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] GameObject gameoverScreen;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameObject firstSelectedButton;
    public Image fadeImage;
    public float fadeDuration = 1f;
    [SerializeField] BoxCollider2D playerCollider;
    [SerializeField] Rigidbody2D playerRigidbody;


    private void Start()
    {
       // StartCoroutine(FadeFromBlack());
    }
    public void ShowGameOverScreen()
    {
        gameoverScreen.SetActive(true);
        playerCollider.enabled = false;
        playerRigidbody.simulated = false;

        //Sets the ui button active for controllers 
        playerInput.SwitchCurrentActionMap("UI");
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    //Fades an image to black when game ends 
    public IEnumerator FadeToBlack()
    {
        float t = 0f;
        Color color = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = t / fadeDuration;
            fadeImage.color = color;
            yield return null;
        }
        ShowGameOverScreen();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        color.a = 1f;
        fadeImage.color = color;
    }
    public IEnumerator FadeFromBlack()
    {
        float t = 0f;
        Color color = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = 1f - (t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
    }
}


