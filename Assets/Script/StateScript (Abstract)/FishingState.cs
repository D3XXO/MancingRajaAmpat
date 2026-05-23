using UnityEngine;
using System.Collections.Generic;
public class FishingState : IPlayerState
{
    private PlayerStateManager _manager;
    private float _currentProgress = 0f;
    private FishData _activeFish;

    public FishingState(PlayerStateManager manager) => _manager = manager;

    public void Enter()
    {
        _currentProgress = 0.2f;
        UpdateUI();

        _manager.PlayerAnimator.SetBool("isWaiting", false);
        _manager.PlayerAnimator.SetBool("PlayerUlur", true);
        _manager.PlayerAnimator.SetBool("PlayerTarik", false);
        
        _manager.fishingMinigamePanel.SetActive(true);
        _manager.fishingButton.SetActive(false);
        _manager.movementButtonsParent.SetActive(false);

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

                int gachaMinigame = Random.Range(0, 2);

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
        else if (_currentProgress <= 0f) LoseFishing();
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
            PlayerPrefs.SetInt("Caught_" + _activeFish.fishID, 1);
            PlayerPrefs.Save();

            _manager.AddValueScore(_activeFish.vsValue);

            FloatingText floatScript = _manager.GetComponentInChildren<FloatingText>(true);
            if (floatScript != null)
            {
                floatScript.TriggerText("+" + _activeFish.vsValue, Color.white);
            }

            _manager.ShowCaughtFish(_activeFish);
        }

        _manager.SwitchState(_manager.MovementState);
    }

    private void LoseFishing()
    {
        if (_manager.totalValueScore > 0)
        {
            FloatingText floatScript = _manager.GetComponentInChildren<FloatingText>(true);
            if (floatScript != null)
            {
                floatScript.TriggerText("-2", Color.red);
            }
        }

        _manager.AddValueScore(-2);

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

        _activeFish = null;
    }
}