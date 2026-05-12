using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : Singleton<PauseMenu>
{
    public static bool GameIsPaused = false;
    public GameObject PauseUI;

    public GameObject ResumeButton;
    public GameObject SettingsButton;
    public GameObject MainMenuButton; 

    void Start()
    {
        if (ResumeButton != null)
            ResumeButton.GetComponent<Button>().onClick.AddListener(Resume);
        
        if (SettingsButton != null)
            SettingsButton.GetComponent<Button>().onClick.AddListener(LoadSettings);
        
        if (MainMenuButton != null)
            MainMenuButton.GetComponent<Button>().onClick.AddListener(ReturnToMenu);
        
    }
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!GameIsPaused)
            {
                UnityEngine.Debug.Log("Pause");
                
                //SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
                Pause();
            }
            else
            {
                UnityEngine.Debug.Log("Resume");
                
                //SceneManager.UnloadSceneAsync("PauseMenu");
                Resume();
            }
        }
    }

    public void Resume()
    {
        
        GameIsPaused = false;
        Time.timeScale = 1f;
        PauseUI.SetActive(false);
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
        PauseUI.SetActive(true);
        GameIsPaused = true;
        Time.timeScale = 0f;
    }

    public void ReturnToMenu()
    {

        GameManager.Instance.Load(GameScene.MAIN_MENU);
    }
}


