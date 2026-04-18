using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public AudioSource bgmSource;

    // Fungsi untuk tombol Play
    public void PlayGame()
    {
        
        SceneManager.LoadScene("MainScene");
    }

    
    public void QuitGame()
    {
        Debug.Log("Game Keluar!");
        Application.Quit();
    }

    
    public void ToggleMusic()
    {
        if (bgmSource != null)
        {
            bgmSource.mute = !bgmSource.mute;
        }
    }
}