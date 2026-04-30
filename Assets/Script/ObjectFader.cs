using UnityEngine;
using System.Collections;

public class ObjectFader : MonoBehaviour
{
    public float fadedAlpha;

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Coroutine _fadeRoutine;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(fadedAlpha);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(1f);
        }
    }

    private void StartFade(float targetAlpha)
    {
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        Color targetColor = new Color(_originalColor.r, _originalColor.g, _originalColor.b, targetAlpha);
        
        while (Mathf.Abs(_spriteRenderer.color.a - targetAlpha) > 0.01f)
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, targetColor, Time.deltaTime * 2f);
            yield return null;
        }
        
        _spriteRenderer.color = targetColor;
    }
}