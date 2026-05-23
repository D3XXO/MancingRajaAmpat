using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Text textDisplay;
    public float moveSpeed;
    public float duration;

    private Color _currentColor;
    private float _timer;
    private Vector3 _startLocalPosition;

    void Awake()
    {
        _startLocalPosition = transform.localPosition;
    }

    public void TriggerText(string text, Color color)
    {
        transform.localPosition = _startLocalPosition;
        _timer = 0f;
        
        _currentColor = color;

        if (textDisplay != null)
        {
            textDisplay.text = text;
            textDisplay.color = color;
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
            textDisplay.color = new Color(_currentColor.r, _currentColor.g, _currentColor.b, alpha);
        }

        if (_timer >= duration)
        {
            gameObject.SetActive(false);
        }
    }
}