using UnityEngine;
using UnityEngine.UI;

public class RhythmNote : MonoBehaviour
{
    public int noteID;
    public float moveSpeed;
    public bool isRedNote;

    private Image _noteImage;
    private bool _isTransparent = false;
    private bool _hasMissed = false;
    private RhythmManager _rhythmManager;

    void Awake()
    {
        _noteImage = GetComponent<Image>();
        _rhythmManager = FindObjectOfType<RhythmManager>();
    }

    void Update()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        if (transform.localPosition.x < -450f && !_hasMissed)
        {
            _hasMissed = true;
            
            if (!isRedNote)
            {
                FindObjectOfType<PlayerStateManager>().FishingState.ChangeProgress(-0.1f);
                
                if (_rhythmManager != null)
                {
                    _rhythmManager.ShowFeedback(100f);
                }
            }
        }

        if (transform.localPosition.x < -550f)
        {
            Destroy(gameObject);
        }
    }

    public void SetToTransparent()
    {
        if (!_isTransparent && _noteImage != null)
        {
            Color c = _noteImage.color;
            c.a = 0.1f;
            _noteImage.color = c;
            _isTransparent = true;
        }
    }

    private void OnDestroy()
    {
        RhythmManager manager = FindObjectOfType<RhythmManager>();
        if (manager != null) manager.allActiveNotes.Remove(this);
    }
}