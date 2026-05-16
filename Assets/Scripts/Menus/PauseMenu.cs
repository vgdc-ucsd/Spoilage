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
            Scene settings = SceneManager.GetSceneByName("Settings");
            if (settings.isLoaded)
            {
                SceneManager.UnloadSceneAsync("Settings");
                PauseUI.SetActive(true);
                return;
            }
            if (!GameIsPaused)
            {
                // stop from pausing in main menu
                Scene mainMenu = SceneManager.GetSceneByName("MainMenu");
                if (mainMenu.isLoaded)
                {
                    return;
                }
                

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
        Scene settings = SceneManager.GetSceneByName("Settings");
        if (settings.isLoaded)
        {
            return;
        }
        SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
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
        //SceneLoader.Instance.ChangeScene("MainMenu");
        Scene settings = SceneManager.GetSceneByName("Settings");
        if (settings.isLoaded)
        {
            SceneManager.UnloadSceneAsync("Settings");
        }
        Scene Cooking = SceneManager.GetSceneByName("Cooking");
        if (Cooking.isLoaded)
        {
            SceneManager.UnloadSceneAsync("Cooking");
        }
        Scene Customer = SceneManager.GetSceneByName("Customer");
        if (Customer.isLoaded)
        {
            SceneManager.UnloadSceneAsync("Customer");
        }
        Resume();
    }
}


