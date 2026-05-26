using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void ClickStartGame()
    {
        GameManager.Instance.Load(GameScene.INTRO_CUTSCENE);
    }

    public void ClickSettings()
    {
        // TODO
    }

    public void ClickExitGame()
    {
        GameManager.Instance.Quit();
    }

    public void PlaySFX(string id)
    {
        AudioManager.Instance.PlaySFX(id);
    }
}
