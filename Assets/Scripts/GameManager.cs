using UnityEngine;

public enum GameScene
{
    COOKING,
    CUSTOMER,
    SUMMARY,
    MAIN_MENU,
    SHOP,
    INTRO_CUTSCENE,
    PERSISTENT,
    GAME_OVER,
}

public class GameManager : Singleton<GameManager>
{
    public void StartGame()
    {
        // TODO: Setup Game, load save data, etc.
        Load(GameScene.COOKING);
    }

    public void GameOver(GameOverCondition condition)
    {
        SaveManager.Instance.Player.GameOverCondition = condition;
        Load(GameScene.GAME_OVER);
    }

    public void Load(GameScene scene)
    {
        SceneLoader.Instance.ChangeScene(scene);
    }
    
    public void Quit()
    {
        // TODO: Handle exit any additional exit processes
        Application.Quit();
    }
}
