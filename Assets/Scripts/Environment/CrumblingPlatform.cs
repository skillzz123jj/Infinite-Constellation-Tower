using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CrumblingPlatform : MonoBehaviour
{
    [SerializeField] private float respawnTime;
    [SerializeField] private float animTime;
    
    public Animator animator;
    public Collider2D platCollider;
    public BoxCollider2D collider;
    public SpriteRenderer spriteRend;
    public string anim;
    public string idle;

    [SerializeField] AudioClip crumble;

    void Start()
    {
        collider = GetComponentInChildren<BoxCollider2D> ();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine("Crumble");
        }
    }

    IEnumerator Crumble()
    {
        animator.Play(anim);
        yield return new WaitForSeconds(animTime);
        if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySfxClip(crumble);
            }
        Components(false);

        yield return new WaitForSeconds(respawnTime);
        animator.Play(idle);
        Components(true);
    }

    private void Components(bool state)
    {
        spriteRend.enabled = state;
        platCollider.enabled = state;
        collider.enabled = state;
    }
}
