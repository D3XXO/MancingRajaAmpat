using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    private NPCStateManager _npcAI;

    public DialogueData npcDialogue;
    public GameObject interactButton;

    private DialogueManager _dialogueManager;
    private EncyclopediaManager _encyclopedia;
    private bool _isPlayerNear = false;

    void Start()
    {
        _dialogueManager = FindObjectOfType<DialogueManager>();
        _npcAI = GetComponent<NPCStateManager>();

        if (_dialogueManager != null)
        {
            _encyclopedia = _dialogueManager.GetComponent<EncyclopediaManager>();
        }

        if (interactButton != null) interactButton.SetActive(false);
    }

    void Update()
    {
        if (_isPlayerNear && _dialogueManager != null && _dialogueManager.playerManager != null)
        {
            EncyclopediaManager enzy = _dialogueManager.GetComponent<EncyclopediaManager>();
            
            bool isMovingState = _dialogueManager.playerManager.CurrentState == _dialogueManager.playerManager.MovementState;
            bool isNotTalking = !_dialogueManager.dialoguePanel.activeSelf;
            
            bool enzyIsClosed = true;
            if (enzy != null && enzy.encyclopediaPanel != null)
            {
                enzyIsClosed = !enzy.encyclopediaPanel.activeSelf;
            }

            bool canInteract = isMovingState && isNotTalking && enzyIsClosed;
            
            if (interactButton.activeSelf != canInteract)
            {
                interactButton.SetActive(canInteract);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerNear = true;
            if (_npcAI != null) _npcAI.SetInteracting(true);
            
            interactButton.GetComponent<Button>().onClick.RemoveAllListeners();
            interactButton.GetComponent<Button>().onClick.AddListener(TriggerDialogue);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            _isPlayerNear = false;
            if (_npcAI != null) _npcAI.SetInteracting(false);
            if (interactButton != null) interactButton.SetActive(false);
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