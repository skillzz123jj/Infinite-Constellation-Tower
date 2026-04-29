using UnityEngine;

public class Tsunami : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TakeDamage"))
        {
            collision.gameObject.GetComponentInParent<PlayerHealth>().TakeDamage((collision.transform.position - transform.position).normalized);
            Debug.Log($"{gameObject.name} hit the player and dealt 1 damage");
        }
    }
}
