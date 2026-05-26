using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaSlot : MonoBehaviour
{
    public Image fishImage;
    public Image backgroundImage;
    private FishData _data;
    private EncyclopediaManager _manager;
    private Outline _outline;

    public void Setup(FishData data, bool isCaught, EncyclopediaManager manager)
    {
        if (_outline == null)
        {
            _outline = GetComponent<Outline>();
            if (_outline == null)
            {
                _outline = gameObject.AddComponent<Outline>();
            }
        }

        _data = data;
        _manager = manager;
        fishImage.sprite = data.fishIcon;

        fishImage.color = isCaught ? Color.white : Color.black;
        GetComponent<Button>().interactable = isCaught;

        Color bgColor = Color.white;
        Color outlineColor = Color.black;

        if (!isCaught)
        {
            ColorUtility.TryParseHtmlString("#B8B3B4", out bgColor);
            ColorUtility.TryParseHtmlString("#6B6667", out outlineColor);
        }
        else
        {
            // Sudah tertangkap
            switch (data.rarity)
            {
                case FishRarity.Normal:
                    ColorUtility.TryParseHtmlString("#9EB8D9", out bgColor);
                    ColorUtility.TryParseHtmlString("#5A7A9F", out outlineColor);
                    break;
                case FishRarity.Endemic:
                    ColorUtility.TryParseHtmlString("#A3C2A0", out bgColor);
                    ColorUtility.TryParseHtmlString("#5A7A57", out outlineColor);
                    break;
                case FishRarity.Rare:
                    ColorUtility.TryParseHtmlString("#D9C39E", out bgColor);
                    ColorUtility.TryParseHtmlString("#9B7E4F", out outlineColor);
                    break;
            }
        }

        backgroundImage.color = bgColor;

        if (_outline != null)
        {
            _outline.effectColor = outlineColor;
        }
    }

    public void OnClick()
    {
        _manager.DisplayFishDetails(_data);
    }
}