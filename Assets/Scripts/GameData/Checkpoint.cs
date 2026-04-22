using TMPro;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    GameManager gm;
    SaveIcon saveIcon;
    [SerializeField] int checkpointNumber;
    PlayerCombat playerCombat;
    PlayerHealth playerHealth;

    void Start()
    {
        gm = FindAnyObjectByType<GameManager>();
        saveIcon = FindAnyObjectByType<SaveIcon>();
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();
    }

    //Automatically saves the game when checkpoint is reached
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Gamedata.Instance.playerPosition = transform.position;
            Gamedata.Instance.checkPointNum = checkpointNumber;
            Gamedata.Instance.playerHealth = playerHealth.health;
            Gamedata.Instance.playerPowerbar = playerCombat.powerBarValue;
            gm.Save();
            Gamedata.Instance.dataExists = true;

            gameObject.GetComponent<Collider2D>().enabled = false;

            if (saveIcon)
            {
                saveIcon.GetComponent<TextMeshProUGUI>().enabled = true;
                saveIcon.StartCoroutine(saveIcon.AnimateDots());
            }
        }
    }
}
