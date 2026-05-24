using UnityEngine;
using UnityEngine.UI;

public class ShrinkingNote : MonoBehaviour
{
    public bool isRedNote;
    public float shrinkSpeed;
    public AudioClip sfxKlikTombol;

    [Header("Accessibility")]
    public Text labelText;

    [Header("Visual Settings")]
    public RectTransform visualTransform;
    public float minHitboxScale;

    private float _currentScale = 10f;
    private PlayerStateManager _stateManager;

    void Start()
    {
        _stateManager = FindObjectOfType<PlayerStateManager>();
        transform.localScale = Vector3.one * _currentScale;

        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClick);
        }

        UpdateScale();
    }

    void Update()
    {
        float speedMultiplier = shrinkSpeed / 60f;
        
        _currentScale -= speedMultiplier;
        transform.localScale = Vector3.one * Mathf.Max(0, _currentScale);

        if (_currentScale <= 0)
        {
            if (!isRedNote)
            {
                if (_stateManager != null) _stateManager.FishingState.ChangeProgress(-0.1f);
                _stateManager.TriggerShake(1f, 0.5f);
            }
            Destroy(gameObject);
        }
    }

    private void UpdateScale()
    {
        float hitboxScale = Mathf.Max(minHitboxScale, _currentScale);
        transform.localScale = Vector3.one * hitboxScale;

        if (visualTransform != null)
        {
            float childLocalScale = Mathf.Max(0, _currentScale) / hitboxScale;
            visualTransform.localScale = Vector3.one * childLocalScale;
        }
    }

    public void OnClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sfxKlikTombol);
        }

        if (_stateManager != null)
        {
            if (isRedNote)
            {
                _stateManager.FishingState.ChangeProgress(-0.1f);
                _stateManager.TriggerShake(1f, 0.5f);
            }
            else
            {
                _stateManager.FishingState.ChangeProgress(0.05f);
            }
        }
        
        Destroy(gameObject);
    }
}