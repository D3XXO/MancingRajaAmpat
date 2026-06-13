using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RhythmManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform targetZoneRect;
    public PlayerStateManager stateManager;
    public GameObject feedbackTextPrefab;
    public GameObject qtePanel;
    public RectTransform feedbackPanel;
    public AudioClip perfectSfx;
    public AudioClip goodSfx;
    public List<RhythmNote> allActiveNotes = new List<RhythmNote>();

    [Header("Threshold Settings")]
    public float rightThreshold;
    public float leftThreshold;

    private bool _isQTEActive = false;
    private int _currentQTENoteID = -1;
    private GameObject _activeQTENoteObj;
    private Coroutine _qteCoroutine;

    private void OnDisable()
    {
        Time.timeScale = 1f;
        _isQTEActive = false;
        
        if (qtePanel != null) qtePanel.SetActive(false);
        if (_activeQTENoteObj != null) Destroy(_activeQTENoteObj);
        
        _qteCoroutine = null;
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

        if (_isQTEActive)
        {
            if (buttonID == _currentQTENoteID)
            {
                EndQTE(true);
            }
            else
            {
                EndQTE(false);
            }
            return;
        }

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

                if (targetDistance <= 20f) 
                {
                    stateManager.FishingState.ChangeProgress(0.05f);
                    TriggerPerfectQTE();
                }
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
            stateManager.FishingState.ChangeProgress(-0.05f);
            stateManager.TriggerShake(2.0f, 0.5f);
            ShowFeedback(999f);
        }
    }

    private void TriggerPerfectQTE()
    {
        if (!gameObject.activeInHierarchy) return;
        _isQTEActive = true;
        Time.timeScale = 0.05f;
        qtePanel.SetActive(true);

        GameObject[] prefabs = stateManager.rhythmSpawner.notePrefabs;
        GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Length)];

        _activeQTENoteObj = Instantiate(prefabToSpawn, feedbackPanel);
        
        RectTransform rt = _activeQTENoteObj.GetComponent<RectTransform>();
        
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        float randomX = Random.Range(-200f, 200f);
        float randomY = Random.Range(-200f, 200f);
        
        rt.anchoredPosition = new Vector2(randomX, randomY);
        rt.localScale = Vector3.one * 2.0f;

        RhythmNote noteComp = _activeQTENoteObj.GetComponent<RhythmNote>();
        if (noteComp != null)
        {
            _currentQTENoteID = noteComp.noteID;
            noteComp.enabled = false; 
        }

        _qteCoroutine = StartCoroutine(QTERoutine());
    }

    private IEnumerator QTERoutine()
    {
        float duration = 2.0f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 2.0f;
        Vector3 endScale = Vector3.zero;

        RectTransform rt = _activeQTENoteObj.GetComponent<RectTransform>();

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; 
            if (rt != null)
            {
                rt.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            }
            yield return null;
        }

        EndQTE(false);
    }

    private void EndQTE(bool isSuccess)
    {
        if (!_isQTEActive) return;

        _isQTEActive = false;
        Time.timeScale = 1f;
        qtePanel.SetActive(false);

        if (_qteCoroutine != null)
        {
            StopCoroutine(_qteCoroutine);
            _qteCoroutine = null;
        }

        if (_activeQTENoteObj != null)
        {
            Destroy(_activeQTENoteObj);
        }

        if (isSuccess)
        {
            stateManager.FishingState.ChangeProgress(0.025f);
            SpawnFeedbackText("NICE!", Color.cyan);
            
            if (AudioManager.Instance != null && perfectSfx != null)
            {
                AudioManager.Instance.PlayFeedback(perfectSfx);
            }
        }
    }

    public void ShowFeedback(float distance)
    {
        string message;
        Color color = Color.white;

        if (distance >= 999f)
        {
            Handheld.Vibrate();
            message = "HAHA!!";
            ColorUtility.TryParseHtmlString("#D36666", out color);
        }
        else if (distance <= 20f)
        {
            message = "PERFECT!";
            ColorUtility.TryParseHtmlString("#76A973", out color);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFeedback(perfectSfx);
            }
        }
        else if (distance <= 80f)
        {
            message = "WELL";
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
        
        go.GetComponent<FeedbackText>().Setup(message, color);
    }
}