using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] GameObject gameoverScreen;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameObject firstSelectedButton;

    [SerializeField] BoxCollider2D playerCollider;
    [SerializeField] Rigidbody2D playerRigidbody;
    [SerializeField] GameObject playerUI;
    [SerializeField] GameObject pauseMenu;

    public void ShowGameOverScreen()
    {
        gameoverScreen.SetActive(true);
        playerUI.SetActive(false);
        Destroy(pauseMenu);
        playerCollider.enabled = false;
        playerRigidbody.simulated = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Sets the ui button active for controllers 
        playerInput.SwitchCurrentActionMap("UI");
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);


    }
}


