using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaSlot : MonoBehaviour
{
    public Image fishImage;
    private FishData _data;
    private EncyclopediaManager _manager;

    public void Setup(FishData data, bool isCaught, EncyclopediaManager manager)
    {
        _data = data;
        _manager = manager;
        fishImage.sprite = data.fishIcon;

        fishImage.color = isCaught ? Color.white : Color.black;
        GetComponent<Button>().interactable = isCaught;
    }

    public void OnClick()
    {
        _manager.DisplayFishDetails(_data);
    }
}