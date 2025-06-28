using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Drawing;

public class LoadingSceneManager : MonoBehaviour
{
    public static string mapScene;
    [SerializeField] Scrollbar progressBar;

    void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string Scene)
    {
        mapScene = Scene;
        SceneManager.LoadScene("LoadingScene");
    }
    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(mapScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                progressBar.size = Mathf.Lerp(progressBar.size, op.progress, timer);
                if (progressBar.size >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.size = Mathf.Lerp(progressBar.size, 1f, timer);
                if (progressBar.size == 1.0f)
                {
                    yield return new WaitForSeconds(0.5f);
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
