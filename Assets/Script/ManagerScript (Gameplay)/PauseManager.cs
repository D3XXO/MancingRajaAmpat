using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseGamePanel;
    public GameObject selectionLevelPanel;
    public GameObject movementButtons;
    public GameObject ensiklopediaButton;
    public GameObject fishingButton;
    public GameObject interactButton;
    public GameObject pauseButton;
    public GameObject muteIcon;
    public GameObject homeButton;
    public AudioClip clickButton;

    private PlayerStateManager _player;
    public LoadingScreen loadingScreen;

    void Start()
    {
        _player = FindObjectOfType<PlayerStateManager>();

        if (AudioManager.Instance != null && muteIcon != null)
        {
            muteIcon.SetActive(AudioManager.Instance.IsMuted);
        }
    }

    public void PauseGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickButton);
        }

        PlayerStateManager player = FindObjectOfType<PlayerStateManager>();
        if (player != null && player.movementComponent != null)
        {
            player.movementComponent.StopMoving();
        }

        pauseGamePanel.SetActive(true);
        pauseButton.SetActive(false);

        Time.timeScale = 0f;

        if (movementButtons != null) movementButtons.SetActive(false);
        if (ensiklopediaButton != null) ensiklopediaButton.SetActive(false);
        if (fishingButton != null) fishingButton.SetActive(false);
        if (interactButton != null) interactButton.SetActive(false);
        if (homeButton != null) homeButton.SetActive(false);
    }

    public void ResumeGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickButton);
        }

        pauseGamePanel.SetActive(false);
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeAllAudio();
        }

        RestoreGameplayUI();
    }

    public void OpenSelectionLevel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(clickButton);
        selectionLevelPanel.SetActive(true);
        pauseGamePanel.SetActive(false);
    }

    public void ChangeDifficulty(int levelIndex)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(clickButton);
        
        DifficultyLevel currentLevel = DifficultyManager.GetCurrentLevel();
        DifficultyLevel targetLevel = (DifficultyLevel)levelIndex;

        if (currentLevel == targetLevel)
        {
            return;
        }

        DifficultyManager.SetDifficulty(targetLevel);

        if (_player != null)
        {
            _player.UpdateDifficultyText();
            _player.ResetStreak();

            if (_player.currentFishingZone != null)
            {
                _player.currentFishingZone.DestroyZoneUI();
                _player.currentFishingZone.UpdateZoneInfoUI();
            }
        }

        selectionLevelPanel.SetActive(false);
        pauseGamePanel.SetActive(true);
    }

    private void RestoreGameplayUI()
    {
        if (_player == null) return;

        bool isMoving = _player.CurrentState == _player.MovementState;
        
        if (movementButtons != null) movementButtons.SetActive(isMoving);
        if (ensiklopediaButton != null) ensiklopediaButton.SetActive(isMoving);
        if (homeButton != null) homeButton.SetActive(isMoving);
        if (fishingButton != null) fishingButton.SetActive(isMoving && _player.IsInFishingZone);

        pauseButton.SetActive(true);
    }

    public void MainMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickButton);
        }

        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeAllAudio();
        }

        loadingScreen.LoadScene("MainMenu");
    }

    public void ToggleMusic()
    {
        if (AudioManager.Instance != null)
        {
            bool isMuted = AudioManager.Instance.ToggleMusic();
            muteIcon.SetActive(isMuted);
        }
    }
}