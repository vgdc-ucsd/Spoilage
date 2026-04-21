using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;


using UnityEngine.InputSystem;

// using monobehaviour cause only monobehavior can use coroutine 
public class SceneLoader : MonoBehaviour 
{

    public static SceneLoader Instance;
    private void Awake()
{
    if (Instance != null)
    {
        Destroy(gameObject);
        return;
    }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
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