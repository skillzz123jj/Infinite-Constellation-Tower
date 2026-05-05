using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public bool IsUsingController { get; private set; }

    public event Action<bool> OnInputMethodChanged;

    private void Update()
    {
        bool newInputMethod = IsUsingController;

        if (Gamepad.current != null && Gamepad.current.allControls.Any(control => control.IsPressed()))
        {
            newInputMethod = true;
        }
        else if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame && Keyboard.current.anyKey.isPressed)
        {
            newInputMethod = false;
        }

        //Only trigger if changed
        if (newInputMethod != IsUsingController)
        {
            IsUsingController = newInputMethod;

            OnInputMethodChanged?.Invoke(IsUsingController);
        }
    }
}