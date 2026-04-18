using UnityEngine;
using UnityEngine.UI;

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

        if (_manager.rhythmSpawner != null)
        {
            _activeFish = _manager.availableFish[Random.Range(0, _manager.availableFish.Count)];
            _manager.rhythmSpawner.StartSpawning(_activeFish);
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
        string namaIkan = (_activeFish != null) ? _activeFish.fishName : "Ikan Misterius";
        Debug.Log("Ikan " + namaIkan + " Tertangkap!");

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
        if (_manager.rhythmSpawner != null) _manager.rhythmSpawner.StopSpawning();

        _activeFish = null;
    }

    public void SetTugging(bool status) => _isTugging = status;
}