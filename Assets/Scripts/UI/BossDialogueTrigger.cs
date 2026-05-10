using UnityEngine;

public class BossDialogueTrigger : MonoBehaviour
{
    [SerializeField] private BossController bossController;
    [SerializeField] private BossDialogue bossDialogue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && bossController != null && bossDialogue != null)
        {
            if (!bossController.preFightDialogueTriggered)
            {
                bossController.preFightDialogueTriggered = true;
                
                // Limit player movement immediately
                PlayerMovement pMove = collision.GetComponent<PlayerMovement>();
                if (pMove != null)
                {
                    pMove.limitMovement = true;
                    pMove.rb.linearVelocity = Vector2.zero; // Stop current momentum
                }

                // Only show dialogue text if it hasn't been shown before
                if (!Gamedata.Instance.bossIntroDialogueShown)
                {
                    Gamedata.Instance.bossIntroDialogueShown = true;
                    bossDialogue.TriggerPreFightDialogue();
                }
                
                bossController.StartBossBattle();
            }
        }
    }
}