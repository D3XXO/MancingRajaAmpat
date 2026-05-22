using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Text textDisplay;
    public float moveSpeed;
    public float duration;

    private Color _textColor;
    private float _timer;
    private Vector3 _startLocalPosition;

    void Awake()
    {
        _startLocalPosition = transform.localPosition;
        
        if (textDisplay != null)
        {
            _textColor = textDisplay.color;
        }
    }

    public void TriggerText(string text, Color color)
    {
        transform.localPosition = _startLocalPosition;
        _timer = 0f;

        if (textDisplay != null)
        {
            textDisplay.text = text;
            textDisplay.color = new Color(color.r, color.g, color.b, 1f);
        }

        gameObject.SetActive(true);
    }

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        _timer += Time.deltaTime;
        
        if (textDisplay != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, _timer / duration);
            textDisplay.color = new Color(_textColor.r, _textColor.g, _textColor.b, alpha);
        }

        if (_timer >= duration)
        {
            gameObject.SetActive(false);
        }
    }
}