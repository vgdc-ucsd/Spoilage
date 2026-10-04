using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Start()
    {
        AudioManager.Instance.PlayMusicEntry("Title");
    }

    public void ClickStartGame()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        GameManager.Instance.Load(GameScene.INTRO_CUTSCENE);
    }

    public void ClickSettings()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
    }

    public void ClickExitGame()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        GameManager.Instance.Quit();
    }

    public void PlaySFX(string id)
    {
        AudioManager.Instance.PlaySFX(id);
    }
}
