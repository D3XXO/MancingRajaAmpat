using UnityEngine;
using UnityEngine.UI;

public class FishingZone : MonoBehaviour
{
    public string zoneName;
    public GameObject textPrefab;
    public Vector3 uiOffset;

    [HideInInspector] public FishingZoneData zoneSettings = new FishingZoneData();
    private GameObject _instantiatedUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateManager player = other.GetComponent<PlayerStateManager>();
            if (player != null && player.fishingButton != null)
            {
                player.IsInFishingZone = true;
                player.fishingButton.SetActive(true);
                
                GenerateDynamicChances(player.totalValueScore, player.availableFish);

                player.currentZoneData = this.zoneSettings;

                UpdateZoneInfoUI();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateManager player = other.GetComponent<PlayerStateManager>();
            if (player != null && player.fishingButton != null)
            {
                player.IsInFishingZone = false;
                player.fishingButton.SetActive(false);
                player.currentZoneData = null;

                if (_instantiatedUI != null) Destroy(_instantiatedUI);
            }
        }
    }

    private void GenerateDynamicChances(int playerScore, System.Collections.Generic.List<FishData> allFish)
    {
        zoneSettings.zoneName = this.zoneName;

        int minEndemicReq = int.MaxValue;
        int minRareReq = int.MaxValue;

        foreach (FishData fish in allFish)
        {
            if (fish.rarity == FishRarity.Endemic && fish.minVSRequirement < minEndemicReq)
                minEndemicReq = fish.minVSRequirement;
            if (fish.rarity == FishRarity.Rare && fish.minVSRequirement < minRareReq)
                minRareReq = fish.minVSRequirement;
        }

        float normal = 100f;
        float endemic = 0f;
        float rare = 0f;

        if (playerScore >= minEndemicReq && minEndemicReq > 0)
        {
            float progressFactor = (float)playerScore / (minEndemicReq * 2);
            endemic = Mathf.Clamp(progressFactor * 25f, 10f, 40f);
        }

        if (playerScore >= minRareReq && minRareReq > 0)
        {
            float progressFactor = (float)playerScore / (minRareReq * 2);
            rare = Mathf.Clamp(progressFactor * 10f, 2f, 20f);
        }

        normal = 100f - (endemic + rare);

        zoneSettings.normalChance = Mathf.Round(normal);
        zoneSettings.endemicChance = Mathf.Round(endemic);
        zoneSettings.rareChance = Mathf.Round(rare);
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
                txt.text = $"{zoneSettings.zoneName}\n" +
                        $"Normal: {zoneSettings.normalChance}%\n" +
                        $"Endemic: {zoneSettings.endemicChance}%\n" +
                        $"Rare: {zoneSettings.rareChance}%";
            }
        }
    }
}