using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioClip clickButton;
    public GameObject muteIcon;
    public LoadingScreen loadingScreen;
    public GameObject selectionLevelPanel;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            muteIcon.SetActive(AudioManager.Instance.IsMuted);
        }
    }

    public void PlayGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickButton);
        }

        if (!DifficultyManager.HasSelectedDifficulty())
        {
            selectionLevelPanel.SetActive(true);
        }
        else
        {
            loadingScreen.LoadScene("MainScene");
        }
    }

    public void SelectDifficultyAndPlay(int levelIndex)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(clickButton);

        DifficultyManager.SetDifficulty((DifficultyLevel)levelIndex);
        selectionLevelPanel.SetActive(false);
        loadingScreen.LoadScene("MainScene");
    }

    public void QuitGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickButton);
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        QuitToUrl("https://d3xxo.itch.io/reel-rhythm");
#else
        Application.Quit();
#endif
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