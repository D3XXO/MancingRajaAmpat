using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    private NPCStateManager _npcAI;

    public DialogueData npcDialogue;
    public GameObject interactButton;

    private DialogueManager _dialogueManager;
    private bool _isPlayerNear = false;

    void Start()
    {
        _dialogueManager = FindObjectOfType<DialogueManager>();
        _npcAI = GetComponent<NPCStateManager>();

        if (interactButton == null)
        {
            GameObject foundButton = GameObject.Find("InteractButton");
            if (foundButton != null) interactButton = foundButton;
        }

        if (interactButton != null) 
        {
            interactButton.SetActive(true);
            PlayerStateManager player = FindObjectOfType<PlayerStateManager>();
            if (player != null) player.SetButtonVisualState(interactButton, false);
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (interactButton != null && interactButton.activeSelf)
            {
                interactButton.SetActive(false);
            }
            return;
        }

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
            _dialogueManager.playerManager.SetButtonVisualState(interactButton, canInteract);
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

            PlayerStateManager player = other.GetComponent<PlayerStateManager>();
            if (player != null && interactButton != null) player.SetButtonVisualState(interactButton, false);
        }
    }

    void TriggerDialogue()
    {
        if (_dialogueManager != null && _dialogueManager.playerManager != null)
        {
            var player = _dialogueManager.playerManager;
            if (player.CurrentState != player.MovementState) return;
        }

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