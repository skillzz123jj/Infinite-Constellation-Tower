using UnityEditor.Tilemaps;
using System.Collections;
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
    State currentState;

    [Header("References")]
    Transform player;
    [SerializeField] Collider2D attackHitbox;

    [Header("Movement")]
    [SerializeField] float jumpHeight = 4f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float dashSpeed = 15f;

    [Header("Attack Conditions")]
    [SerializeField] float AttackDistanceX = 3f;

    [Header("State Durations")]
    [SerializeField] float vulnerableTime = 2.5f;
    [SerializeField] float chargeTime = 1f;
    [SerializeField] float jumpDelayAfterHit = 0.1f;

    // Cached components
    Rigidbody2D rb;
    Collider2D bodyCollider;
    Collider2D[] playerColliders;
    Animator animator;

    // Runtime values
    float stateTimer;
    float targetJumpY;
    float defaultGravity;
    bool hasHitPlayerThisDash;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        playerColliders = player.GetComponentsInChildren<Collider2D>();

        defaultGravity = rb.gravityScale;

        // Attack hitbox is configured as a trigger so it detects overlaps for damage 
        // without causing physical bouncing or blocking during the dash.
        attackHitbox.isTrigger = true;
        SetAttackHitboxActive(false);

        // We permanently ignore physical collision between the enemy's body collider and the players body collider.
        // This doesn't affect player taking damage as it doesnt ignore collsions
        // with the attackhitbox
        IgnorePlayerCollision();

        EnterVulnerableState();
    }

    void Update()
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

    void HandleVulnerable()
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

    void EnterVulnerableState()
    {
        currentState = State.Vulnerable;
        animator.Play("Dragonfly_Stun");

        rb.gravityScale = defaultGravity;
        rb.linearVelocity = Vector2.zero;

        hasHitPlayerThisDash = false;
        stateTimer = vulnerableTime;

        SetAttackHitboxActive(false);
    }

    void EnterJumpingState()
    {
        currentState = State.Jumping;
        animator.Play("Dragonfly_Jumping_up");

        rb.gravityScale = 0f;
        targetJumpY = transform.position.y + jumpHeight;
        rb.linearVelocity = new Vector2(0f, jumpSpeed);
        

        SetAttackHitboxActive(false);
    }

    void EnterChargingState()
    {
        float facingDirection = Mathf.Sign(transform.localScale.x);

        //using negated localscale because the default model is facing x- instead of x+ as it should
        facingDirection = -facingDirection;

        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);

        if (facingDirection != directionToPlayer)
        {
            Flip();
        }

        currentState = State.Charging;
        animator.Play("Dragonfly_Charging_Attack");

        rb.linearVelocity = Vector2.zero;
        stateTimer = chargeTime;

        SetAttackHitboxActive(false);
    }

    void EnterDashingState()
    {
        float facingDirection = Mathf.Sign(transform.localScale.x);

        //using negated localscale because the default model is facing x- instead of x+ as it should
        facingDirection = -facingDirection;

        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);

        if (facingDirection != directionToPlayer)
        {
            Flip();
        }
        currentState = State.Dashing;
        animator.Play("Dragonfly_Attack");
        hasHitPlayerThisDash = false;

        Vector2 dashDirection = (player.position - transform.position).normalized;
        rb.linearVelocity = dashDirection * dashSpeed;

        SetAttackHitboxActive(true);
    }

    void SetAttackHitboxActive(bool isActive)
    {
        attackHitbox.enabled = isActive;
    }

    void IgnorePlayerCollision()
    {
        // Get the player's main body collider only (not weapon colliders)
        Collider2D playerBodyCollider = player.GetComponent<Collider2D>();
        if (playerBodyCollider != null)
        {
            Physics2D.IgnoreCollision(playerBodyCollider, bodyCollider, true);
        }
    }
    private void Flip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentState == State.Dashing && !hasHitPlayerThisDash && collision.CompareTag("TakeDamage"))
        {
            hasHitPlayerThisDash = true;
            // Call player.TakeDamage()
            collision.gameObject.GetComponentInParent<PlayerHealth>().TakeDamage((collision.transform.position - transform.position).normalized);
            Debug.Log("Player hit! Insert damage logic.");
            StartCoroutine(DelayedJumpAfterHit());
        }
    }

    IEnumerator DelayedJumpAfterHit()
    {
        yield return new WaitForSeconds(jumpDelayAfterHit);
        EnterJumpingState();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == State.Dashing && collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Hit the ground. Entering Vulnerable.");
            EnterVulnerableState();
        }
    }
}