using TMPro;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    GameManager gm;
    [SerializeField] int checkpointNumber;
    PlayerCombat playerCombat;
    PlayerHealth playerHealth;
    [SerializeField] Animator anim;

    void Start()
    {
        gm = FindAnyObjectByType<GameManager>();
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();
    }

    //Automatically saves the game when checkpoint is reached
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            Gamedata.Instance.playerPosition = transform.position + new Vector3(0f, 1f, 0f);
            Gamedata.Instance.checkPointNum = checkpointNumber;
            Gamedata.Instance.playerHealth = playerHealth.health;
            Gamedata.Instance.playerPowerbar = playerCombat.powerBarValue;
            gm.Save();
            Gamedata.Instance.dataExists = true;
            anim.SetTrigger("Activate");

            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
}
