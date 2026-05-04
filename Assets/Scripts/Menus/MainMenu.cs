using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void ClickStartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void ClickSettings()
    {
        // TODO
    }

    public void ClickExitGame()
    {
        GameManager.Instance.Quit();
    }
}
