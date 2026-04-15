using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;


using UnityEngine.InputSystem;


// using monobehaviour cause only monobehavior can use coroutine 
public class SceneLoader : MonoBehaviour 
{
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }
    // IEnumerator forward only iterator
    private IEnumerator LoadSceneRoutine(string sceneName)
    {   
        // min wait time to get rid of flickering
        // if we have a loading animation, probably better 
        // to not have the time hardcoded into this file but
        // based on some variable in that scene
        // this works for now though
        float minWaitTime = 1.0f;
        float startTime = Time.time;
        // load the loading screen
        // additive load scene as specified in testing read me. 
        yield return SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);

        // https://docs.unity3d.com/ScriptReference/AsyncOperation.html
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        
        // dont load scene
        op.allowSceneActivation = false;
        // https://docs.unity3d.com/ScriptReference/AsyncOperation-progress.html
        // op.progress hits max 0.9 when allowSceneActivation is false

        while (op.progress < 0.9f || (Time.time - startTime) < minWaitTime){
            float progress = op.progress;
            UnityEngine.Debug.Log($"loading scene {sceneName} {progress}" );

            yield return null;
        }

        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync("LoadingScreen");
    }
    
    void Update()
    {
        // this one ; so its hard to press by accident
        if (Keyboard.current.semicolonKey.wasPressedThisFrame)
        {
            UnityEngine.Debug.Log("Loading Screen Test");
            ChangeScene("LoadingTest");
        }
    }
}
