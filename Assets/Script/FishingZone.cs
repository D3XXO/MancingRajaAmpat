using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FishingZone : MonoBehaviour
{
    [Header("Zone Configuration")]
    public float gachaCooldown;

    [Header("UI World Space Setup")]
    public GameObject textPrefab;
    public Vector3 uiOffset;

    [Header("Streak UI Setup")]
    public GameObject streakTextPrefab;
    public Vector3 streakUiOffset;
    private GameObject _instantiatedStreakUI;

    [HideInInspector] public FishingZoneData zoneSettings = new FishingZoneData();
    [HideInInspector] public bool isPlayerInside = false;
    [HideInInspector] public FishingZoneSpawner spawnerParent;
    
    private GameObject _instantiatedUI;
    private bool _hasGeneratedBefore = false;
    private PlayerStateManager _currentPlayer;

    private Coroutine _cooldownCoroutine;
    private float _currentCooldownTimer = 0f;

    public void ForceUpdateChances(int playerScore, System.Collections.Generic.List<FishData> allFish)
    {
        GenerateDynamicChances(playerScore, allFish);
        _hasGeneratedBefore = true;

        if (isPlayerInside)
        {
            UpdateZoneInfoUI();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            _currentPlayer = other.GetComponent<PlayerStateManager>();

            if (_cooldownCoroutine != null)
            {
                StopCoroutine(_cooldownCoroutine);
                _cooldownCoroutine = null;
            }

            if (_currentPlayer != null && _currentPlayer.fishingButton != null)
            {
                _currentPlayer.IsInFishingZone = true;
                _currentPlayer.fishingButton.SetActive(true);

                _currentPlayer.currentFishingZone = this;
                
                if (!_hasGeneratedBefore)
                {
                    GenerateDynamicChances(_currentPlayer.totalValueScore, _currentPlayer.availableFish);
                    _hasGeneratedBefore = true;
                }

                _currentPlayer.currentZoneData = this.zoneSettings;
                UpdateZoneInfoUI();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;

            if (_currentPlayer != null && _currentPlayer.fishingButton != null)
            {
                _currentPlayer.IsInFishingZone = false;
                _currentPlayer.fishingButton.SetActive(false);
                _currentPlayer.currentZoneData = null;
                _currentPlayer.currentFishingZone = null;

                if (_instantiatedUI != null) Destroy(_instantiatedUI);
                if (_instantiatedStreakUI != null) Destroy(_instantiatedStreakUI);
            }

            _currentPlayer = null;

            _cooldownCoroutine = StartCoroutine(CooldownRoutine());

            if (spawnerParent != null)
            {
                spawnerParent.OnPlayerLeftZone(this.gameObject);
            }
        }
    }

    private IEnumerator CooldownRoutine()
    {
        while (_currentCooldownTimer < gachaCooldown)
        {
            _currentCooldownTimer += Time.deltaTime;
            yield return null;
        }

        _hasGeneratedBefore = false;
        _currentCooldownTimer = 0f;

        PlayerStateManager player = FindObjectOfType<PlayerStateManager>();
        if (player != null && player.activeStreakZone == this)
        {
            player.ResetStreak();
            player.activeStreakZone = null;
        }
    }

    private void GenerateDynamicChances(int playerScore, System.Collections.Generic.List<FishData> allFish)
    {
        int minEndemicReq = int.MaxValue;
        int minRareReq = int.MaxValue;

        foreach (FishData fish in allFish)
        {
            if (fish.rarity == FishRarity.Endemic && fish.minVSRequirement < minEndemicReq)
                minEndemicReq = fish.minVSRequirement;
            if (fish.rarity == FishRarity.Rare && fish.minVSRequirement < minRareReq)
                minRareReq = fish.minVSRequirement;
        }

        float maxEndemicPossible = 0f;
        float maxRarePossible = 0f;

        if (playerScore >= minEndemicReq) maxEndemicPossible = 50f;
        if (playerScore >= minRareReq) maxRarePossible = 30f;

        float endemicChance = Random.Range(0f, maxEndemicPossible);
        float rareChance = Random.Range(0f, maxRarePossible);
        float normalChance = 100f - (endemicChance + rareChance);

        zoneSettings.normalChance = Mathf.Round(normalChance);
        zoneSettings.endemicChance = Mathf.Round(endemicChance);
        zoneSettings.rareChance = Mathf.Round(rareChance);

        zoneSettings.selectedMinigame = Random.Range(0, 2);
    }

    private void UpdateZoneInfoUI()
    {
        if (_instantiatedUI == null && textPrefab != null)
        {
            _instantiatedUI = Instantiate(textPrefab, transform.position + uiOffset, Quaternion.identity, transform);
        }

        if (_instantiatedUI != null)
        {
            string ritemMode = zoneSettings.selectedMinigame == 0 ? "Flow" : "Catch";

            Text txt = _instantiatedUI.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = $"(Ritem {ritemMode} Mode)\n\n" +
                        $"Normal: {zoneSettings.normalChance}%\n" +
                        $"Endemic: {zoneSettings.endemicChance}%\n" +
                        $"Rare: {zoneSettings.rareChance}%";
            }
        }

        PlayerStateManager player = FindObjectOfType<PlayerStateManager>();
        
        bool hasStreak = (player != null && player.activeStreakZone == this && player.currentWinStreak >= 2);

        if (hasStreak)
        {
            if (_instantiatedStreakUI == null && streakTextPrefab != null)
            {
                _instantiatedStreakUI = Instantiate(streakTextPrefab, transform.position + streakUiOffset, streakTextPrefab.transform.rotation, transform);
            }

            if (_instantiatedStreakUI != null)
            {
                Text streakTxt = _instantiatedStreakUI.GetComponentInChildren<Text>();
                if (streakTxt != null)
                {
                    int multiplier = player.currentWinStreak + 1;
                    streakTxt.text = $"Streak berikutnya X{multiplier}";
                }
            }
        }
        else
        {
            if (_instantiatedStreakUI != null)
            {
                Destroy(_instantiatedStreakUI);
                _instantiatedStreakUI = null;
            }
        }
    }

    private void OnDestroy()
    {
        PlayerStateManager player = FindObjectOfType<PlayerStateManager>();
        if (player != null && player.activeStreakZone == this)
        {
            player.ResetStreak();
            player.activeStreakZone = null;
        }
    }
}