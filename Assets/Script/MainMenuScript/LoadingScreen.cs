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
        GameObject canvasRoot = loadingPanel.transform.root.gameObject;
        Canvas canvas = canvasRoot.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 9999;
        }

        DontDestroyOnLoad(canvasRoot);

        GameObject scriptRoot = transform.root.gameObject;
        if (scriptRoot != canvasRoot)
        {
            DontDestroyOnLoad(scriptRoot);
        }

        StartCoroutine(LoadSceneAsync(sceneName, canvasRoot, scriptRoot));
    }

    IEnumerator LoadSceneAsync(string sceneName, GameObject canvasRoot, GameObject scriptRoot)
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

        operation.allowSceneActivation = true;

        yield return new WaitUntil(() => operation.isDone);
        yield return new WaitForSecondsRealtime(0.5f);

        Destroy(canvasRoot);
        
        if (scriptRoot != canvasRoot)
        {
            Destroy(scriptRoot);
        }
    }
}