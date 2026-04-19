using UnityEngine;
using System.Collections.Generic;
public class FishingState : IPlayerState
{
    private PlayerStateManager _manager;
    private bool _isTugging;
    private float _currentProgress = 0f;
    private FishData _activeFish;

    public FishingState(PlayerStateManager manager) => _manager = manager;

    public void Enter()
    {
        _currentProgress = 0.2f;
        UpdateUI();

        _manager.PlayerAnimator.SetBool("isWaiting", false);
        _manager.PlayerAnimator.SetBool("PlayerUlur", true);
        
        _manager.fishingMinigamePanel.SetActive(true);
        _manager.fishingButton.SetActive(false);
        _manager.movementButtonsParent.SetActive(false);

        if (_manager.rhythmSpawner != null && _manager.availableFish.Count > 0)
        {
            List<FishData> catchableFish = new List<FishData>();

            foreach (FishData fish in _manager.availableFish)
            {
                if (_manager.totalValueScore >= fish.minVSRequirement)
                {
                    catchableFish.Add(fish);
                }
            }

            if (catchableFish.Count > 0)
            {
                _activeFish = catchableFish[Random.Range(0, catchableFish.Count)];
                _manager.rhythmSpawner.StartSpawning(_activeFish);
            }
            else
            {
                Debug.LogWarning("Tidak ada ikan yang tersedia untuk skor saat ini!");
            }
        }
    }

    public void ChangeProgress(float amount)
    {
        _currentProgress += amount;
        _currentProgress = Mathf.Clamp01(_currentProgress);
        UpdateUI();

        if (_currentProgress >= 1f) WinFishing();
        else if (_currentProgress <= 0f) LoseFishing();
    }

    private void UpdateUI()
    {
        if (_manager.fishingProgressBar != null)
            _manager.fishingProgressBar.fillAmount = _currentProgress;
    }

    private void WinFishing()
    {
        if (_activeFish != null)
        {
            PlayerPrefs.SetInt("Caught_" + _activeFish.fishID, 1);
            PlayerPrefs.Save();

            _manager.AddValueScore(_activeFish.vsValue);
            _manager.ShowCaughtFish(_activeFish);
        }

        _manager.SwitchState(_manager.MovementState);
    }

    private void LoseFishing()
    {
        Debug.Log("Ikan Lepas!");
        _manager.SwitchState(_manager.MovementState);
    }

    public void Update()
    {
        _manager.PlayerAnimator.SetBool("PlayerTarik", _isTugging);
        _manager.PlayerAnimator.SetBool("PlayerUlur", !_isTugging);
    }

    public void Exit()
    {
        _manager.fishingMinigamePanel.SetActive(false);
        _manager.fishingButton.SetActive(true);
        _manager.ensiklopediaButton.SetActive(true);

        if (_manager.rhythmSpawner != null) _manager.rhythmSpawner.StopSpawning();

        _activeFish = null;
    }

    public void SetTugging(bool status) => _isTugging = status;
}