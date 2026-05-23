using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class EncyclopediaManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject encyclopediaPanel;
    public Transform gridParent;
    public GameObject slotPrefab;
    public Text descriptionText;
    public Text nameText;
    public Image detailImage;

    [Header("Panel Detail & Konten")]
    public GameObject selectionButtonsPanel;
    public GameObject contentTextPanel;
    public Text detailContentText;
    private FishData _selectedFish;

    [Header("Controls to Toggle")]
    public GameObject movementButtons;
    public GameObject fishingButton;
    public GameObject interactButton;

    [Header("Audio Custom Settings")]
    public AudioSource fishAudioSource;
    public Text recordingStatusText;
    private AudioClip _recordingClip;
    private float _startRecordingTime;
    private bool _isRecordingCancelled;

    [Header("Data")]
    public List<FishData> allFish;

    void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (Transform child in gridParent) Destroy(child.gameObject);

        foreach (FishData fish in allFish)
        {
            GameObject slotObj = Instantiate(slotPrefab, gridParent);
            EncyclopediaSlot slot = slotObj.GetComponent<EncyclopediaSlot>();
            
            bool caught = PlayerPrefs.GetInt("Caught_" + fish.fishID, 0) == 1;
            slot.Setup(fish, caught, this);
        }
    }

    public void DisplayFishDetails(FishData data)
    {
        CancelRecording();

        _selectedFish = data;
        nameText.text = data.fishName;
        detailImage.sprite = data.fishIcon;
        detailImage.color = Color.white;

        selectionButtonsPanel.SetActive(true);
        contentTextPanel.SetActive(false);

        string savedPath = Path.Combine(Application.persistentDataPath, _selectedFish.fishID + ".wav");
        
        if (File.Exists(savedPath))
        {
            FileInfo fileInfo = new FileInfo(savedPath);
            if (fileInfo.Length < 1000)
            {
                File.Delete(savedPath);
            }
        }

        if (File.Exists(savedPath))
        {
            if (recordingStatusText != null) recordingStatusText.text = "Tekan Untuk Rekam";
            _selectedFish.customAudioPath = savedPath;
            PlayFishAudio();
        }
        else
        {
            _selectedFish.customAudioPath = "";
            _selectedFish.customAudioClip = null;
            if (recordingStatusText != null) recordingStatusText.text = "Tekan Untuk Rekam";

            if (fishAudioSource != null && fishAudioSource.isPlaying)
            {
                fishAudioSource.Stop();
            }
        }
    }

    public void ShowGeneralDescription()
    {
        CancelRecording();

        if (_selectedFish == null) return;
        
        detailContentText.text = _selectedFish.generalDescription;
        selectionButtonsPanel.SetActive(false);
        contentTextPanel.SetActive(true);
    }

    public void ShowFunFact()
    {
        CancelRecording();

        if (_selectedFish == null) return;

        detailContentText.text = _selectedFish.funFact;
        selectionButtonsPanel.SetActive(false);
        contentTextPanel.SetActive(true);
    }

    public void BackToSelection()
    {
        selectionButtonsPanel.SetActive(true);
        contentTextPanel.SetActive(false);
    }

    public void ClearDetails()
    {
        if (nameText != null) nameText.text = "";
        if (descriptionText != null) descriptionText.text = null;

        if (detailImage != null)
        {
            detailImage.sprite = null;
            detailImage.color = new Color(1, 1, 1, 0);
        }

        selectionButtonsPanel.SetActive(false);
        contentTextPanel.SetActive(false);
    }

    public void ToggleEncyclopedia()
    {
        if (encyclopediaPanel == null) return;
        bool willBeActive = !encyclopediaPanel.activeSelf;

        ClearDetails();

        if (willBeActive)
        {
            PlayerStateManager player = FindObjectOfType<PlayerStateManager>();
            if (player != null && player.movementComponent != null)
            {
                player.movementComponent.StopMoving();
            }

            RefreshUI();
        }

        PauseManager pm = FindObjectOfType<PauseManager>();
        if (pm != null && pm.pauseButton != null)
        {
            pm.pauseButton.SetActive(!willBeActive);
        }

        encyclopediaPanel.SetActive(willBeActive);

        if (movementButtons != null) movementButtons.SetActive(!willBeActive);
        if (willBeActive && interactButton != null) interactButton.SetActive(false);

        if (fishingButton != null)
        {
            if (willBeActive)
            {
                fishingButton.SetActive(false);
            }
            else
            {
                PlayerStateManager player = FindObjectOfType<PlayerStateManager>();
                if (player != null)
                {
                    fishingButton.SetActive(player.IsInFishingZone);
                }
            }
        }
    }

    public void StartRecordingVoice()
    {
        if (_selectedFish == null) return;

        _isRecordingCancelled = false;

        _recordingClip = Microphone.Start(null, false, 10, 44100);
        _startRecordingTime = Time.time;

        if (recordingStatusText != null) recordingStatusText.text = "Merekam... (Maks 10 Detik)";
    }

    public void StopRecordingVoice()
    {
        if (_selectedFish == null || !Microphone.IsRecording(null) || _isRecordingCancelled) return;

        float recordingLength = Time.time - _startRecordingTime;
        Microphone.End(null);

        string filePath = Path.Combine(Application.persistentDataPath, _selectedFish.fishID + ".wav");
        
        AudioClip trimmedClip = TrimAudioClip(_recordingClip, recordingLength);
        SaveWavFile(filePath, trimmedClip);

        _selectedFish.customAudioPath = filePath;
        _selectedFish.customAudioClip = trimmedClip;

        if (recordingStatusText != null) recordingStatusText.text = "Rekaman Berhasil!";
    }

    public void PlayFishAudio()
    {
        if (_selectedFish == null || string.IsNullOrEmpty(_selectedFish.customAudioPath)) return;

        if (_selectedFish.customAudioClip == null)
        {
            StartCoroutine(LoadAudioFromLocal(_selectedFish.customAudioPath, _selectedFish));
        }
        else
        {
            PlayClip(_selectedFish.customAudioClip);
        }
    }

    private IEnumerator LoadAudioFromLocal(string path, FishData fish)
    {
        string fullPath = new System.Uri(path).AbsoluteUri;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fullPath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                fish.customAudioClip = DownloadHandlerAudioClip.GetContent(www);
                PlayClip(fish.customAudioClip);
            }
            else
            {
                Debug.LogError("FMOD/UnityWebRequest Error: " + www.error);
            }
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (fishAudioSource != null && clip != null)
        {
            if (AudioManager.Instance != null)
            {
                fishAudioSource.mute = AudioManager.Instance.IsMuted;
            }

            fishAudioSource.clip = clip;
            fishAudioSource.Play();
        }
    }

    private AudioClip TrimAudioClip(AudioClip clip, float duration)
    {
        duration = Mathf.Max(1f, duration);

        int totalSamples = Mathf.CeilToInt(clip.frequency * duration);
        
        totalSamples = Mathf.Min(totalSamples, clip.samples);

        float[] data = new float[totalSamples * clip.channels];
        clip.GetData(data, 0);

        AudioClip newClip = AudioClip.Create(clip.name, totalSamples, clip.channels, clip.frequency, false);
        newClip.SetData(data, 0);
        
        return newClip;
    }

    private void SaveWavFile(string filepath, AudioClip clip)
    {
        byte[] wavData = SavWav.ConvertToWav(clip);
        File.WriteAllBytes(filepath, wavData);
    }

    public void CancelRecording()
    {
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
            _isRecordingCancelled = true;
            
            if (recordingStatusText != null) recordingStatusText.text = "Tekan Untuk Rekam!";
        }
    }
}