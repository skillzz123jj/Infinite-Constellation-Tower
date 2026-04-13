using UnityEngine;
using System.Collections;

public class Miniboss : MonoBehaviour
{
    [Header("Combat Settings")]
    private Transform player;
    [SerializeField] float meleeRange = 2f;
    [SerializeField] float dashRange = 7f;
    [SerializeField] GameObject rockPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float rockSpeed = 10f;

    [Header("Melee Detection")]
    [SerializeField] Transform meleeCheckPoint; // Place this where the weapon hits
    [SerializeField] float meleeHitRadius = 1.5f;
    [SerializeField] LayerMask playerLayer;

    [Header("Cooldowns")]
    [SerializeField] float rangedCooldown = 20f;
    [SerializeField] float dashCooldown = 5f;

    private float lastRangedTime = -20f;
    private float lastDashTime = -5f;

    [Header("Movement")]
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float dashSpeed = 15f;
    [SerializeField] float dashDuration = 0.4f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isBusy = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (isBusy || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= meleeRange)
        {
            PerformMelee();
        }
        else if (distanceToPlayer <= dashRange && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(PerformDashSequence());
        }
        else if (Time.time >= lastRangedTime + rangedCooldown)
        {
            PerformRanged();
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    // --- ANIMATION TRIGGERING ---

    void MoveTowardsPlayer()
    {
        LookAtPlayer();
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * walkSpeed, rb.linearVelocity.y);
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    void PerformMelee()
    {
        isBusy = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Melee");
    }

    void PerformRanged()
    {
        isBusy = true;
        lastRangedTime = Time.time;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Ranged");
    }

    // --- PUBLIC METHODS (CALLED BY ANIMATION EVENTS) ---

    public void ExecuteMeleeDamage()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(meleeCheckPoint.position, meleeHitRadius, playerLayer);
        if (hitPlayer != null)
        {
            // Hadnt decided yet on the way to deal damage, but this could maybe call hitplayer.Playercomponent.takedamage or whatever
        }
    }


    public void LaunchRock()
    {
        if (player == null) return;

        GameObject rock = Instantiate(rockPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rockrb = rock.GetComponent<Rigidbody2D>();

        if (rockrb)
        {
            Vector2 shootDir = new Vector2(player.position.x - firePoint.position.x, 0f).normalized;
            rockrb.linearVelocity = shootDir * rockSpeed;
        }
    }

    public void ResetAction()
    {
        isBusy = false;
        anim.ResetTrigger("Melee");
        anim.ResetTrigger("Ranged");
        anim.SetFloat("Speed", 0);
    }

    // --- UTILITIES ---

    IEnumerator PerformDashSequence()
    {
        isBusy = true;
        lastDashTime = Time.time;
        rb.linearVelocity = Vector2.zero;

        anim.SetBool("IsCharging", true);
        yield return new WaitForEndOfFrame();
        float chargeLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(chargeLength);
        anim.SetBool("IsCharging", false);

        anim.SetBool("IsDashing", true);
        LookAtPlayer();
        Vector2 dashDir = (player.position - transform.position).normalized;

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            rb.linearVelocity = dashDir * dashSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("IsDashing", false);
        isBusy = false;
    }

    void LookAtPlayer()
    {
        Vector3 currentScale = transform.localScale;
        if (player.position.x > transform.position.x)
        {
            if (currentScale.x > 0) currentScale.x *= -1;
        }
        else
        {
            if (currentScale.x < 0) currentScale.x *= -1;
        }
        transform.localScale = currentScale;
    }

    // Visualize the melee hit box in the editor
    private void OnDrawGizmosSelected()
    {
        if (meleeCheckPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleeCheckPoint.position, meleeHitRadius);
    }
}