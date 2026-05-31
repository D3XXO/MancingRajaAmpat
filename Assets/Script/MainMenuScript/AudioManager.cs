using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource feedbackSource;

    private bool _isMuted = false;
    public bool IsMuted => _isMuted;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAudioSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip) return;
        
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public bool ToggleMusic()
    {
        _isMuted = !_isMuted;
        bgmSource.mute = _isMuted;
        sfxSource.mute = _isMuted;
        feedbackSource.mute = _isMuted;

        PlayerPrefs.SetInt("IsMusicMuted", _isMuted ? 1 : 0);
        PlayerPrefs.Save();
        return _isMuted;
    }

    private void LoadAudioSettings()
    {
        _isMuted = PlayerPrefs.GetInt("IsMusicMuted", 0) == 1;
        bgmSource.mute = _isMuted;
        sfxSource.mute = _isMuted;
        feedbackSource.mute = _isMuted;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null && !_isMuted)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayFeedback(AudioClip clip, float volume = 1f)
    {
        if (clip != null && !_isMuted)
        {
            feedbackSource.PlayOneShot(clip, volume);
        }
    }

    public void PauseAllAudio()
    {
        if (bgmSource != null) bgmSource.Pause();
        if (sfxSource != null) sfxSource.Pause();
        if (feedbackSource != null) feedbackSource.Pause();
    }

    public void ResumeAllAudio()
    {
        if (bgmSource != null) bgmSource.UnPause();
        if (sfxSource != null) sfxSource.UnPause();
        if (feedbackSource != null) feedbackSource.UnPause();
    }
}