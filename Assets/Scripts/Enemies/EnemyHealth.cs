using UnityEngine;
using UnityEngine.U2D.Animation;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int health;
    [SerializeField] GameObject deathParticlePrefab;
    private Animator anim;

    [SerializeField] AudioClip damaged;
    [SerializeField] AudioClip death;

    public void TakeDamage(int amount)
    {
        if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySfxClip(damaged);
            }
        health -= amount;

        if (health <= 0)
        {
            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySfxClip(damaged);
            }
            Debug.Log($"Enemy {gameObject} died");

            StartCoroutine(DestroyRoutine());
        }
    }

    private IEnumerator DestroyRoutine()
    {
        var animator = GetComponent<Animator>();
        if (animator) animator.enabled = false;

        foreach (var skin in GetComponentsInChildren<SpriteSkin>(true))
            skin.enabled = false;

        foreach (var r in GetComponentsInChildren<Renderer>(true))
            r.enabled = false;

        Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);

        // wait one frame so rendering/deformation systems can settle
        yield return null;

        Destroy(gameObject);
    }

    public int GetHealth()
    {
        return health;
    }
}