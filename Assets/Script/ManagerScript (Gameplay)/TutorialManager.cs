using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tutorialPanel;
    public Text tutorialText;
    public Image characterImage;
    public Image hintImage;
    public Image fishTutorImage;
    public Image tutorImageFrame;

    [Header("Data & References")]
    public TutorialData tutorialData;
    public PlayerStateManager playerManager;
    public AudioClip clickButton;

    private Queue<TutorialStep> _tutorialSteps = new Queue<TutorialStep>();

    IEnumerator Start()
    {
        yield return null;

        if (PlayerPrefs.GetInt("HasPlayedTutorial", 0) == 0)
        {
            StartTutorial();
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }

    public void StartTutorial()
    {
        if (tutorialData == null || tutorialData.steps.Length == 0) return;

        tutorialPanel.SetActive(true);
        TogglePlayerUI(false);

        _tutorialSteps.Clear();
        
        foreach (TutorialStep step in tutorialData.steps)
        {
            _tutorialSteps.Enqueue(step);
        }

        DisplayNextStep();
    }

    public void DisplayNextStep()
    {
        if (AudioManager.Instance != null && clickButton != null)
        {
            AudioManager.Instance.PlaySFX(clickButton);
        }

        if (_tutorialSteps.Count == 0)
        {
            EndTutorial();
            return;
        }

        TutorialStep currentStep = _tutorialSteps.Dequeue();
        
        tutorialText.text = currentStep.sentence;

        if (characterImage != null)
        {
            if (currentStep.characterSprite != null)
            {
                characterImage.sprite = currentStep.characterSprite;
                characterImage.gameObject.SetActive(true);
            }
            else
            {
                characterImage.gameObject.SetActive(false);
            }
        }

        if (hintImage != null)
        {
            if (currentStep.tutorialImage != null)
            {
                hintImage.sprite = currentStep.tutorialImage;
                hintImage.gameObject.SetActive(true);
            }
            else
            {
                hintImage.gameObject.SetActive(false);
            }
        }

        if (fishTutorImage != null)
        {
            if (currentStep.fishingTutorImage != null)
            {
                fishTutorImage.sprite = currentStep.fishingTutorImage;
                fishTutorImage.gameObject.SetActive(true);
                tutorImageFrame.gameObject.SetActive(true);
            }
            else
            {
                fishTutorImage.gameObject.SetActive(false);
                tutorImageFrame.gameObject.SetActive(false);
            }
        }
    }

    private void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        TogglePlayerUI(true);
        
        PlayerPrefs.SetInt("HasPlayedTutorial", 1);
        PlayerPrefs.Save();
    }

    private void TogglePlayerUI(bool status)
    {
        if (playerManager == null) return;

        if (playerManager.movementButtonsParent != null)
        {
            playerManager.movementButtonsParent.SetActive(status);
        }

        if (playerManager.ensiklopediaButton != null)
        {
            playerManager.ensiklopediaButton.SetActive(status);
        }

        if (playerManager.valueScoreText != null)
        {
            playerManager.valueScoreText.gameObject.SetActive(status);
        }

        if (playerManager.pauseButton != null)
        {
            playerManager.pauseButton.SetActive(status);
        }

        if (playerManager.fishingButton != null)
        {
            if (status)
            {
                playerManager.fishingButton.SetActive(playerManager.IsInFishingZone);
            }
            else
            {
                playerManager.fishingButton.SetActive(false);
            }
        }

        if (!status && playerManager.movementComponent != null)
        {
            playerManager.movementComponent.StopMoving();
        }
    }
}