using UnityEngine;

public class StarProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;

    [Header("Spin Settings")]
    [SerializeField] private float minSpinSpeed = 200f;
    [SerializeField] private float maxSpinSpeed = 500f;

    private Vector3 moveDirection;
    private bool isFired = false;
    private float currentSpinSpeed;

    private void Start()
    {
        currentSpinSpeed = Random.Range(minSpinSpeed, maxSpinSpeed);

        // 50% chance to spin clockwise or counter-clockwise
        if (Random.value > 0.5f)
        {
            currentSpinSpeed = -currentSpinSpeed;
        }

        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
    }

    public void FireAtPlayer(Vector3 targetDirection)
    {
        moveDirection = targetDirection.normalized;
        isFired = true;

        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        if (!isFired)
        {
            transform.Rotate(0, 0, currentSpinSpeed * Time.deltaTime);
        }
        else
        {
            // Move towards the player once fired
            transform.position += moveDirection * speed * Time.deltaTime;
            transform.Rotate(0, 0, currentSpinSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TakeDamage"))
        {
            collision.gameObject.GetComponentInParent<PlayerHealth>().TakeDamage((collision.transform.position - transform.position).normalized);
            Debug.Log($"{gameObject.name} hit the player and dealt 1 damage");
            Destroy(gameObject);
        }
    }
}