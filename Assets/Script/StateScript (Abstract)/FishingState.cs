using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
public class FishingState : IPlayerState
{
    private PlayerStateManager _manager;
    private float _currentProgress = 0f;
    private FishData _activeFish;

    public FishingState(PlayerStateManager manager) => _manager = manager;

    public void Enter()
    {
        if (_manager.currentFishingZone != _manager.activeStreakZone)
        {
            _manager.ResetStreak();
            _manager.activeStreakZone = _manager.currentFishingZone;
        }

        _currentProgress = 0.2f;
        UpdateUI();

        _manager.PlayerAnimator.SetBool("isWaiting", false);
        _manager.PlayerAnimator.SetBool("PlayerUlur", true);
        _manager.PlayerAnimator.SetBool("PlayerTarik", false);
        
        _manager.fishingMinigamePanel.SetActive(true);
        _manager.fishingButton.SetActive(false);
        _manager.movementButtonsParent.SetActive(false);
        _manager.homeButton.SetActive(false);

        if (_manager.rhythmSpawner != null && _manager.availableFish.Count > 0 && _manager.currentZoneData != null)
        {
            List<FishData> validScoreFish = new List<FishData>();
            foreach (FishData fish in _manager.availableFish)
            {
                if (_manager.totalValueScore >= fish.minVSRequirement)
                {
                    validScoreFish.Add(fish);
                }
            }

            if (validScoreFish.Count > 0)
            {
                FishRarity selectedRarity = RollRarity(_manager.currentZoneData);

                List<FishData> finalPool = new List<FishData>();
                foreach (FishData fish in validScoreFish)
                {
                    if (fish.rarity == selectedRarity)
                    {
                        finalPool.Add(fish);
                    }
                }

                if (finalPool.Count == 0)
                {
                    finalPool = validScoreFish;
                }

                _activeFish = finalPool[Random.Range(0, finalPool.Count)];

                int gachaMinigame = _manager.currentZoneData.selectedMinigame;
                if (gachaMinigame == 0 && _manager.rhythmSpawner != null)
                {
                    if (_manager.shrinkingSpawner != null) _manager.shrinkingSpawner.gameObject.SetActive(false);
                    _manager.rhythmSpawner.gameObject.SetActive(true);
                    _manager.rhythmSpawner.StartSpawning(_activeFish);
                }
                else if (gachaMinigame == 1 && _manager.shrinkingSpawner != null)
                {
                    if (_manager.rhythmSpawner != null) _manager.rhythmSpawner.gameObject.SetActive(false);
                    _manager.shrinkingSpawner.gameObject.SetActive(true);
                    _manager.shrinkingSpawner.StartSpawning(_activeFish);
                }
            }
            else
            {
                _manager.SwitchState(_manager.MovementState);
            }
        }
    }

    private FishRarity RollRarity(FishingZoneData zone)
    {
        float roll = Random.Range(0f, 100f);

        if (roll <= zone.normalChance)
        {
            return FishRarity.Normal;
        }
        else if (roll <= zone.normalChance + zone.endemicChance)
        {
            return FishRarity.Endemic;
        }
        else
        {
            return FishRarity.Rare;
        }
    }

    public void ChangeProgress(float amount)
    {
        _currentProgress += amount;
        _currentProgress = Mathf.Clamp01(_currentProgress);
        UpdateUI();

        if (_currentProgress == 1f) WinFishing();
        else if (_currentProgress == 0f) LoseFishing();
    }

    private void UpdateUI()
    {
        if (_manager.rhythmProgressBar != null)
            _manager.rhythmProgressBar.fillAmount = _currentProgress;
            
        if (_manager.shrinkingProgressBar != null)
            _manager.shrinkingProgressBar.fillAmount = _currentProgress;
    }

    private void WinFishing()
    {
        if (_activeFish != null)
        {
            _manager.currentWinStreak++;
            
            int maxMultiplier = DifficultyManager.GetCurrentStrategy().GetMaxStreakMultiplier();
            int multiplier = Mathf.Min(_manager.currentWinStreak, maxMultiplier);
            int finalScore = _activeFish.vsValue * multiplier;

            PlayerPrefs.SetInt("Caught_" + _activeFish.fishID, 1);
            PlayerPrefs.Save();

            _manager.AddValueScore(finalScore, true);

            FloatingText floatScript = _manager.GetComponentInChildren<FloatingText>(true);
            if (floatScript != null)
            {
                floatScript.TriggerText("+" + finalScore, Color.white);
            }

            _manager.ShowCaughtFish(_activeFish, multiplier);
            _manager.StartCoroutine(PlayRecordAudioRoutine(_activeFish.fishID));
        }

        _manager.SwitchState(_manager.MovementState);
    }

    private IEnumerator PlayRecordAudioRoutine(string fishID)
    {
        string path = Path.Combine(Application.persistentDataPath, fishID + ".wav");
        
        if (File.Exists(path))
        {
            string fullPath = new System.Uri(path).AbsoluteUri;

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fullPath, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlaySFX(clip);
                    }
                    else if (Camera.main != null)
                    {
                        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
                    }
                }
            }
        }
    }

    private void LoseFishing()
    {
        _manager.ResetStreak();
        int penalty = Mathf.RoundToInt(_manager.totalValueScore / 10f);

        if (_manager.totalValueScore > 0)
        {
            FloatingText floatScript = _manager.GetComponentInChildren<FloatingText>(true);
            if (floatScript != null)
            {
                floatScript.TriggerText("-" + penalty, Color.red);
            }
        }

        _manager.AddValueScore(-penalty, true);
        _manager.TriggerShake(2.0f, 0.5f);
        _manager.SwitchState(_manager.MovementState);
    }

    public void Update()
    {
        AnimatorStateInfo stateInfo = _manager.PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 1f && !_manager.PlayerAnimator.IsInTransition(0))
        {
            if (stateInfo.IsName("PlayerUlur"))
            {
                _manager.PlayerAnimator.SetBool("PlayerUlur", false);
                _manager.PlayerAnimator.SetBool("PlayerTarik", true);
            }
            else if (stateInfo.IsName("PlayerTarik"))
            {
                _manager.PlayerAnimator.SetBool("PlayerTarik", false);
                _manager.PlayerAnimator.SetBool("PlayerUlur", true);
            }
        }
    }

    public void Exit()
    {
        if (_manager.fishingMinigamePanel != null)
            _manager.fishingMinigamePanel.SetActive(false);

        if (_manager.rhythmSpawner != null)
            _manager.rhythmSpawner.StopSpawning();
            
        if (_manager.shrinkingSpawner != null)
            _manager.shrinkingSpawner.StopSpawning();

        if (_manager.rhythmSpawner != null)
            _manager.rhythmSpawner.gameObject.SetActive(false);
            
        if (_manager.shrinkingSpawner != null)
            _manager.shrinkingSpawner.gameObject.SetActive(false);

        if (_manager.movementButtonsParent != null)
            _manager.movementButtonsParent.SetActive(true);
            
        if (_manager.homeButton != null)
            _manager.homeButton.SetActive(true);
            
        if (_manager.ensiklopediaButton != null)
            _manager.ensiklopediaButton.SetActive(true);
            
        if (_manager.pauseButton != null)
            _manager.pauseButton.SetActive(true);
            
        if (_manager.fishingButton != null)
            _manager.fishingButton.SetActive(_manager.IsInFishingZone);

        _activeFish = null;
    }
}