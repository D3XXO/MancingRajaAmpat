using UnityEngine;
using System.Collections.Generic;

public class TargetZone : MonoBehaviour
{
    public List<RhythmNote> currentNotesInZone = new List<RhythmNote>();

    private void OnTriggerEnter2D(Collider2D other)
{
    if (other.TryGetComponent(out RhythmNote note))
    {
        if (!currentNotesInZone.Contains(note))
        {
            currentNotesInZone.Add(note);
        }
    }
}

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out RhythmNote note))
        {
            if (currentNotesInZone.Contains(note))
            {
                currentNotesInZone.Remove(note);
            }
        }
    }
}