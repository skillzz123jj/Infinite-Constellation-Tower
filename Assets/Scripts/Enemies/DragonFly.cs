using UnityEngine;

public enum State
{
    Vulnerable,
    Jumping,
    Charging,
    Dashing
}

public class DiveEnemy : MonoBehaviour
{
    // Runtime state
    private State currentState;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Collider2D attackHitbox;

    [Header("Movement")]
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float dashSpeed = 15f;

    [Header("Attack Conditions")]
    [SerializeField] private float AttackDistanceX = 3f;

    [Header("State Durations")]
    [SerializeField] private float vulnerableTime = 2.5f;
    [SerializeField] private float chargeTime = 1f;

    // Cached components
    private Rigidbody2D rb;
    private Collider2D bodyCollider;
    private Collider2D playerCollider;

    // Runtime values
    private float stateTimer;
    private float targetJumpY;
    private float defaultGravity;
    private bool hasHitPlayerThisDash;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        playerCollider = player.GetComponent<Collider2D>();

        defaultGravity = rb.gravityScale;

        // Attack hitbox only deals damage as a trigger during dash.
        attackHitbox.isTrigger = true;
        SetAttackHitboxActive(false);

        // Body collision with player stays ignored in all states.
        // This keeps the enemy from being pushed/stuck on the player.
        SetPlayerBodyCollisionIgnored(true);

        EnterVulnerableState();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Vulnerable:
                HandleVulnerable();
                break;

            case State.Jumping:
                if (transform.position.y >= targetJumpY)
                {
                    EnterChargingState();
                }
                break;

            case State.Charging:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    EnterDashingState();
                }
                break;

            case State.Dashing:
                // Dash movement runs until collision callback changes state.
                break;
        }
    }

    private void HandleVulnerable()
    {
        rb.linearVelocity = Vector2.zero;
        stateTimer -= Time.deltaTime;

        if (stateTimer > 0f)
        {
            return;
        }

        bool playerInRange = Mathf.Abs(player.position.x - transform.position.x) <= AttackDistanceX;
        if (playerInRange)
        {
            EnterJumpingState();
        }
        else
        {
            stateTimer = 0.1f;
        }
    }

    private void EnterVulnerableState()
    {
        currentState = State.Vulnerable;

        rb.gravityScale = defaultGravity;
        rb.linearVelocity = Vector2.zero;

        hasHitPlayerThisDash = false;
        stateTimer = vulnerableTime;

        // We disable every non-dash state so it is always reset correctly,
        // even if state entry order changes later.
        SetAttackHitboxActive(false);
        SetPlayerBodyCollisionIgnored(true);
    }

    private void EnterJumpingState()
    {
        currentState = State.Jumping;

        rb.gravityScale = 0f;
        targetJumpY = transform.position.y + jumpHeight;
        rb.linearVelocityY = jumpSpeed;

        SetAttackHitboxActive(false);
    }

    private void EnterChargingState()
    {
        currentState = State.Charging;

        rb.linearVelocity = Vector2.zero;
        stateTimer = chargeTime;

        SetAttackHitboxActive(false);
    }

    private void EnterDashingState()
    {
        currentState = State.Dashing;
        hasHitPlayerThisDash = false;

        Vector2 dashDirection = (player.position - transform.position).normalized;
        rb.linearVelocity = dashDirection * dashSpeed;

        SetPlayerBodyCollisionIgnored(true);
        SetAttackHitboxActive(true);
    }

    private void SetAttackHitboxActive(bool isActive)
    {
        attackHitbox.enabled = isActive;
    }

    private void SetPlayerBodyCollisionIgnored(bool ignored)
    {
        Physics2D.IgnoreCollision(playerCollider, bodyCollider, ignored);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Damage is trigger-based so enemy can pass through player physically.
        if (currentState == State.Dashing && !hasHitPlayerThisDash && collision.CompareTag("Player"))
        {
            hasHitPlayerThisDash = true;
            // Call player.TakeDamage()
            Debug.Log("Player hit! Insert damage logic.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ground collision ends dash and returns enemy to vulnerable state.
        if (currentState == State.Dashing && collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Hit the ground. Entering Vulnerable.");
            EnterVulnerableState();
        }
    }
}