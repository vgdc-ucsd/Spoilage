using System.Collections.Generic;
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
        SceneLoader.Instance.ChangeScene(scene);
    }
    
    public void Quit()
    {
        // TODO: Handle exit any additional exit processes
        Application.Quit();
    }
}
