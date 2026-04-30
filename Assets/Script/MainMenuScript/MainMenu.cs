using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("SFX Clips")]
    public AudioClip clickButton;

    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickButton);
        }
    }

    public void QuitGame()
    {
        Application.Quit();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickButton);
        }
    }

    public void ToggleMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMusic();
        }
    }
}