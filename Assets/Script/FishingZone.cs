using UnityEngine;
using UnityEngine.UI;

public class FishingZone : MonoBehaviour
{
    [Header("Zone Configuration")]
    public float gachaCooldown;

    [Header("UI World Space Setup")]
    public GameObject textPrefab;
    public Vector3 uiOffset;

    [HideInInspector] public FishingZoneData zoneSettings = new FishingZoneData();
    [HideInInspector] public bool isPlayerInside = false;
    [HideInInspector] public FishingZoneSpawner spawnerParent;
    
    private GameObject _instantiatedUI;
    private float _lastGachaTime = -999f;
    private bool _hasGeneratedBefore = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;

            PlayerStateManager player = other.GetComponent<PlayerStateManager>();
            if (player != null && player.fishingButton != null)
            {
                player.IsInFishingZone = true;
                player.fishingButton.SetActive(true);
                
                if (!_hasGeneratedBefore || Time.time - _lastGachaTime >= gachaCooldown)
                {
                    GenerateDynamicChances(player.totalValueScore, player.availableFish);
                    _lastGachaTime = Time.time;
                    _hasGeneratedBefore = true;
                }

                player.currentZoneData = this.zoneSettings;

                UpdateZoneInfoUI();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;

            PlayerStateManager player = other.GetComponent<PlayerStateManager>();
            if (player != null && player.fishingButton != null)
            {
                player.IsInFishingZone = false;
                player.fishingButton.SetActive(false);
                player.currentZoneData = null;

                if (_instantiatedUI != null) Destroy(_instantiatedUI);
            }

            if (spawnerParent != null)
            {
                spawnerParent.OnPlayerLeftZone(this.gameObject);
            }
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
    }

    private void UpdateZoneInfoUI()
    {
        if (_instantiatedUI == null && textPrefab != null)
        {
            _instantiatedUI = Instantiate(textPrefab, transform.position + uiOffset, Quaternion.identity, transform);
        }

        if (_instantiatedUI != null)
        {
            Text txt = _instantiatedUI.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = $"Normal: {zoneSettings.normalChance}%\n" +
                        $"Endemic: {zoneSettings.endemicChance}%\n" +
                        $"Rare: {zoneSettings.rareChance}%";
            }
        }
    }
}