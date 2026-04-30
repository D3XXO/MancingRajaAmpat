using UnityEngine;

public class BGMSetter : MonoBehaviour
{
    public AudioClip sceneBGM;

    void Start()
    {
        if (AudioManager.Instance != null && sceneBGM != null)
        {
            AudioManager.Instance.PlayBGM(sceneBGM);
        }
    }
}