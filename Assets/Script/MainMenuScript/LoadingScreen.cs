using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingPanel;
    public Text loadingText;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingPanel.SetActive(true);
        Time.timeScale = 1f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float displayProgress = 0f;

        while (operation.progress < 0.9f || displayProgress < 1f)
        {
            displayProgress = Mathf.MoveTowards(displayProgress, 1f, Time.unscaledDeltaTime * 0.5f);
            loadingText.text = "Loading... " + (displayProgress * 100f).ToString("F0") + "%";
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.5f);
        operation.allowSceneActivation = true;
    }
}