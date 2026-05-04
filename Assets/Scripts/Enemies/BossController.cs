using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] int currentBossHP;
    [SerializeField] int normalHitDamage;
    private bool wasStarHitThisFrame;
    private int starsDestroyed = 0;

    private int maxBossHP;
    private int currentPhase = 1;
    private bool pendingPhaseTransition = false;

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

    [Header("Tsunami Settings")]
    [SerializeField] GameObject tsunamiPrefab;
    [SerializeField] Transform tsunamiStartPoint;
    [SerializeField] Transform tsunamiEndPoint;
    [SerializeField] float tsunamiSpeed = 12f;

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

    [Header("Animations")]
    [SerializeField] Animator animator;

    [Header("Effects")]
    [SerializeField] CinemachineVirtualCamera camera;
    [SerializeField] AnimationCurve shakeCurve;
    [SerializeField] GameObject slamVFX;
    [SerializeField] float shakeDuration;
    [SerializeField] AudioClip bossRoar;

    [Header("Debug")]
    [SerializeField] TextMeshProUGUI bossHPText;

    private Vector3 rightPincerStartPoint;
    private Vector3 leftPincerStartPoint;

    private Transform player;

    private void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").transform;

        rightPincerStartPoint = rightPincer.position;
        leftPincerStartPoint = leftPincer.position;

        maxBossHP = currentBossHP;

        bossHPText.text = $"Boss HP: {currentBossHP} / {maxBossHP}";

        StartCoroutine(BossBattleLoop());
    }

    private IEnumerator BossBattleLoop()
    {
        yield return new WaitForSeconds(1.5f); // Brief dramatic pause when the fight starts

        while (currentBossHP >= 0)
        {
            if (pendingPhaseTransition)
            {
                Debug.Log("Entering Phase Transition!");
                yield return PhaseTransitionRoutine();
            }
            else if (isPlatformPhase)
            {
                Debug.Log("Entering platform phase");
                yield return PlatformPhaseRoutine();
            }
            else
            {
                Debug.Log("Entering attack phase");
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

        bossHPText.text = $"Boss HP: {currentBossHP} / {maxBossHP}";

        float hpPercentage = (float)currentBossHP / maxBossHP;
        if (currentPhase == 1 && hpPercentage <= 0.67f)
        {
            pendingPhaseTransition = true;
        }
        else if (currentPhase == 2 && hpPercentage <= 0.34f)
        {
            pendingPhaseTransition = true;
        }

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
        animator.SetTrigger("StarBroken");

        /*// PHASE TRANSITION CHECK (Stars) CURRENTLY DISABLED
        if (currentPhase == 1 && starsDestroyed >= 2)
        {
            pendingPhaseTransition = true;
        }
        else if (currentPhase == 2 && starsDestroyed >= 4)
        {
            pendingPhaseTransition = true;
        }
        */
    }

    // This is called in the state machine when a new attack is needed. 
    // It sums up the total weights of all attakcs and selects a number 
    private string GetRandomAttack()
    {
        // 1. Add up all the weights in the pool
        int totalWeight = 0;
        foreach (BossAttack attack in attackPool)
        {
            // Ignore the Tsunami attack if we are still in Phase 1
            if (attack.attackName == "Tsunami" && currentPhase == 1) continue;

            totalWeight += attack.weight;
        }

        // Pick a random number between 0 and the total weight
        int randomValue = Random.Range(0, totalWeight);
        int currentWeightSum = 0;

        // 3. Find which attack that random number landed on
        foreach (BossAttack attack in attackPool)
        {
            // Ignore the Tsunami attack if we are still in Phase 1
            if (attack.attackName == "Tsunami" && currentPhase == 1) continue;

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
        else if (nextAttack == "Tsunami")
        {
            Debug.Log("Starting Tsunami Attack");
            yield return TsunamiAttackRoutine();
        }

        attacksPerformed++;

        if (attacksPerformed >= attacksBeforePlatforms)
        {
            isPlatformPhase = true;
            attacksPerformed = 0;
        }

        yield return new WaitForSeconds(globalCooldown);
    }

    private IEnumerator PhaseTransitionRoutine()
    {
        pendingPhaseTransition = false;
        currentPhase++;

        Debug.Log("Transitioning to Phase " + currentPhase);

        attacksPerformed = 0;

        if (AudioManager.Instance)
        {
            AudioManager.Instance.PlaySfxClip(bossRoar);
        }

        yield return CameraShake();

        yield return new WaitForSeconds(2f);
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

    private IEnumerator TsunamiAttackRoutine()
    {
        platformGroup.SetActive(true);

        bool flipped = Random.value > 0.5f;
        Transform start = flipped ? tsunamiEndPoint : tsunamiStartPoint;
        Transform end = flipped ? tsunamiStartPoint : tsunamiEndPoint;

        GameObject tsunami = Instantiate(tsunamiPrefab, start.position, Quaternion.identity);
        if (flipped) tsunami.transform.localScale = Vector3.Scale(tsunami.transform.localScale, new Vector3(-1, 1, 1));

        while (Vector3.Distance(tsunami.transform.position, end.position) > 0.1f)
        {
            tsunami.transform.position = Vector3.MoveTowards(tsunami.transform.position, end.position, tsunamiSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(tsunami);
        platformGroup.SetActive(false);
    }

    private IEnumerator PincerSwipeAttackRoutine()
    {
        animator.enabled = false;
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
        animator.enabled = true;
    }

    private IEnumerator PincerSlamAttackRoutine()
    {
        animator.enabled = false;
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
        animator.enabled = true;
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

    ///////////////////////HELPER FUNCTIONS///////////////////////////

    private IEnumerator MovePincer(Transform pincer, Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(pincer.position, targetPosition) > 0.01f)
        {
            pincer.position = Vector3.MoveTowards(pincer.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        pincer.position = targetPosition;
    }

    IEnumerator CameraShake()
    {
        Transform followTarget = camera.Follow;
        camera.Follow = null;

        Vector3 startPos = camera.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;
            float strength = shakeCurve.Evaluate(elapsedTime /  shakeDuration);
            camera.transform.position = new Vector3(startPos.x + Random.insideUnitCircle.x * strength, startPos.y + Random.insideUnitCircle.y * strength, startPos.z);
            yield return null;
        }

        camera.Follow = followTarget;
    }
}

[System.Serializable]
public class BossAttack
{
    public string attackName; // e.g., "PincerSwipe", "Projectile"
    public int weight;        // Higher number = higher chance of being picked
}