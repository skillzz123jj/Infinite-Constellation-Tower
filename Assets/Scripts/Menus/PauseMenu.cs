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
    [SerializeField] GameObject bossUI;
    [SerializeField] AudioClip hover;
    [SerializeField] AudioClip click;
    [SerializeField] InputHandler inputHandler;
    [SerializeField] GameObject controllerUI;
    [SerializeField] GameObject keyboardUI;

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
        bossUI.SetActive(true);

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
            bossUI.SetActive(false);

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
        if ((scene == 1 || scene == 0) && AudioManager.Instance) AudioManager.Instance.PlayMusic(0);  // When dying in boss scene and returning to levelblocking or mainmenu, start playing default music again
    }

    public void SelectActiveButton(GameObject firstSelectedButton)
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);

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

    private void OnEnable()
    {
        inputHandler.OnInputMethodChanged += HandleInputChange;
    }

    private void OnDisable()
    {
        inputHandler.OnInputMethodChanged -= HandleInputChange;
    }

    private void HandleInputChange(bool usingController)
    {
        if (usingController)
        {
            controllerUI.SetActive(true);
            keyboardUI.SetActive(false);
        }
        else
        {
            controllerUI.SetActive(false);
            keyboardUI.SetActive(true);
        }
    }
}
