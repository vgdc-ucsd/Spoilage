using UnityEngine;

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
        Load(GameScene.COOKING);
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
