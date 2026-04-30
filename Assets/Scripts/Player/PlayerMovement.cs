using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 8f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] bool isGrounded;
    [SerializeField] float dashForce = 5f;
    [SerializeField] float maxFallSpeed = 20f;
    public Rigidbody2D rb;
    private Vector2 moveInput;
    [SerializeField] float dashCooldown = 1f;
    Vector3 scale;
    private bool isDashing;
    private float lastDashTime;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float gravity = -9.81f;
    public bool isFacingRight = true;
    public bool limitMovement = false;
    [SerializeField] Animator animator;
    private bool hasJumped;
    private bool wasGrounded;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] PlayerHealth playerHealth;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.3f;

    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (Gamedata.Instance.dataExists && SceneManager.GetActiveScene().buildIndex == 1)
        {
            gameObject.transform.position = Gamedata.Instance.playerPosition;

        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        scale = transform.localScale;
        animator.SetFloat("Walk", Mathf.Abs(moveInput.x));

        if (moveInput.x != 0 && !isDashing && !limitMovement)
        {
            scale.x = moveInput.x < 0 ? -1 : 1;
            isFacingRight = moveInput.x < 0 ? false : true;
            transform.localScale = scale;
        }
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }
   
    public void Jump(InputAction.CallbackContext context)
    {
        //Prevent double jump by checking hasJumped as well as coyote time
        if (context.performed && coyoteTimeCounter > 0 && !isDashing && !hasJumped)
        {
            rb.linearVelocityY = 0;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Jump");
            hasJumped = true;

            coyoteTimeCounter = 0; 

            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySfxClip(jumpSound);
            }
        }
    }

    //Apply upward force to player once enemy was hit with down attack
    public IEnumerator ApplyForceOnHit()
    {
        limitMovement = true;
        rb.linearVelocityY = 0;

        rb.AddForce(Vector2.up* 500f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.3f);
        limitMovement = false;
    }

    //Starts a coroutine to stop the movement caused by dash so player stops after 
    public void Dash(InputAction.CallbackContext context)
    {
        //Cooldown for the dash so it cant be spammed 
        if (context.performed && Time.time >= lastDashTime + dashCooldown && !limitMovement)
        {
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Dash");
            lastDashTime = Time.time;
            StartCoroutine(DoDash());
        }
    }

    IEnumerator DoDash()
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
        bool grounded = IsGrounded();

        if (grounded && !wasGrounded)
        {
            hasJumped = false;
            coyoteTimeCounter = coyoteTime;
        }
        else if (!grounded)
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        wasGrounded = grounded;

        if (!isDashing && !limitMovement)
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.x = moveInput.x == 0 ? 0 : Mathf.Sign(moveInput.x) * movementSpeed;
            rb.linearVelocity = velocity;

            ApplyGravity();
        }
    }

    public bool IsGrounded()
    {
        Vector2 size = new Vector2(0.17f, 0.1f);

        RaycastHit2D hit = Physics2D.BoxCast(
            groundCheck.position,
            size,
            0f,
            Vector2.down,
            0.05f,
            groundLayer
        );

        return hit.collider != null;
    }
}