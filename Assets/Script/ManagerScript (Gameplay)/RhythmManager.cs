using UnityEngine;
using System.Collections.Generic;

public class RhythmManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform targetZoneRect;
    public PlayerStateManager stateManager;
    
    public List<RhythmNote> allActiveNotes = new List<RhythmNote>();

    public void OnButtonPressed(int buttonID)
    {
        stateManager.FishingState.SetTugging(true);

        RhythmNote noteToHit = null;
        float hitThreshold = 80f;

        for (int i = allActiveNotes.Count - 1; i >= 0; i--)
        {
            RhythmNote note = allActiveNotes[i];
            if (note != null && note.noteID == buttonID)
            {
                float distance = Mathf.Abs(note.GetComponent<RectTransform>().anchoredPosition.x - targetZoneRect.anchoredPosition.x);

                if (distance <= hitThreshold)
                {
                    noteToHit = note;
                    break;
                }
            }
        }

        if (noteToHit != null)
        {
            stateManager.FishingState.ChangeProgress(0.1f);
            allActiveNotes.Remove(noteToHit);
            Destroy(noteToHit.gameObject);
        }
    }

    public void OnButtonReleased()
    {
        stateManager.FishingState.SetTugging(false);
    }
}