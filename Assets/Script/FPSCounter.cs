using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private static FPSCounter _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}