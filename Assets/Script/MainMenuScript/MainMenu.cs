using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioClip clickButton;
    public GameObject muteIcon;
    public LoadingScreen loadingScreen;

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

        loadingScreen.LoadScene("MainScene");
    }

    public void QuitGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickButton);
        }

        Application.Quit();
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