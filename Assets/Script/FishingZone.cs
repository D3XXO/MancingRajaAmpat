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

    [Header("UI Off-Screen Indicator Setup")]
    public GameObject leftIndicatorPrefab;
    public GameObject rightIndicatorPrefab;
    [HideInInspector] public bool hasBeenVisited = false;
    [HideInInspector] public bool hasBeenEnteredOnce = false;
    
    private GameObject _leftIndicator;
    private GameObject _rightIndicator;
    private Image _leftIndicatorImage;
    private Image _rightIndicatorImage;
    private Transform _leftContainer;
    private Transform _rightContainer;
    private Camera _mainCamera;
    private Color _indicatorGoldColor;
    private bool _isBlinkingIndicator = false;
    private SpriteRenderer _spriteRenderer;

    [HideInInspector] public FishingZoneData zoneSettings = new FishingZoneData();
    [HideInInspector] public bool isPlayerInside = false;
    [HideInInspector] public FishingZoneSpawner spawnerParent;
    
    private GameObject _instantiatedUI;
    private bool _hasGeneratedBefore = false;
    private PlayerStateManager _currentPlayer;

    private Coroutine _cooldownCoroutine;
    private float _currentCooldownTimer = 0f;
    private int _fishingAttempts = 0;
    private int _endemicAttempts = 0;
    private int _rareAttempts = 0;
    private bool _unlockedEndemicInZone = false;
    private bool _unlockedRareInZone = false;
    private float _baseEndemicChance = 0f;
    private float _baseRareChance = 0f;
    private float _endemicDecayRate = 0f;
    private float _rareDecayRate = 0f;
    private const float INDICATOR_ALPHA_MAX = 0.5f;

    void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _mainCamera = Camera.main;
        ColorUtility.TryParseHtmlString("#FFD700", out _indicatorGoldColor);

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _indicatorGoldColor;
        }

        PlayerStateManager player = FindObjectOfType<PlayerStateManager>();
        if (player != null)
        {
            Collider2D zoneCollider = GetComponent<Collider2D>();
            if (zoneCollider != null && zoneCollider.OverlapPoint(player.transform.position))
            {
                hasBeenVisited = true;
                hasBeenEnteredOnce = true;

                if (_spriteRenderer != null)
                {
                    _spriteRenderer.color = Color.white;
                }
            }
        }

        GameObject leftObj = GameObject.Find("LeftIndicatorContainer");
        GameObject rightObj = GameObject.Find("RightIndicatorContainer");

        if (leftObj != null) _leftContainer = leftObj.transform;
        if (rightObj != null) _rightContainer = rightObj.transform;

        if (leftIndicatorPrefab != null && _leftContainer != null)
        {
            _leftIndicator = Instantiate(leftIndicatorPrefab, _leftContainer);
            _leftIndicatorImage = _leftIndicator.GetComponent<Image>();
            _leftIndicator.SetActive(false);
        }

        if (rightIndicatorPrefab != null && _rightContainer != null)
        {
            _rightIndicator = Instantiate(rightIndicatorPrefab, _rightContainer);
            _rightIndicatorImage = _rightIndicator.GetComponent<Image>();
            _rightIndicator.SetActive(false);
        }

        StartCoroutine(BlinkIndicatorRoutine());
    }

    void Update()
    {
        PlayerStateManager tempPlayer = _currentPlayer;
        if (tempPlayer == null) tempPlayer = FindObjectOfType<PlayerStateManager>();
        if (tempPlayer != null && tempPlayer.isTutorialActive)
        {
            if (_leftIndicator != null && _leftIndicator.activeSelf) _leftIndicator.SetActive(false);
            if (_rightIndicator != null && _rightIndicator.activeSelf) _rightIndicator.SetActive(false);
            return;
        }

        if (_mainCamera != null)
        {
            Vector3 viewportPos = _mainCamera.WorldToViewportPoint(transform.position);
            
            bool isVisible = viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1 && viewportPos.z > 0;

            if (isVisible)
            {
                if (_leftIndicator != null && _leftIndicator.activeSelf) _leftIndicator.SetActive(false);
                if (_rightIndicator != null && _rightIndicator.activeSelf) _rightIndicator.SetActive(false);
            }
            else
            {
                Color targetColor = hasBeenVisited ? Color.white : _indicatorGoldColor;
                targetColor.a = INDICATOR_ALPHA_MAX;

                if (viewportPos.x < 0f)
                {
                    if (_leftIndicator != null)
                    {
                        if (!_leftIndicator.activeSelf) _leftIndicator.SetActive(true);
                        if (_leftIndicatorImage != null && !_isBlinkingIndicator) _leftIndicatorImage.color = targetColor;
                    }
                    if (_rightIndicator != null && _rightIndicator.activeSelf) _rightIndicator.SetActive(false);
                }
                else if (viewportPos.x > 1f)
                {
                    if (_rightIndicator != null)
                    {
                        if (!_rightIndicator.activeSelf) _rightIndicator.SetActive(true);
                        if (_rightIndicatorImage != null && !_isBlinkingIndicator) _rightIndicatorImage.color = targetColor;
                    }
                    if (_leftIndicator != null && _leftIndicator.activeSelf) _leftIndicator.SetActive(false);
                }
            }
        }
    }

    private IEnumerator BlinkIndicatorRoutine()
    {
        _isBlinkingIndicator = true;
        int blinkCount = 3;
        float blinkDuration = 0.5f;

        for (int i = 0; i < blinkCount; i++)
        {
            SetIndicatorAlpha(0f);
            yield return new WaitForSeconds(blinkDuration);

            SetIndicatorAlpha(1f);
            yield return new WaitForSeconds(blinkDuration);
        }

        _isBlinkingIndicator = false;
    }

    private void SetIndicatorAlpha(float alphaMultiplier)
    {
        float finalAlpha = alphaMultiplier * INDICATOR_ALPHA_MAX;

        if (_leftIndicatorImage != null)
        {
            Color c = _indicatorGoldColor;
            c.a = finalAlpha;
            _leftIndicatorImage.color = c;
        }

        if (_rightIndicatorImage != null)
        {
            Color c = _indicatorGoldColor;
            c.a = finalAlpha;
            _rightIndicatorImage.color = c;
        }
    }

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
            _currentPlayer = other.GetComponent<PlayerStateManager>();
            isPlayerInside = true;
            hasBeenVisited = true;
            hasBeenEnteredOnce = true;

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.white;
            }

            if (_cooldownCoroutine != null)
            {
                StopCoroutine(_cooldownCoroutine);
                _cooldownCoroutine = null;
            }

            if (_currentPlayer != null && _currentPlayer.fishingButton != null)
            {
                _currentPlayer.IsInFishingZone = true;
                _currentPlayer.SetButtonVisualState(_currentPlayer.fishingButton, true);

                _currentPlayer.currentFishingZone = this;
                
                if (!_hasGeneratedBefore)
                {
                    _fishingAttempts = 0;
                    _endemicAttempts = 0;
                    _rareAttempts = 0;
                    _unlockedEndemicInZone = false;
                    _unlockedRareInZone = false;
                    _baseEndemicChance = 0f;
                    _baseRareChance = 0f;
                    _endemicDecayRate = 0f;
                    _rareDecayRate = 0f;

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

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.white;
            }

            if (_currentPlayer != null && _currentPlayer.fishingButton != null)
            {
                _currentPlayer.IsInFishingZone = false;
                _currentPlayer.SetButtonVisualState(_currentPlayer.fishingButton, false);
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
        hasBeenVisited = false;

        if (_spriteRenderer != null)
        {
            Color goldColor;
            ColorUtility.TryParseHtmlString("#FFD700", out goldColor);
            _spriteRenderer.color = goldColor;
        }

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

        bool meetsEndemic = playerScore >= minEndemicReq;
        bool meetsRare = playerScore >= minRareReq;

        if (meetsEndemic && !_unlockedEndemicInZone)
        {
            _unlockedEndemicInZone = true;
            _endemicAttempts = 0;
            _baseEndemicChance = Random.Range(10f, 50f);
            _endemicDecayRate = Random.Range(5f, 12f);
        }

        if (meetsRare && !_unlockedRareInZone)
        {
            _unlockedRareInZone = true;
            _rareAttempts = 0;
            _baseRareChance = Random.Range(5f, 35f);
            _rareDecayRate = Random.Range(3f, 8f);
        }

        float endemicChance = 0f;
        if (_unlockedEndemicInZone)
        {
            endemicChance = Mathf.Max(0f, _baseEndemicChance - (_endemicAttempts * _endemicDecayRate));
        }

        float rareChance = 0f;
        if (_unlockedRareInZone)
        {
            rareChance = Mathf.Max(0f, _baseRareChance - (_rareAttempts * _rareDecayRate));
        }

        float normalChance = 100f - (endemicChance + rareChance);

        zoneSettings.normalChance = Mathf.Round(normalChance);
        zoneSettings.endemicChance = Mathf.Round(endemicChance);
        zoneSettings.rareChance = Mathf.Round(rareChance);

        zoneSettings.selectedMinigame = Random.Range(0, 2);
    }

    public void UpdateZoneInfoUI()
    {
        if (_instantiatedUI == null && textPrefab != null)
        {
            _instantiatedUI = Instantiate(textPrefab, transform.position + uiOffset, Quaternion.identity, transform);
        }

        if (_instantiatedUI != null)
        {
            string ritemMode = zoneSettings.selectedMinigame == 0 ? "Flow" : "Catch";

            string uiText = $"(Ritem {ritemMode} Mode)\n\n" +
                            $"Normal: {zoneSettings.normalChance}%\n" +
                            $"Endemic: {zoneSettings.endemicChance}%\n" +
                            $"Rare: {zoneSettings.rareChance}%";

            Text txt = _instantiatedUI.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = uiText;
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
                    int nextStreak = player.currentWinStreak + 1;
                    int maxMultiplier = DifficultyManager.GetCurrentStrategy().GetMaxStreakMultiplier();
                    int displayedMultiplier = Mathf.Min(nextStreak, maxMultiplier);

                    if (DifficultyManager.GetCurrentLevel() == DifficultyLevel.Easy)
                    {
                        streakTxt.text = "Streak!";
                    }
                    else
                    {
                        streakTxt.text = $"Streak berikutnya X{displayedMultiplier}";
                    }
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

    public void RecordFishingAttempt()
    {
        _fishingAttempts++;
        if (_unlockedEndemicInZone) _endemicAttempts++;
        if (_unlockedRareInZone) _rareAttempts++;
        
        if (_currentPlayer != null)
        {
            ForceUpdateChances(_currentPlayer.totalValueScore, _currentPlayer.availableFish);
        }
    }

    private void OnDestroy()
    {
        if (_leftIndicator != null) Destroy(_leftIndicator);
        if (_rightIndicator != null) Destroy(_rightIndicator);

        PlayerStateManager player = FindObjectOfType<PlayerStateManager>();
        if (player != null && player.activeStreakZone == this)
        {
            player.ResetStreak();
            player.activeStreakZone = null;
        }
    }

    public void DestroyZoneUI()
    {
        if (_instantiatedUI != null)
        {
            Destroy(_instantiatedUI);
            _instantiatedUI = null;
        }

        if (_instantiatedStreakUI != null)
        {
            Destroy(_instantiatedStreakUI);
            _instantiatedStreakUI = null;
        }
    }

    public void ToggleIndicatorsByTutorial(bool status)
    {
        if (!status)
        {
            if (_leftIndicator != null) _leftIndicator.SetActive(false);
            if (_rightIndicator != null) _rightIndicator.SetActive(false);
        }
    }
}