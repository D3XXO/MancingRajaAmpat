using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class RhythmManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform targetZoneRect;
    public PlayerStateManager stateManager;
    public GameObject feedbackTextPrefab;
    public RectTransform feedbackPanel;
    public List<RhythmNote> allActiveNotes = new List<RhythmNote>();

    public void OnButtonPressed(int buttonID)
    {
        if (stateManager != null && stateManager.rhythmSpawner != null && stateManager.rhythmSpawner.isCountingDown) return;

        RhythmNote noteToHit = null;
        float hitThreshold = 80f;
        float distance = 0f;

        for (int i = allActiveNotes.Count - 1; i >= 0; i--)
        {
            RhythmNote note = allActiveNotes[i];
            if (note != null && note.noteID == buttonID)
            {
                distance = Mathf.Abs(note.GetComponent<RectTransform>().anchoredPosition.x - targetZoneRect.anchoredPosition.x);
                if (distance <= hitThreshold)
                {
                    noteToHit = note;
                    break;
                }
            }
        }

        if (noteToHit != null)
        {
            ShowFeedback(noteToHit.isRedNote ? 999f : distance);

            if (noteToHit.isRedNote)
            {
                stateManager.FishingState.ChangeProgress(-0.1f);
                stateManager.TriggerShake(2.0f, 0.5f);
            }
            else
            {
                if (distance <= 20f)
                {
                    stateManager.FishingState.ChangeProgress(0.1f);
                }
                else if (distance <= 50f)
                {
                    stateManager.FishingState.ChangeProgress(0.05f);
                }
                else
                {
                    stateManager.FishingState.ChangeProgress(-0.1f);
                    stateManager.TriggerShake(2.0f, 0.5f);
                }
            }

            allActiveNotes.Remove(noteToHit);
            Destroy(noteToHit.gameObject);
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
        else if (distance <= 30f) { message = "PERFECT"; color = Color.green; }
        else if (distance <= 50f) { message = "GOOD"; color = Color.yellow; }
        else { message = "BAD"; color = Color.red; }

        GameObject go = Instantiate(feedbackTextPrefab, feedbackPanel);
        
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(Random.Range(-200f, 200f), Random.Range(-200f, 200f));
        
        go.GetComponent<FeedbackText>().Setup(message, color);
    }
}