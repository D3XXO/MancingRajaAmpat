using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

public class HoldDifficultyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Settings")]
    public float holdDuration;

    [Header("References")]
    public Image fillImage;
    public int thisButtonDifficultyIndex;
    
    public GameObject notificationTextObj;
    public Text notificationText;

    [Header("Events")]
    public UnityEvent onHoldComplete;

    private bool _isPointerDown = false;
    private float _holdTimer = 0f;
    private bool _hasTriggered = false;
    private bool _isCurrentActiveLevel = false;

    private void OnEnable()
    {
        ResetButton();

        if (notificationTextObj != null)
        {
            notificationTextObj.SetActive(false);
        }

        if (thisButtonDifficultyIndex != -1)
        {
            int currentLevel = (int)DifficultyManager.GetCurrentLevel();
            _isCurrentActiveLevel = (thisButtonDifficultyIndex == currentLevel);

            if (fillImage != null)
            {
                Color c = fillImage.color;
                c.a = _isCurrentActiveLevel ? 0.1f : 1f;
                fillImage.color = c;
            }
        }
    }

    void Update()
    {
        if (_isPointerDown && !_hasTriggered)
        {
            _holdTimer += Time.unscaledDeltaTime;

            if (fillImage != null)
            {
                fillImage.fillAmount = 1f - (_holdTimer / holdDuration);
            }

            if (_holdTimer >= holdDuration)
            {
                bool isFree = PlayerPrefs.GetInt("FreeDifficultyUsed", 0) == 0;
                _hasTriggered = true;
                onHoldComplete?.Invoke();
                ResetButton();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isCurrentActiveLevel) return;
        _isPointerDown = true;
        _holdTimer = 0f;
        _hasTriggered = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetButton();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetButton();
    }

    private void ResetButton()
    {
        _isPointerDown = false;
        _holdTimer = 0f;
        
        if (fillImage != null)
        {
            fillImage.fillAmount = 1f;
        }
    }

    private void ShowNotification(string message, Color textColor)
    {
        if (notificationTextObj != null && notificationText != null)
        {
            notificationText.text = message;
            notificationText.color = textColor;
            notificationTextObj.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(HideNotificationRoutine());
        }
    }

    private IEnumerator HideNotificationRoutine()
    {
        yield return new WaitForSecondsRealtime(2f);
        
        if (notificationTextObj != null)
        {
            notificationTextObj.SetActive(false);
        }
    }
}