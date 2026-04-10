using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] bool paused;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameObject firstSelectedButton;

    public void PauseInput(InputAction.CallbackContext context)
    {
        if (context.performed && pauseMenu)
        {
            if (!paused)
            {
                Pause();
            }
            else
            {
                Continue();
            }
        }
    }
    public void Continue()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        paused = false;

        playerInput.SwitchCurrentActionMap("Player");
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        paused = true;

        playerInput.SwitchCurrentActionMap("UI");
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    public void Exit(int scene)
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
        SceneManager.LoadScene(scene);
    }

}
