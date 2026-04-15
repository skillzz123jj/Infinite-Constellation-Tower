using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health;
    [SerializeField] GameObject deathParticlePrefab;

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {

            Debug.Log($"Enemy {gameObject} died");
            Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public int GetHealth()
    {
        return health;
    }
}