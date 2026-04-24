using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    public DialogueData npcDialogue;
    public GameObject interactButton;
    private DialogueManager _dialogueManager;

    void Start()
    {
        _dialogueManager = FindObjectOfType<DialogueManager>();
        if (interactButton != null) interactButton.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactButton.SetActive(true);
            
            interactButton.GetComponent<Button>().onClick.RemoveAllListeners();
            interactButton.GetComponent<Button>().onClick.AddListener(TriggerDialogue);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactButton != null)
            {
                interactButton.SetActive(false);
            }
        }
    }

    void TriggerDialogue()
    {
        interactButton.SetActive(false);
        _dialogueManager.StartDialogue(npcDialogue);
    }

    private void OnDestroy()
    {
        if (interactButton != null)
        {
            interactButton.SetActive(false);
        }
    }
}