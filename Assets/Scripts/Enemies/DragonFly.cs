using UnityEngine;

public enum State { 
    Vulnerable, 
    Jumping, 
    Charging, 
    Dashing 
}

public class DiveEnemy : MonoBehaviour
{
    private State currentState;

    [SerializeField] private Transform player;
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float AttackDistanceX = 3f;

    [SerializeField] private float vulnerableTime = 2.5f;
    [SerializeField] private float chargeTime = 1f;

    private Rigidbody2D rb;
    private float stateTimer;
    private float targetJumpY;
    private float defaultGravity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        EnterVulnerableState();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Vulnerable:
                rb.linearVelocity = Vector2.zero;
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    if (Mathf.Abs(player.position.x - transform.position.x) <= AttackDistanceX)
                    {
                        EnterJumpingState();
                    }
                    else
                    {
                        stateTimer = 0.1f;
                    }
                }
                break;

            case State.Jumping:
                if (transform.position.y >= targetJumpY) EnterChargingState();
                break;

            case State.Charging:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f) EnterDashingState();
                break;

            case State.Dashing:
                break;
        }
    }

    private void EnterVulnerableState()
    {
        currentState = State.Vulnerable;
        rb.gravityScale = defaultGravity;
        rb.linearVelocity = Vector2.zero;
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
        stateTimer = vulnerableTime;
    }

    private void EnterJumpingState()
    {
        currentState = State.Jumping;
        rb.gravityScale = 0f;
        targetJumpY = transform.position.y + jumpHeight;
        rb.linearVelocityY = jumpSpeed;
    }

    private void EnterChargingState()
    {
        currentState = State.Charging;
        rb.linearVelocity = Vector2.zero;
        stateTimer = chargeTime;
    }

    private void EnterDashingState()
    {
        currentState = State.Dashing;
        Vector2 dashDirection = (player.position - transform.position).normalized;
        rb.linearVelocity = dashDirection * dashSpeed;
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == State.Dashing && collision.gameObject.CompareTag("Player"))
        {
            //Call player.TakeDamage()
            EnterVulnerableState();
        }

        if (currentState == State.Dashing && collision.gameObject.CompareTag("Ground"))
        {
            EnterVulnerableState();
        }
    }
}