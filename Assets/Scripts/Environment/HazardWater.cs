using UnityEngine;

public class HazardWater : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(); 
                if (Gamedata.Instance.playerHealth > 0)
                {
                    collision.transform.position = spawnPoint.position;
                }
            }
        }
    }
}
