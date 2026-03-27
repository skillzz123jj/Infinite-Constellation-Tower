using UnityEngine;

public enum RangedState { 
    Idle, 
    Preparing,
    Shooting,
    Cooldown
}

public class RangedEnemy : MonoBehaviour
{
    private RangedState currentState;

    [SerializeField] private Transform player;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePointHigh;
    [SerializeField] Transform firePointLow;
    [SerializeField] float attackDistanceX = 10f;
    [SerializeField] float prepareTime = 1f;
    [SerializeField] float cooldownTime = 2f;
    [SerializeField] bool alternateShotHeight;

    Rigidbody2D rb;
    float stateTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        EnterIdleState();
    }

    private void Update()
    {
        switch (currentState)
        {
            case RangedState.Idle:
                if (Mathf.Abs(player.position.x - transform.position.x) <= attackDistanceX)
                {
                    EnterPreparingState();
                }
                break;

            case RangedState.Preparing:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    EnterShootingState();
                }
                break;

            case RangedState.Shooting:
                EnterCooldownState();
                break;

            case RangedState.Cooldown:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    EnterIdleState();
                }
                break;
        }
    }

    private void EnterIdleState()
    {
        currentState = RangedState.Idle;
        rb.linearVelocity = Vector2.zero;
    }

    private void EnterPreparingState()
    {
        currentState = RangedState.Preparing;
        stateTimer = prepareTime;
    }

    private void EnterShootingState()
    {
        currentState = RangedState.Shooting;
        
        Transform selectedFirePoint = firePointLow;
            
        if (alternateShotHeight && selectedFirePoint == firePointLow) 
        {
            selectedFirePoint = firePointHigh;
        }
        else if (alternateShotHeight && selectedFirePoint == firePointHigh)
        {
            selectedFirePoint = firePointLow;
        }

        Instantiate(bulletPrefab, selectedFirePoint.position, selectedFirePoint.rotation);
    }

    private void EnterCooldownState()
    {
        currentState = RangedState.Cooldown;
        stateTimer = cooldownTime;
    }
}
