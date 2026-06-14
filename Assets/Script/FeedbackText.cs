using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FeedbackText : MonoBehaviour
{
    private Text _textComponent;
    private float _lifetime = 1.0f;

    public void Setup(string message, Color color)
    {
        _textComponent = GetComponent<Text>();
        if (_textComponent == null) _textComponent = GetComponentInChildren<Text>();

        _textComponent.text = message;
        _textComponent.color = color;

        if (message == "PERFECT!!")
        {
            StartCoroutine(PerfectShakeAnim());
        }
        else if (message == "GOOD")
        {
            StartCoroutine(GoodPopFloatAnim());
        }
        else if (message == "MISS!" || message == "HAHA!")
        {
            StartCoroutine(BadFallAnim());
        }
        else
        {
            StartCoroutine(FadeOutOnly());
        }
    }

    private IEnumerator PerfectShakeAnim()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;
        
        transform.localScale = Vector3.one * 1.2f;

        while (elapsed < _lifetime)
        {
            elapsed += Time.unscaledDeltaTime;

            float shakeIntensity = 10f * (1f - (elapsed / _lifetime));
            float x = originalPos.x + Random.Range(-shakeIntensity, shakeIntensity);
            float y = originalPos.y + Random.Range(-shakeIntensity, shakeIntensity);
            transform.localPosition = new Vector3(x, y, originalPos.z);

            FadeOutLogic(elapsed);

            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator GoodPopFloatAnim()
    {
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 0.5f;
        Vector3 targetScale = Vector3.one * 1.1f;

        while (elapsed < _lifetime)
        {
            elapsed += Time.unscaledDeltaTime;

            transform.localPosition += Vector3.up * 30f * Time.unscaledDeltaTime;

            if (elapsed < 0.2f)
            {
                transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / 0.2f);
            }
            else
            {
                transform.localScale = Vector3.Lerp(targetScale, Vector3.one, (elapsed - 0.2f) / 0.8f);
            }

            FadeOutLogic(elapsed);
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator BadFallAnim()
    {
        float elapsed = 0f;
        float fallSpeed = -20f;
        float gravity = -100f;

        transform.localScale = Vector3.one * 0.9f; 

        while (elapsed < _lifetime)
        {
            elapsed += Time.unscaledDeltaTime;

            fallSpeed += gravity * Time.unscaledDeltaTime;
            transform.localPosition += Vector3.up * fallSpeed * Time.unscaledDeltaTime;

            FadeOutLogic(elapsed);
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator FadeOutOnly()
    {
        float elapsed = 0f;
        while (elapsed < _lifetime)
        {
            elapsed += Time.unscaledDeltaTime;
            transform.localPosition += Vector3.up * 10f * Time.unscaledDeltaTime;
            FadeOutLogic(elapsed);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void FadeOutLogic(float elapsed)
    {
        if (elapsed > _lifetime * 0.5f)
        {
            float alpha = 1f - ((elapsed - (_lifetime * 0.5f)) / (_lifetime * 0.5f));
            Color c = _textComponent.color;
            c.a = Mathf.Clamp01(alpha);
            _textComponent.color = c;
        }
    }
}