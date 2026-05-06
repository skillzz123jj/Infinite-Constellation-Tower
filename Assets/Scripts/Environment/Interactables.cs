using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Interactables : MonoBehaviour
{
    [SerializeField] private GameObject interactableImage;
    [SerializeField] private GameObject interactableInstruction;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private GameObject controllerUI;
    [SerializeField] private GameObject keyboardUI;
    [SerializeField] private GameObject exitButton;
    bool isPlayerInRange;
    bool isOpen;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactableInstruction.SetActive(true);
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactableInstruction.SetActive(false);
            isPlayerInRange = false;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed && isPlayerInRange && !isOpen)
        {
            StartCoroutine(SwitchToUI());
        }
    }

    IEnumerator SwitchToUI()
    {
        interactableImage.SetActive(true);

        playerInput.SwitchCurrentActionMap("UI");

        isOpen = true;

        yield return null; 

        EventSystem.current.SetSelectedGameObject(exitButton);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitInteractable()
    {
        interactableImage.SetActive(false);

        isOpen = false;

        StartCoroutine(SwitchBackToPlayer());
    }

    IEnumerator SwitchBackToPlayer()
    {
        EventSystem.current.SetSelectedGameObject(null);


        yield return null; 

        playerInput.SwitchCurrentActionMap("Player");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
