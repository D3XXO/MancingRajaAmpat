using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 _originalScale;
    public float shrinkFactor;

    void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = _originalScale * shrinkFactor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = _originalScale;
    }
}