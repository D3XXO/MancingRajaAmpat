using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RhythmManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform targetZoneRect;
    public PlayerStateManager stateManager;
    public GameObject feedbackTextPrefab;
    public RectTransform feedbackPanel;
    public AudioClip perfectSfx;
    public AudioClip goodSfx;
    public List<RhythmNote> allActiveNotes = new List<RhythmNote>();

    [Header("Threshold Settings")]
    public float rightThreshold;
    public float leftThreshold;

    void OnDisable()
    {
        if (feedbackPanel != null)
        {
            foreach (Transform child in feedbackPanel)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void Update()
    {
        if (targetZoneRect == null) return;
        
        float targetX = targetZoneRect.anchoredPosition.x;

        for (int i = 0; i < allActiveNotes.Count; i++)
        {
            RhythmNote note = allActiveNotes[i];
            if (note != null)
            {
                float noteX = note.GetComponent<RectTransform>().anchoredPosition.x;
                float offset = noteX - targetX;

                if (offset < -leftThreshold)
                {
                    note.SetToTransparent();
                }
            }
        }
    }

    public void OnButtonPressed(int buttonID)
    {
        if (stateManager != null && stateManager.rhythmSpawner != null && stateManager.rhythmSpawner.isCountingDown) return;

        RhythmNote targetNote = null;
        float targetDistance = 0f;

        for (int i = 0; i < allActiveNotes.Count; i++)
        {
            RhythmNote note = allActiveNotes[i];
            if (note != null)
            {
                float noteX = note.GetComponent<RectTransform>().anchoredPosition.x;
                float targetX = targetZoneRect.anchoredPosition.x;
                
                float offset = noteX - targetX;
                
                if (offset >= -leftThreshold && offset <= rightThreshold)
                {
                    targetNote = note;
                    targetDistance = Mathf.Abs(offset);
                    break;
                }
            }
        }

        if (targetNote != null)
        {
            if (targetNote.isRedNote)
            {
                ShowFeedback(999f);
                stateManager.FishingState.ChangeProgress(-0.05f);
                stateManager.TriggerShake(2.0f, 0.5f);

                allActiveNotes.Remove(targetNote);
                Destroy(targetNote.gameObject);
                return;
            }

            bool isCorrectButton = (targetNote.noteID == buttonID);

            if (isCorrectButton)
            {
                ShowFeedback(targetDistance);

                if (targetDistance <= 20f) stateManager.FishingState.ChangeProgress(0.05f);
                else if (targetDistance <= 80f) stateManager.FishingState.ChangeProgress(0.025f);
                else
                {
                    stateManager.FishingState.ChangeProgress(-0.05f);
                    stateManager.TriggerShake(2.0f, 0.5f);
                }
            }
            else
            {
                ShowFeedback(999f);
                stateManager.FishingState.ChangeProgress(-0.05f);
                stateManager.TriggerShake(2.0f, 0.5f);
            }

            allActiveNotes.Remove(targetNote);
            Destroy(targetNote.gameObject);
        }
        else
        {
            ShowFeedback(999f);
            stateManager.FishingState.ChangeProgress(-0.05f);
            stateManager.TriggerShake(2.0f, 0.5f);
        }
    }

    public void ShowFeedback(float distance)
    {
        string message;
        Color color = Color.white;

        if (distance >= 999f)
        {
            Handheld.Vibrate();
            message = "HAHA!";
            ColorUtility.TryParseHtmlString("#D36666", out color);
        }
        else if (distance <= 20f)
        {
            message = "PERFECT!!";
            ColorUtility.TryParseHtmlString("#76A973", out color);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFeedback(perfectSfx);
            }
        }
        else if (distance <= 80f)
        {
            message = "GOOD";
            ColorUtility.TryParseHtmlString("#E1B05F", out color);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFeedback(goodSfx);
            }
        }
        else
        {
            message = "MISS!";
            ColorUtility.TryParseHtmlString("#D36666", out color);
        }

        SpawnFeedbackText(message, color);
    }

    private void SpawnFeedbackText(string message, Color color)
    {
        GameObject go = Instantiate(feedbackTextPrefab, feedbackPanel);
        
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(Random.Range(-200f, 200f), Random.Range(-200f, 200f));
        
        StartCoroutine(AnimateFeedbackText(go, message, color));
    }

    private IEnumerator AnimateFeedbackText(GameObject textObj, string message, Color color)
    {
        Text txt = textObj.GetComponent<Text>();
        if (txt == null) txt = textObj.GetComponentInChildren<Text>();

        if (txt != null)
        {
            txt.text = message;
            txt.color = color;
        }

        RectTransform rt = textObj.GetComponent<RectTransform>();
        Vector3 originalPos = rt.localPosition;

        float lifetime = 1.0f;
        float elapsed = 0f;

        if (message == "PERFECT!!") rt.localScale = Vector3.one * 1.0f;
        else if (message == "GOOD") rt.localScale = Vector3.one * 0.5f;
        else if (message == "MISS!" || message == "HAHA!") rt.localScale = Vector3.one * 0.9f;

        float fallSpeed = -20f;
        float gravity = -100f;

        while (elapsed < lifetime && textObj != null)
        {
            elapsed += Time.unscaledDeltaTime;

            if (message == "PERFECT!!")
            {
                float shake = 10f * (1f - (elapsed / lifetime));
                rt.localPosition = originalPos + new Vector3(Random.Range(-shake, shake), Random.Range(-shake, shake), 0f);
            }
            else if (message == "GOOD")
            {
                rt.localPosition += Vector3.up * 30f * Time.unscaledDeltaTime;

                if (elapsed < 0.2f) rt.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 1.1f, elapsed / 0.2f);
                else rt.localScale = Vector3.Lerp(Vector3.one * 1.1f, Vector3.one, (elapsed - 0.2f) / 0.8f);
            }
            else if (message == "MISS!" || message == "HAHA!")
            {
                fallSpeed += gravity * Time.unscaledDeltaTime;
                rt.localPosition += Vector3.up * fallSpeed * Time.unscaledDeltaTime;
            }

            if (txt != null && elapsed > lifetime * 0.5f)
            {
                float alpha = 1f - ((elapsed - (lifetime * 0.5f)) / (lifetime * 0.5f));
                Color c = txt.color;
                c.a = Mathf.Clamp01(alpha);
                txt.color = c;
            }

            yield return null;
        }

        if (textObj != null) Destroy(textObj);
    }
}