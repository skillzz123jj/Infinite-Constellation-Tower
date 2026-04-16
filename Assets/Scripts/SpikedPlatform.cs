using UnityEngine;
using System.Collections;

public class SpikedPlatform : MonoBehaviour
{
    [SerializeField] private float upTime;
    
    public Animator animator;
    public Collider2D platCollider;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            animator.Play("Spike_Up");
        }
    }

}
