using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 _originalScale;
    public float shrinkFactor;
    private bool _isInitialized = false;

    void Awake()
    {
        _originalScale = transform.localScale;
        _isInitialized = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = _originalScale * shrinkFactor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = _originalScale;
    }

    void OnDisable()
    {
        if (_isInitialized)
        {
            transform.localScale = _originalScale;
        }
    }
}