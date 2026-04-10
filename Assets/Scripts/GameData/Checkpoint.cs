using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Array[] checkpoints = new Array[0];
    GameManager gm;

    void Start()
    {
        gm = FindAnyObjectByType<GameManager>();

    }

    //Automatically saves the game when checkpoint is reached
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Gamedata.Instance.playerPosition = transform.position;
            gm.Save();
        }
    }
}
