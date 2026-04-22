using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Pincer References")]
    public Transform rightPincer;
    public Transform leftPincer;

    [Header("Right Attack Points")]
    public Transform rightAirPoint;
    public Transform rightGroundPoint;
    public Transform leftSweepPoint;

    [Header("Left Attack Points")]
    public Transform leftAirPoint;
    public Transform leftGroundPoint;
    public Transform rightSweepPoint;

    [Header("Speeds")]
    public float floatSpeed = 5f;
    public float slamSpeed = 20f;
    public float sweepSpeed = 15f;

    [Header("Effects")]
    [SerializeField] GameObject slamVFX;

    // Call this method to start the attack
    private void Start()
    {
        StartPincerAttack();
    }

    public void StartPincerAttack()
    {
        StartCoroutine(PincerAttackRoutine());
    }

    private IEnumerator PincerAttackRoutine()
    {
        // --- PHASE 1: RIGHT PINCER ATTACK ---

        // Move to the air
        yield return MovePincer(rightPincer, rightAirPoint.position, floatSpeed);

        // Wait a moment so the player knows what's coming
        yield return new WaitForSeconds(0.5f);

        // Slam to the ground
        yield return MovePincer(rightPincer, rightGroundPoint.position, slamSpeed);

        Instantiate(slamVFX, rightGroundPoint.position, Quaternion.identity);

        // Add screen shake or slam particle effects here!
        yield return new WaitForSeconds(0.2f);

        // 3. Sweep across the screen
        Vector3 rightSweepTarget = new Vector3(leftSweepPoint.position.x, rightGroundPoint.position.y, 0);
        yield return MovePincer(rightPincer, rightSweepTarget, sweepSpeed);

        // --- PHASE 2: LEFT PINCER ATTACK ---

        // Move to the air
        yield return MovePincer(leftPincer, leftAirPoint.position, floatSpeed);

        // Wait a moment so the player knows what's coming
        yield return new WaitForSeconds(0.5f);

        // Slam to the ground
        yield return MovePincer(leftPincer, leftGroundPoint.position, slamSpeed);

        Instantiate(slamVFX, leftGroundPoint.position, Quaternion.identity);


        // Add screen shake or slam particle effects here too
        yield return new WaitForSeconds(0.2f);

        // Sweep across the screen
        Vector3 leftSweepTarget = new Vector3(rightSweepPoint.position.x, rightGroundPoint.position.y, 0);
        yield return MovePincer(leftPincer, leftSweepTarget, sweepSpeed);

        // --- PHASE 3: RESET ---
        // Return pincers to their original resting positions on the boss body

            yield return MovePincer(rightPincer, rightAirPoint.position, sweepSpeed);
        yield return MovePincer(leftPincer, leftAirPoint.position, sweepSpeed);

    }

    // A helper coroutine to smoothly move a transform from point A to point B
    private IEnumerator MovePincer(Transform pincer, Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(pincer.position, targetPosition) > 0.01f)
        {
            pincer.position = Vector3.MoveTowards(pincer.position, targetPosition, speed * Time.deltaTime);
            yield return null; // Wait until next frame
        }

        pincer.position = targetPosition; // Snap to exact position at the end
    }
}