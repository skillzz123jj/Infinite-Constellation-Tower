using UnityEngine;
using System.Collections;

public class CrumblingPlatform : MonoBehaviour
{
    [SerializeField] private bool breaking = false;
    [SerializeField] private bool broken = false;
    
    private float cdTime = 5;
    private float breakingTime = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            breaking = true; //Animation trigger
            //Wait for breaking time
            breaking = false; // Animation stop
            broken = true;
            //wait for cdTime
            broken = false; // quick less than 1 sec rebuild animation
            

        }
    }
    private void Cooldown(){

    }
}
