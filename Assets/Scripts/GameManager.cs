using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameScene
{
    COOKING,
    SUMMARY,
    MAIN_MENU,
    SHOP
}

public class GameManager : Singleton<GameManager>
{
    public void StartGame()
    {
        // TODO: Setup Game, load save data, etc.
        //Load(GameScene.COOKING);
        SceneManager.LoadScene("Customer", LoadSceneMode.Additive);
        SceneManager.LoadScene("Cooking", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("MainMenu");
    }

    public void Load(GameScene scene)
    {
        switch (scene)
        {
            case GameScene.COOKING:
                SceneLoader.Instance.ChangeScene("Cooking");
                break;
            case GameScene.SUMMARY:
                SceneLoader.Instance.ChangeScene("Summary");
                break;
            case GameScene.MAIN_MENU:
                SceneLoader.Instance.ChangeScene("MainMenu");
                break;
            case GameScene.SHOP:
                SceneLoader.Instance.ChangeScene("Shop");
                break;
            default:
                Debug.LogError($"Scene {scene} not recognized or configured");
                break;
        }
    }

    public void Quit()
    {
        // TODO: Handle exit any additional exit processes
        Application.Quit();
    }
}
