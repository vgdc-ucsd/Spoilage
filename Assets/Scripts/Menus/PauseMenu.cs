using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PauseUI;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        GameIsPaused = false;
        PauseUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void LoadSettings()
    {
        // TODO
    }

    public void QuitButton()
    {
        GameManager.Instance.Load(GameScene.MAIN_MENU);
    }

    void Pause()
    {
        GameIsPaused = true;
        PauseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ReturnToMenu()
    {
        GameManager.Instance.Load(GameScene.MAIN_MENU);
    }
}
