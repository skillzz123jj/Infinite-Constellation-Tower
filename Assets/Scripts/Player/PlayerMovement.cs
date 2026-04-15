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
    [SerializeField] Animator animator;

    [SerializeField] AudioClip jumpSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //if (Gamedata.Instance.dataExists)
        //{
        //    gameObject.transform.position = Gamedata.Instance.playerPosition;

        //}
    }

    //Move and jump use the new input system and context is taken from the Inputs asset
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        scale = transform.localScale;
        animator.SetFloat("Walk", Mathf.Abs(moveInput.x));

        if (moveInput.x != 0)
        {
            scale.x = moveInput.x < 0 ? -1 : 1;
            transform.localScale = scale;
        }
    }
    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && coyoteTimeCounter > 0)
        {
            rb.linearVelocityY = 0;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.SetTrigger("Jump");
            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySfxClip(jumpSound);
            }
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
            animator.SetTrigger("Dash");
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
    [SerializeField] bool isFalling;
    private void FixedUpdate()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (rb.linearVelocity.y < -0.1f && !isGrounded)
        {
           // animator.SetBool("IsFalling", true);
            isFalling = true;

        }
        else
        {
          //  animator.SetBool("IsFalling", false);
            isFalling = false;
        }

        if (!isDashing)
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.x = moveInput.x * movementSpeed;
            rb.linearVelocity = velocity;

            ApplyGravity();
        }
    }

    public LayerMask groundLayer;
    bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 1.0f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }
}