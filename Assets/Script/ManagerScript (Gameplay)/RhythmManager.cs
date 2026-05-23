using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class RhythmManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform targetZoneRect;
    public PlayerStateManager stateManager;
    public AudioClip sfxKlikTombol;
    public GameObject feedbackTextPrefab;
    public RectTransform feedbackPanel;
    public List<RhythmNote> allActiveNotes = new List<RhythmNote>();

    public void OnButtonPressed(int buttonID)
    {
        RhythmNote noteToHit = null;
        float hitThreshold = 80f;
        float distance = 0f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sfxKlikTombol);
        }

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
                stateManager.TriggerShake(1f, 0.5f);
            }
            else
            {
                stateManager.FishingState.ChangeProgress(0.05f);
            }

            allActiveNotes.Remove(noteToHit);
            Destroy(noteToHit.gameObject);
        }
        else
        {
            stateManager.FishingState.ChangeProgress(-0.1f);
            stateManager.TriggerShake(1f, 0.5f);
            ShowFeedback(999f);
        }
    }

    private void ShowFeedback(float distance)
    {
        string message;
        Color color;

        if (distance >= 999f) { message = "BAD"; color = Color.red; }
        else if (distance < 20f) { message = "PERFECT"; color = Color.green; }
        else if (distance < 70f) { message = "GOOD"; color = Color.yellow; }
        else { message = "BAD"; color = Color.red; }

        GameObject go = Instantiate(feedbackTextPrefab, feedbackPanel);
        
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(Random.Range(-50f, 50f), Random.Range(-50f, 50f));
        
        go.GetComponent<FeedbackText>().Setup(message, color);
    }
}