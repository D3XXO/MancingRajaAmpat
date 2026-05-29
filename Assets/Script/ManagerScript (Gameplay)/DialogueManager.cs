using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public Text dialogueText;

    public Text nameText;
    public Image characterImage;

    public PlayerStateManager playerManager;
    public AudioClip clickButton;

    private Queue<DialogueLine> _dialogueLines = new Queue<DialogueLine>();

    public void StartDialogue(DialogueData data)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickButton);
        }

        dialoguePanel.SetActive(true);
        TogglePlayerUI(false);

        int randomIndex = Random.Range(0, data.dialogueGroups.Length);
        DialogueTopic selectedTopic = data.dialogueGroups[randomIndex];

        _dialogueLines.Clear();
        
        foreach (DialogueLine line in selectedTopic.lines)
        {
            _dialogueLines.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickButton);
        }

        if (_dialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = _dialogueLines.Dequeue();
        
        dialogueText.text = currentLine.sentence;

        if (nameText != null)
        {
            nameText.text = currentLine.characterName;
        }

        if (characterImage != null)
        {
            if (currentLine.characterSprite != null)
            {
                characterImage.sprite = currentLine.characterSprite;
                characterImage.gameObject.SetActive(true);
            }
            else
            {
                characterImage.gameObject.SetActive(false);
            }
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        TogglePlayerUI(true);

        playerManager.AddValueScore(2);
        FloatingText floatScript = playerManager.GetComponentInChildren<FloatingText>(true);
                if (floatScript != null)
                {
                    floatScript.TriggerText("+2", Color.white);
                }
    }

    private void TogglePlayerUI(bool status)
    {
        playerManager.movementButtonsParent.SetActive(status);
        playerManager.ensiklopediaButton.SetActive(status);
        playerManager.homeButton.SetActive(status);

        if (playerManager.fishingButton != null) playerManager.fishingButton.SetActive(status);
        if (playerManager.interactButton != null) playerManager.interactButton.SetActive(status);

        if (!status) playerManager.movementComponent.StopMoving();

        PauseManager pm = FindObjectOfType<PauseManager>();
        if (pm != null && pm.pauseButton != null) pm.pauseButton.SetActive(status);
    }
}