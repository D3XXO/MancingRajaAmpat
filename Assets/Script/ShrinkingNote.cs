using UnityEngine;
using UnityEngine.UI;

public class ShrinkingNote : MonoBehaviour
{
    public bool isRedNote;
    public float shrinkSpeed;
    public AudioClip sfxKlikTombol;

    [Header("Accessibility")]
    public Text labelText;

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
    }

    void Update()
    {
        float speedMultiplier = shrinkSpeed / 80f;
        
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