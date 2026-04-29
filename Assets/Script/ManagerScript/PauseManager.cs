using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseGamePanel;

    [Header("Gameplay HUD to Hide")]
    public GameObject movementButtons;
    public GameObject ensiklopediaButton;
    public GameObject fishingButton;
    public GameObject interactButton;
    public GameObject pauseButton;

    private PlayerStateManager _player;

    void Start()
    {
        _player = FindObjectOfType<PlayerStateManager>();
    }

    public void PauseGame()
    {
        pauseGamePanel.SetActive(true);
        pauseButton.SetActive(false);

        Time.timeScale = 0f;

        if (movementButtons != null) movementButtons.SetActive(false);
        if (ensiklopediaButton != null) ensiklopediaButton.SetActive(false);
        if (fishingButton != null) fishingButton.SetActive(false);
        if (interactButton != null) interactButton.SetActive(false);
    }

    public void ResumeGame()
    {
        pauseGamePanel.SetActive(false);
        Time.timeScale = 1f;
        RestoreGameplayUI();
    }

    private void RestoreGameplayUI()
    {
        if (_player == null) return;

        bool isMoving = _player.CurrentState == _player.MovementState;
        
        if (movementButtons != null) movementButtons.SetActive(isMoving);
        if (ensiklopediaButton != null) ensiklopediaButton.SetActive(isMoving);

        if (fishingButton != null)
        {
            fishingButton.SetActive(isMoving && _player.IsInFishingZone);
        }

        pauseButton.SetActive(true);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}