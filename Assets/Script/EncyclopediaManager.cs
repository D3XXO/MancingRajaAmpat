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

    [Header("Data")]
    public List<FishData> allFish;

    void Start()
    {
        if (encyclopediaPanel != null)
        {
            encyclopediaPanel.SetActive(false);
        }
    }

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
        bool isActive = gameObject.activeSelf;
        gameObject.SetActive(!isActive);
    }
}