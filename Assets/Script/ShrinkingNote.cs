using UnityEngine;
using UnityEngine.UI;

public class ShrinkingNote : MonoBehaviour
{
    public bool isRedNote;
    public float shrinkSpeed;
    public AudioClip clickSfx;

    [Header("Accessibility")]
    public Text labelText;

    [Header("Visual Settings")]
    public RectTransform visualTransform;
    public float minHitboxScale;
    public GameObject explosionPrefab;

    private float _currentScale;
    private PlayerStateManager _stateManager;

    void Start()
    {
        _stateManager = FindObjectOfType<PlayerStateManager>();

        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClick);
        }
    }

    public void SetupNote(float startScale, float speed)
    {
        _currentScale = startScale;
        shrinkSpeed = speed;
        transform.localScale = Vector3.one * _currentScale;
        UpdateScale();
    }

    void Update()
    {
        float speedMultiplier = (shrinkSpeed / 40f);
        
        _currentScale -= speedMultiplier;
        UpdateScale();

        if (_currentScale <= 0)
        {
            if (!isRedNote)
            {
                if (_stateManager != null) _stateManager.FishingState.ChangeProgress(-0.1f);
                if (_stateManager != null) _stateManager.TriggerShake(2.0f, 0.5f);
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
        if (_stateManager != null)
        {
            if (isRedNote)
            {
                _stateManager.FishingState.ChangeProgress(-0.1f);
                _stateManager.TriggerShake(2.0f, 0.5f);
            }
            else
            {
                _stateManager.FishingState.ChangeProgress(0.05f);
            }
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFeedback(clickSfx);
        }

        SpawnExplosion();
        Destroy(gameObject);
    }

    private void SpawnExplosion()
    {
        if (explosionPrefab != null)
        {
            GameObject vfx = Instantiate(explosionPrefab, transform.position, Quaternion.identity, transform.parent);
            
            Image targetImage = visualTransform != null ? visualTransform.GetComponent<Image>() : GetComponent<Image>();
            if (targetImage != null)
            {
                ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var main = ps.main;
                    main.startColor = targetImage.color;
                }
            }

            Destroy(vfx, 1f);
        }
    }
}