using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health;

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Debug.Log($"Enemy {gameObject} died");
            Destroy(gameObject);
        }
    }

    public int GetHealth()
    {
        return health;
    }
}