using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] BoxCollider2D attackFront;
    [SerializeField] BoxCollider2D attackUp;

    public void Attack(InputAction.CallbackContext context)
    { 
        if (context.performed)
        {
            Debug.Log("attack");
           attackFront.enabled = true;
            Invoke("ResetAttacks", 0.3f); 
        }
    }

    private void ResetAttacks()
    {
        attackFront.enabled = false;
        attackUp.enabled = false;
    }
}
