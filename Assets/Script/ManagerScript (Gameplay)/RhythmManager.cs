using UnityEngine;
using System.Collections.Generic;

public class RhythmManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform targetZoneRect;
    public PlayerStateManager stateManager;
    public AudioClip sfxKlikTombol;
    
    public List<RhythmNote> allActiveNotes = new List<RhythmNote>();

    public void OnButtonPressed(int buttonID)
    {
        RhythmNote noteToHit = null;
        float hitThreshold = 80f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sfxKlikTombol);
        }

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
            if (noteToHit.isRedNote)
            {
                stateManager.FishingState.ChangeProgress(-0.1f);
            }
            else
            {
                stateManager.FishingState.ChangeProgress(0.025f);
            }

            allActiveNotes.Remove(noteToHit);
            Destroy(noteToHit.gameObject);
        }
        else
        {
            stateManager.FishingState.ChangeProgress(-0.1f);
        }
    }
}