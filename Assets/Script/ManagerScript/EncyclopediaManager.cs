using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EncyclopediaManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject encyclopediaPanel;
    public Transform gridParent;
    public GameObject slotPrefab;
    public Text descriptionText;
    public Text nameText;
    public Image detailImage;

    [Header("Controls to Toggle")]
    public GameObject movementButtons;
    public GameObject fishingButton;
    public GameObject interactButton;

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
        nameText.text = data.fishName;
        descriptionText.text = data.description;
        detailImage.sprite = data.fishIcon;
        detailImage.color = Color.white;
    }


    public void ToggleEncyclopedia()
    {
        if (encyclopediaPanel == null) return;
        bool willBeActive = !encyclopediaPanel.activeSelf;

        if (willBeActive)
        {
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
}