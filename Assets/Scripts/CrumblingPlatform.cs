using UnityEngine;
using System.Collections;
using System;

public class CrumblingPlatform : MonoBehaviour
{
    [SerializeField] float respawnTime;
    [SerializeField] float animTime;
    
    public Animator animator;
    public Collider2D platCollider;
    public SpriteRenderer spriteRend;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine("Crumble");
        }
    }

    IEnumerator Crumble()
    {
        //Animation here
        yield return new WaitForSeconds(animTime);
        Components(false);

        yield return new WaitForSeconds(respawnTime);
        //Animation here
        Components(true);
    }

    private void Components(bool state)
    {
        spriteRend.enabled = state;
        platCollider.enabled = state;
    }
}
