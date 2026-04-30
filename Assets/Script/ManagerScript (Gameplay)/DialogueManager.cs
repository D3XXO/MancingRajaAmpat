using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public PlayerStateManager playerManager;

    private Queue<string> _sentences = new Queue<string>();

    public void StartDialogue(DialogueData data)
    {
        dialoguePanel.SetActive(true);
        TogglePlayerUI(false);

        int randomIndex = Random.Range(0, data.dialogueGroups.Length);
        DialogueTopic selectedTopic = data.dialogueGroups[randomIndex];

        _sentences.Clear();
        foreach (string sentence in selectedTopic.sentences)
        {
            _sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = _sentences.Dequeue();
        dialogueText.text = sentence;
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        TogglePlayerUI(true);
    }

    private void TogglePlayerUI(bool status)
    {
        playerManager.movementButtonsParent.SetActive(status);
        playerManager.ensiklopediaButton.SetActive(status);
        playerManager.movementComponent.enabled = status;

        PauseManager pm = FindObjectOfType<PauseManager>();
        if (pm != null && pm.pauseButton != null)
        {
            pm.pauseButton.SetActive(status);
        }
    }
}