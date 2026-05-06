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
        PlayerStateManager player = FindObjectOfType<PlayerStateManager>();
        if (player != null && player.movementComponent != null)
        {
            player.movementComponent.StopMoving();
        }

        pauseGamePanel.SetActive(true);
        pauseButton.SetActive(false);

        Time.timeScale = 0f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PauseAllAudio();
        }

        if (movementButtons != null) movementButtons.SetActive(false);
        if (ensiklopediaButton != null) ensiklopediaButton.SetActive(false);
        if (fishingButton != null) fishingButton.SetActive(false);
        if (interactButton != null) interactButton.SetActive(false);
    }

    public void ResumeGame()
    {
        pauseGamePanel.SetActive(false);
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeAllAudio();
        }

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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeAllAudio();
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void ToggleMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMusic();
        }
    }
}