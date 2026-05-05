using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
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
     
        float minWaitTime = 1.0f;
        float startTime = Time.time;
        // load the loading screen
        // additive load scene as specified in testing read me. 
        yield return SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);

        // https://docs.unity3d.com/ScriptReference/AsyncOperation.html
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        

        op.allowSceneActivation = false;

        while (op.progress < 0.9f || (Time.time - startTime) < minWaitTime){
            float progress = op.progress;
            //UnityEngine.Debug.Log($"loading scene {sceneName} {progress}" );

            yield return null;
        }

        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync("LoadingScreen");
    }

    private IEnumerator UnloadSceneRoutine(string sceneName)
    {
        float minWaitTime = 1.0f;
        float startTime = Time.time;

        // https://docs.unity3d.com/ScriptReference/AsyncOperation.html
        AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);
        // load the loading screen
        // additive load scene as specified in testing read me. 
        yield return SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);

        

        op.allowSceneActivation = false;

        while (op.progress < 0.9f || (Time.time - startTime) < minWaitTime){
            float progress = op.progress;
            //UnityEngine.Debug.Log($"loading scene {sceneName} {progress}" );

            yield return null;
        }

        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync("LoadingScreen");
    
    }
}
