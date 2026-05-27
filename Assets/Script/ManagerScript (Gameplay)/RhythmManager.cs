using UnityEngine;
using System.Collections.Generic;

public class RhythmManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform targetZoneRect;
    public PlayerStateManager stateManager;
    public GameObject feedbackTextPrefab;
    public RectTransform feedbackPanel;
    public List<RhythmNote> allActiveNotes = new List<RhythmNote>();

    [Header("Threshold Settings")]
    public float rightThreshold;
    public float leftThreshold;

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
                stateManager.FishingState.ChangeProgress(-0.1f);
                stateManager.TriggerShake(2.0f, 0.5f);

                allActiveNotes.Remove(targetNote);
                Destroy(targetNote.gameObject);
                return;
            }

            bool isCorrectButton = (targetNote.noteID == buttonID);

            if (isCorrectButton)
            {
                ShowFeedback(targetDistance);

                if (targetDistance <= 20f) stateManager.FishingState.ChangeProgress(0.1f);
                else if (targetDistance <= 80f) stateManager.FishingState.ChangeProgress(0.05f);
                else
                {
                    stateManager.FishingState.ChangeProgress(-0.1f);
                    stateManager.TriggerShake(2.0f, 0.5f);
                }
            }
            else
            {
                ShowFeedback(999f);
                stateManager.FishingState.ChangeProgress(-0.1f);
                stateManager.TriggerShake(2.0f, 0.5f);
            }

            allActiveNotes.Remove(targetNote);
            Destroy(targetNote.gameObject);
        }
        else
        {
            stateManager.FishingState.ChangeProgress(-0.1f);
            stateManager.TriggerShake(2.0f, 0.5f);
            ShowFeedback(999f);
        }
    }

    private void ShowFeedback(float distance)
    {
        string message;
        Color color;

        if (distance >= 999f) { message = "BAD"; color = Color.red; }
        else if (distance <= 20f) { message = "PERFECT"; color = Color.green; }
        else if (distance <= 80f) { message = "GOOD"; color = Color.yellow; }
        else { message = "MISS"; color = Color.red; }

        GameObject go = Instantiate(feedbackTextPrefab, feedbackPanel);
        
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(Random.Range(-200f, 200f), Random.Range(-200f, 200f));
        
        go.GetComponent<FeedbackText>().Setup(message, color);
    }
}