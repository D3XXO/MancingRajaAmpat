using UnityEngine;
using UnityEngine.UI;

public class RhythmNote : MonoBehaviour
{
    public int noteID;
    public float moveSpeed;
    public bool isRedNote;
    public Text labelText;

    private Image _noteImage;
    private bool _isTransparent = false;

    void Awake()
    {
        _noteImage = GetComponent<Image>();
    }

    void Update()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        if (transform.localPosition.x < -550f)
        {
            if (!isRedNote)
            {
                FindObjectOfType<PlayerStateManager>().FishingState.ChangeProgress(-0.1f);
            }

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
            
            if (labelText != null)
            {
                Color txtColor = labelText.color;
                txtColor.a = 0.1f;
                labelText.color = txtColor;
            }
            
            _isTransparent = true;
        }
    }

    private void OnDestroy()
    {
        RhythmManager manager = FindObjectOfType<RhythmManager>();
        if (manager != null) manager.allActiveNotes.Remove(this);
    }
}