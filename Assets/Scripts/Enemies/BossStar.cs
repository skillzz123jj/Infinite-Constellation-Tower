using UnityEngine;

public class BossStar : MonoBehaviour
{
    [Header("Star Health Values")]
    [SerializeField] int hitsToDestroy;
    [SerializeField] int damagePerHitToBoss;

    [SerializeField] BossController boss;
        
    public void TakeDamage(bool isSpecialAttack)
    {
        // Special attack coutns as 2 hits
        int hitsTaken = isSpecialAttack ? 2 : 1;
        hitsToDestroy -= hitsTaken;

        int damage = damagePerHitToBoss * hitsTaken;
        boss.TakeBodyDamage(damage);

        if (hitsToDestroy <= 0)
        {
            BreakStar();
        }
    }

    private void BreakStar()
    {
        boss.OnStarDestroyed();

        // play breaking animation

        gameObject.SetActive(false);
    }
}
