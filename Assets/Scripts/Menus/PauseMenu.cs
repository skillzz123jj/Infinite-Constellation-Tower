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
    [SerializeField] GameObject playerUI;
    [SerializeField] AudioClip hover;
    [SerializeField] AudioClip click;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
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
        playerUI.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerInput.SwitchCurrentActionMap("Player");
    }

    public void Pause()
    {
        if (pauseMenu)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            paused = true;
            playerUI.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            playerInput.SwitchCurrentActionMap("UI");
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    public void Exit(int scene)
    {
        if (pauseMenu)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            paused = false;
            SceneManager.LoadScene(scene);
        }
    
    }

    public void SwitchScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void Hover()
    {
        if (AudioManager.Instance)
            AudioManager.Instance.PlaySfxClip(hover);
    }
    public void Click()
    {
        if (AudioManager.Instance)
            AudioManager.Instance.PlaySfxClip(click);
    }
}
