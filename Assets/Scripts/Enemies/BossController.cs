using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] int currentBossHP;
    [SerializeField] int normalHitDamage;
    private bool wasStarHitThisFrame;
    private int starsDestroyed = 0;

    [Header("Attack Settings")]
    [SerializeField] List<BossAttack> attackPool;

    [Header("Battle Flow")]
    [SerializeField] float globalCooldown = 2f;
    [SerializeField] int attacksBeforePlatforms = 3;

    private int attacksPerformed = 0;
    private bool isPlatformPhase = false;

    [Header("Platform Phase Settings")]
    [SerializeField] GameObject platformGroup;
    [SerializeField] float platformPhaseDuration = 8f;
    private bool starDestroyed = false;

    [Header("Pincer References")]
    [SerializeField] Transform rightPincer;
    [SerializeField] Transform leftPincer;
    [SerializeField] BossMeleeHitbox rightPincerHitbox;
    [SerializeField] BossMeleeHitbox leftPincerHitbox;

    [Header("Sweep Attack Points")]
    [SerializeField] Transform rightAirPoint;
    [SerializeField] Transform rightGroundPoint;
    [SerializeField] Transform leftSweepPoint;
    [SerializeField] Transform leftAirPoint;
    [SerializeField] Transform leftGroundPoint;
    [SerializeField] Transform rightSweepPoint;

    [Header("Sweep Attack Speeds")]
    [SerializeField] float floatSpeed = 5f;
    [SerializeField] float slamSpeed = 20f;
    [SerializeField] float sweepSpeed = 15f;

    [Header("Projectile Attack Settings")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform[] projectileSpawnPoints; 
    [SerializeField] GameObject starFormingVFX;

    [Header("Effects")]
    [SerializeField] GameObject slamVFX;

    private Vector3 rightPincerStartPoint;
    private Vector3 leftPincerStartPoint;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rightPincerStartPoint = rightPincer.position;
        leftPincerStartPoint = leftPincer.position;

        // Kick off the infinite battle loop!
        StartCoroutine(BossBattleLoop());
    }

    // This is your "State Machine"
    private IEnumerator BossBattleLoop()
    {
        yield return new WaitForSeconds(1.5f); // Brief dramatic pause when the fight starts

        while (currentBossHP >= 0)
        {
            if (isPlatformPhase)
            {
                Debug.Log("Entering platform phase");
                yield return PlatformPhaseRoutine();
            }
            else
            {
                Debug.Log("Entering attack phgase");
                yield return PerformAttackRoutine();
            }
        }
    }

    public void TakeWeaponHitDamage()
    {
        //This is done so we can easily change the amount of damage we take from normal hits and star hits
        TakeBodyDamage(normalHitDamage);
    }

    public void TakeBodyDamage(int damageAmount)
    {
        currentBossHP -= damageAmount;

        // Update health bar UI

        if (currentBossHP <= 0)
        {
            // Trigger the defeat sequence
        }
    }

    public void OnStarDestroyed()
    {
        starsDestroyed++;
        starDestroyed = true; //Signal to stop the platform phase

        if (starsDestroyed == 2)
        {
            // currentPhase = 2
        }
        else if(starsDestroyed == 4)
        {
            // currentPhase = 3
        }
    }

    // This is called in the state machine when a new attack is needed. 
    // It sums up the total weights of all attakcs and selects a number 
    private string GetRandomAttack()
    {
        // 1. Add up all the weights in the pool
        int totalWeight = 0;
        foreach (BossAttack attack in attackPool)
        {
            totalWeight += attack.weight;
        }

        // Pick a random number between 0 and the total weight
        int randomValue = Random.Range(0, totalWeight);
        int currentWeightSum = 0;

        // 3. Find which attack that random number landed on
        foreach (BossAttack attack in attackPool)
        {
            currentWeightSum += attack.weight;
            if (randomValue < currentWeightSum)
            {
                Debug.Log($"Selected attack {attack.attackName}");
                return attack.attackName;
            }
        }

        return "Idle"; // Fallback just in case
    }

    ///////////////////////ATTACK ROUTINES///////////////////////////
    // Used for debugging

    private IEnumerator PerformAttackRoutine()
    {
        string nextAttack = GetRandomAttack(); // Get a random attack from the weighted pool

        // Play the chosen attack and WAIT for it to finish
        if (nextAttack == "PincerSwipe")
        {
            Debug.Log("Starting Pincer Swipe");
            yield return PincerSwipeAttackRoutine();
        }
        else if (nextAttack == "PincerSlam")
        {
            Debug.Log("Starting Pincer Slam");
            yield return PincerSlamAttackRoutine();
        }
        else if (nextAttack == "Projectile")
        {
            Debug.Log("Starting Projectile Attack");
            yield return ProjectileAttackRoutine();
        }

        attacksPerformed++;

        if (attacksPerformed >= attacksBeforePlatforms)
        {
            isPlatformPhase = true;
            attacksPerformed = 0;
        }

        yield return new WaitForSeconds(globalCooldown);
    }

    private IEnumerator PlatformPhaseRoutine()
    {
        // spawn platforms
        if (platformGroup != null) platformGroup.SetActive(true);

        // Boss stays idle for time being, replace with playing idle animation or whatever
        starDestroyed = false;
        float timer = 0f;

        // Loop runs until the timer hits 8 seconds OR a star is destroyed
        while (timer < platformPhaseDuration && !starDestroyed)
        {
            timer += Time.deltaTime; // Tick the timer up
            yield return null;       // Wait one frame before checking again
        }

        // Despawn Platforms
        if (platformGroup != null) platformGroup.SetActive(false);

        // 4. Return to normal attacks
        isPlatformPhase = false;

        // Brief pause before the boss immediately swings at them again!
        yield return new WaitForSeconds(1f);
    }


    private IEnumerator PincerSwipeAttackRoutine()
    {
        // --- PHASE 1: RIGHT PINCER SWIPE ---
        yield return MovePincer(rightPincer, rightGroundPoint.position, floatSpeed);

        Vector3 rightSweepTarget = new Vector3(leftSweepPoint.position.x, rightGroundPoint.position.y, 0f);
        rightPincerHitbox.EnableHitbox();
        yield return MovePincer(rightPincer, rightSweepTarget, sweepSpeed);
        rightPincerHitbox.DisableHitbox();

        yield return MovePincer(rightPincer, rightPincerStartPoint, sweepSpeed);

        // --- PHASE 2: LEFT PINCER SWIPE ---
        yield return MovePincer(leftPincer, leftGroundPoint.position, floatSpeed);

        Vector3 leftSweepTarget = new Vector3(rightSweepPoint.position.x, leftGroundPoint.position.y, 0f);
        leftPincerHitbox.EnableHitbox();
        yield return MovePincer(leftPincer, leftSweepTarget, sweepSpeed);
        leftPincerHitbox.DisableHitbox();

        // RESET
        yield return MovePincer(leftPincer, leftPincerStartPoint, sweepSpeed);
    }

    private IEnumerator PincerSlamAttackRoutine()
    {
        // --- PHASE 1: RIGHT PINCER SLAM ---
        yield return MovePincer(rightPincer, rightAirPoint.position, floatSpeed);

        yield return new WaitForSeconds(0.5f);
        rightPincerHitbox.EnableHitbox();
        yield return MovePincer(rightPincer, rightGroundPoint.position, slamSpeed);

        Instantiate(slamVFX, rightGroundPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        rightPincerHitbox.DisableHitbox();

        yield return MovePincer(rightPincer, rightPincerStartPoint, sweepSpeed);

        // --- PHASE 2: LEFT PINCER SLAM ---
        yield return MovePincer(leftPincer, leftAirPoint.position, floatSpeed);

        yield return new WaitForSeconds(0.5f);
        leftPincerHitbox.EnableHitbox();
        yield return MovePincer(leftPincer, leftGroundPoint.position, slamSpeed);

        Instantiate(slamVFX, leftGroundPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        leftPincerHitbox.DisableHitbox();

        yield return MovePincer(leftPincer, leftPincerStartPoint, sweepSpeed);
    }

    private IEnumerator ProjectileAttackRoutine()
    {
        GameObject[] spawnedProjectiles = new GameObject[projectileSpawnPoints.Length];

        for (int i = 0; i < projectileSpawnPoints.Length; i++)
        {
            spawnedProjectiles[i] = Instantiate(projectilePrefab, projectileSpawnPoints[i].position, Quaternion.identity);
            Instantiate(starFormingVFX, projectileSpawnPoints[i].position, Quaternion.identity);
        }

        yield return new WaitForSeconds(1f);

        foreach (GameObject proj in spawnedProjectiles)
        {
            if (proj != null && player != null)
            {
                // Calculate direction to player at the exact moment of firing
                Vector3 directionToPlayer = player.position - proj.transform.position;

                proj.GetComponent<StarProjectile>().FireAtPlayer(directionToPlayer);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator MovePincer(Transform pincer, Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(pincer.position, targetPosition) > 0.01f)
        {
            pincer.position = Vector3.MoveTowards(pincer.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        pincer.position = targetPosition;
    }
}

[System.Serializable]
public class BossAttack
{
    public string attackName; // e.g., "PincerSwipe", "Projectile"
    public int weight;        // Higher number = higher chance of being picked
}