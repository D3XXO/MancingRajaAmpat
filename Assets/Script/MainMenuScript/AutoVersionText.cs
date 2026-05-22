using UnityEngine;
using UnityEngine.UI;

public class AutoVersionText : MonoBehaviour
{
    private void Awake()
    {
        Text textComponent = GetComponent<Text>();

        if (textComponent != null)
        {
            textComponent.text = "v" + Application.version;
        }
    }
}