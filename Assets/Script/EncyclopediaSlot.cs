using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaSlot : MonoBehaviour
{
    public Image fishImage;
    public Image backgroundImage;
    private FishData _data;
    private EncyclopediaManager _manager;

    public void Setup(FishData data, bool isCaught, EncyclopediaManager manager)
    {
        _data = data;
        _manager = manager;
        fishImage.sprite = data.fishIcon;

        fishImage.color = isCaught ? Color.white : Color.black;
        GetComponent<Button>().interactable = isCaught;

        if (isCaught)
        {
            switch (data.rarity)
            {
                case FishRarity.Normal: backgroundImage.color = new Color(0.8f, 0.9f, 1f); break;
                case FishRarity.Endemic: backgroundImage.color = new Color(0.5f, 1f, 0.5f); break;
                case FishRarity.Rare: backgroundImage.color = new Color(1f, 0.8f, 0f); break;
            }
        }
    }

    public void OnClick()
    {
        _manager.DisplayFishDetails(_data);
    }
}