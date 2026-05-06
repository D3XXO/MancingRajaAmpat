using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}