using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] float jumpForce = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    [SerializeField] bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //Move and jump use the new input system and context is taken from the Inputs asset
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            Debug.Log("jump");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    private void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.x = moveInput.x * speed;
        rb.linearVelocity = velocity;
    }

    //Checks if player is touching ground 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}