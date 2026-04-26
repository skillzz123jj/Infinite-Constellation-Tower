using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] int currentBossHP;
    [SerializeField] int normalHitDamage;
    private bool wasStarHitThisFrame;
    private int starsDestroyed = 0;

    [Header("Pincer References")]
    [SerializeField] Transform rightPincer;
    [SerializeField] Transform leftPincer;

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

    [Header("Effects")]
    [SerializeField] GameObject slamVFX;

    private Vector3 rightPincerStartPoint;
    private Vector3 leftPincerStartPoint;

    private void Start()
    {
        rightPincerStartPoint = rightPincer.position;
        leftPincerStartPoint = leftPincer.position;

        StartPincerAttack();
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

        if(starsDestroyed == 2)
        {
            // currentPhase = 1
        }
        else if(starsDestroyed == 4)
        {
            // currentPhase = 2
        }
    }

    ///////////////////////ATTACK ROUTINES///////////////////////////
    public void StartPincerAttack()
    {
        StartCoroutine(PincerAttackRoutine());
    }

    private IEnumerator PincerAttackRoutine()
    {
        // --- PHASE 1: RIGHT PINCER ATTACK ---
        yield return MovePincer(rightPincer, rightAirPoint.position, floatSpeed);

        yield return new WaitForSeconds(0.5f);
        yield return MovePincer(rightPincer, rightGroundPoint.position, slamSpeed);

        Instantiate(slamVFX, rightGroundPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);

        Vector3 rightSweepTarget = new Vector3(leftSweepPoint.position.x, rightGroundPoint.position.y, 0f);
        yield return MovePincer(rightPincer, rightSweepTarget, sweepSpeed);

        // --- PHASE 2: LEFT PINCER ATTACK ---
        yield return MovePincer(leftPincer, leftAirPoint.position, floatSpeed);

        yield return new WaitForSeconds(0.5f);
        yield return MovePincer(leftPincer, leftGroundPoint.position, slamSpeed);

        Instantiate(slamVFX, leftGroundPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);

        Vector3 leftSweepTarget = new Vector3(rightSweepPoint.position.x, rightGroundPoint.position.y, 0f);
        yield return MovePincer(leftPincer, leftSweepTarget, sweepSpeed);

        // --- PHASE 3: RESET ---
        // Return pincers to their original resting positions on the boss body
        yield return MovePincer(rightPincer, rightPincerStartPoint, sweepSpeed);
        yield return MovePincer(leftPincer, leftPincerStartPoint, sweepSpeed);
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