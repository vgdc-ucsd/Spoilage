using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] private float _minWaitTime = 0.0f;

    private void Start()
    {
        if (SceneManager.loadedSceneCount <= 1)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    public void UnloadScene(string sceneName) 
    {
        StartCoroutine(UnloadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {   
        float startTime = Time.time;

        // TODO: Scene transition, loading screen

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        
        op.allowSceneActivation = false;
        yield return new WaitWhile(() => op.progress < 0.9f || (Time.time - startTime) < _minWaitTime);
        op.allowSceneActivation = true;

        yield return new WaitUntil(() => op.isDone);
    }

    private IEnumerator UnloadSceneRoutine(string sceneName)
    {
        float startTime = Time.time;

        AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);

        op.allowSceneActivation = false;
        yield return new WaitWhile(() => op.progress < 0.9f || (Time.time - startTime) < _minWaitTime);
        op.allowSceneActivation = true;

        yield return new WaitUntil(() => op.isDone);
    }
}
