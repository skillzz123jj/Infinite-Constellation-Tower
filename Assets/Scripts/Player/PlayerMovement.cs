using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 8f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] bool isGrounded;
    [SerializeField] float dashForce = 5f;
    [SerializeField] float maxFallSpeed = 20f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    [SerializeField] float dashCooldown = 1f;
    Vector3 scale;
    private bool isDashing;
    private float lastDashTime;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float gravity = -9.81f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //Move and jump use the new input system and context is taken from the Inputs asset
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        scale = transform.localScale;

        if (moveInput.x != 0)
        {
            scale.x = moveInput.x < 0 ? -1 : 1;
            transform.localScale = scale;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && coyoteTimeCounter > 0)
        {
           rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (context.canceled)
        {
            coyoteTimeCounter = 0;
        }
    }
   
    //Starts a coroutine to stop the movement caused by dash so player stops after 
    public void Dash(InputAction.CallbackContext context)
    {
        //Cooldown for the dash so it cant be spammed 
        if (context.performed && Time.time >= lastDashTime + dashCooldown)
        {
            lastDashTime = Time.time;
            StartCoroutine(DoDash());
        }
    }
    private System.Collections.IEnumerator DoDash()
    {
        isDashing = true;
        
        rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(scale.x * dashForce, 0f);

        yield return new WaitForSeconds(0.3f);

        rb.gravityScale = 1f;

        isDashing = false;
    }
    
    //Apply custom gravity for falling
    private void ApplyGravity()
    {
        float gravityMultiplier = rb.linearVelocity.y > 0 ? 1 : 1.5f;

        Vector2 velocity = rb.linearVelocity;
        velocity.y += gravity * gravityMultiplier * Time.fixedDeltaTime;

        if (velocity.y < -maxFallSpeed)
        {
            velocity.y = -maxFallSpeed;
        }

        rb.linearVelocity = velocity;
    }

    private void FixedUpdate()
    {   
        //Coyote timer allows player to jump after a delay 
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (!isDashing)
        {
            Vector2 velocity = rb.linearVelocity;

            velocity.x = moveInput.x * movementSpeed;

            rb.linearVelocity = velocity;

            ApplyGravity();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}