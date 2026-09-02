using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] private Image _transition;
    private List<GameScene> _loadedScenes;
    private const float TRANSITION_TIME = 0.25f;

    private void Start()
    {
        _loadedScenes = new List<GameScene>();
        _transition.color = Color.clear;
        _transition.gameObject.SetActive(true);

        if (SceneManager.loadedSceneCount <= 1)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
            _loadedScenes.Add(GameScene.MAIN_MENU);
        }
        else
        {
            for (int i = 0; i < SceneManager.loadedSceneCount; i++)
            {
                GameScene scene = GetSceneEnum(SceneManager.GetSceneAt(i).name);
                if (scene != GameScene.PERSISTENT) _loadedScenes.Add(scene);
            }
        }
    }

    public void ChangeScene(GameScene scene)
    {
        List<GameScene> scenes = new List<GameScene>{scene};
        StartCoroutine(LoadSceneRoutine(scenes));
    }

    public void ChangeScene(List<GameScene> scenes)
    {
        StartCoroutine(LoadSceneRoutine(scenes));
    }

    private GameScene GetSceneEnum(string sceneName)
    {
        switch (sceneName)
        {
            case "Cooking":
                return GameScene.COOKING;
            case "Customer":
                return GameScene.CUSTOMER;
            case "Summary Screen":
                return GameScene.SUMMARY;
            case "MainMenu":
                return GameScene.MAIN_MENU;
            case "Shop":
                return GameScene.SHOP;
            case "IntroCutscene":
                return GameScene.INTRO_CUTSCENE;
            default:
                return GameScene.PERSISTENT;
        }
    }

    private string GetSceneName(GameScene scene)
    {
        switch (scene)
        {
            case GameScene.COOKING:
                return "Cooking";
            case GameScene.CUSTOMER:
                return "Customer";
            case GameScene.SUMMARY:
                return "Summary Screen";
            case GameScene.MAIN_MENU:
                return "MainMenu";
            case GameScene.SHOP:
                return "Shop";
            case GameScene.INTRO_CUTSCENE:
                return "IntroCutscene";
            default:
                Debug.LogError($"Scene {scene} not recognized or configured");
                return null;
        }
    }

    private AsyncOperation LoadScene(GameScene scene)
    {
        return SceneManager.LoadSceneAsync(GetSceneName(scene), LoadSceneMode.Additive);
    }

    private AsyncOperation UnloadScene(GameScene scene)
    {
        return SceneManager.UnloadSceneAsync(GetSceneName(scene));
    }

    private IEnumerator LoadSceneRoutine(List<GameScene> scenes)
    {   
        yield return BasicAnimations.Interpolate(
            null,
            (t) =>
            {
                float smooth = BasicAnimations.Smooth(t);
                _transition.color = new Color(0.0f, 0.0f, 0.0f, smooth);
            },
            () => _transition.color = Color.black,
            TRANSITION_TIME
        );
        
        List<AsyncOperation> loads = new List<AsyncOperation>();
        
        foreach (GameScene scene in _loadedScenes)
        {
            loads.Add(UnloadScene(scene));
        }
        
        foreach (GameScene scene in scenes)
        {
            loads.Add(LoadScene(scene));
        }

        _loadedScenes = scenes;
        
        bool loading = true;
        while (loading)
        {
            loading = false;
            foreach (AsyncOperation load in loads)
            {
                if (!load.isDone)
                {
                    loading = true;
                    break;
                }
            }

            yield return null;
        }

        yield return BasicAnimations.Interpolate(
            null,
            (t) =>
            {
                float smooth = BasicAnimations.Smooth(t);
                _transition.color = new Color(0.0f, 0.0f, 0.0f, 1.0f - smooth);
            },
            () => _transition.color = Color.clear,
            TRANSITION_TIME
        );
    }
}
