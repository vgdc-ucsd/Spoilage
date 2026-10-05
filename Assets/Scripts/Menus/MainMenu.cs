using UnityEngine;

public class MainMenu : MonoBehaviour
{   
    [SerializeField] private GameObject _continueButton;

    public void Start()
    {
        AudioManager.Instance.PlayMusicEntry("Title");
        _continueButton.SetActive(false);
        
        if (SaveManager.Instance.LoadSaveOverviews()?.SaveOverviews.Count > 0)
        {
            _continueButton.SetActive(true);
        }
    }

    public void ClickStartGame()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        GameManager.Instance.Load(GameScene.INTRO_CUTSCENE);
    }

    public void ClickContinue()
    {
        GameManager.Instance.Load(GameScene.LOAD_SAVE);
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
