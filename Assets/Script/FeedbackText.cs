using UnityEngine;
using UnityEngine.UI;

public class FeedbackText : MonoBehaviour
{
    public void Setup(string message, Color color)
    {
        Text txt = GetComponent<Text>();
        txt.text = message;
        txt.color = color;
        Destroy(gameObject, 0.5f);
    }
}