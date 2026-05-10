using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class BossDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueSequence
    {
        [TextArea(3, 10)]
        public string[] texts;
    }

    [Header("UI Elements")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue Sets")]
    [SerializeField] private DialogueSequence[] preFightDialogue;
    [SerializeField] private DialogueSequence[] postFightDialogue;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    private int currentDialogueIndex = 0;
    private int currentTextIndex = 0;
    private bool isTyping = false;
    private bool isDialogueActive = false;
    private DialogueSequence[] currentSequence;

    private Coroutine typingCoroutine;

    private void Start()
    {
         dialogueCanvas.SetActive(false);
    }

    public void TriggerPreFightDialogue()
    {
        if(preFightDialogue.Length > 0 && !isDialogueActive)
        {
            StartDialogue(preFightDialogue);
        }
    }

    public void TriggerPostFightDialogue()
    {
         if(postFightDialogue.Length > 0 && !isDialogueActive)
        {
            StartDialogue(postFightDialogue);
        }
    }

    private void StartDialogue(DialogueSequence[] sequence)
    {
        isDialogueActive = true;
        currentSequence = sequence;
        currentDialogueIndex = 0;
        currentTextIndex = 0;
        
        dialogueCanvas.SetActive(true);
        gameObject.SetActive(true); // Ensure object is active so coroutines run
        DisplayNextText();
    }
    
    // Call this from Input System Jump event if the player hits Jump
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && isDialogueActive)
        {
            if (isTyping)
            {
               // Finish typing immediately
               if(typingCoroutine != null)
               {
                   StopCoroutine(typingCoroutine);
               }

               if(currentSequence != null && currentDialogueIndex < currentSequence.Length)
               {
                   if(currentTextIndex > 0)
                   {
                      dialogueText.text = currentSequence[currentDialogueIndex].texts[currentTextIndex - 1]; 
                   }

               }

               isTyping = false;
            }
            else
            {
                DisplayNextText();
            }
        }
    }


    private void DisplayNextText()
    {
        if (currentSequence == null || currentDialogueIndex >= currentSequence.Length)
        {
            EndDialogue();
            return;
        }

        if (currentTextIndex < currentSequence[currentDialogueIndex].texts.Length)
        {
             if(typingCoroutine != null)
               {
                   StopCoroutine(typingCoroutine);
               }
            typingCoroutine = StartCoroutine(TypeText(currentSequence[currentDialogueIndex].texts[currentTextIndex]));
            currentTextIndex++;
        }
        else
        {
            currentDialogueIndex++;
            currentTextIndex = 0;
            DisplayNextText();
        }
    }

    private IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in textToType.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialogueCanvas.SetActive(false);

        // Notify boss controller here if needed
    }

    public bool IsDialogueActive()
    {
         return isDialogueActive;
    }
}