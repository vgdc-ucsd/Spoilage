using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : Singleton<PauseMenu>
{
    public static bool GameIsPaused = false;
    public GameObject PauseBar;
    public GameObject PauseUI;
    public GameObject SettingsUI;
    public GameObject SavesUI;

    public GameObject ResumeButton;
    public GameObject LoadButton;
    public GameObject SettingsButton;
    public GameObject MainMenuButton;

    void Start()
    {
        SettingsUI.SetActive(false);

        if (ResumeButton != null)
            ResumeButton.GetComponent<Button>().onClick.AddListener(Resume);

        if (LoadButton != null)
            MainMenuButton.GetComponent<Button>().onClick.AddListener(LoadSaves);

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

        if (Mouse.current.leftButton.wasPressedThisFrame)
            Debug.Log("mouse click");
    }
    
    public void Resume()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        PauseUI.SetActive(false);
        SettingsUI.SetActive(false);
        SavesUI.SetActive(false);
        PauseBar.SetActive(false);
    }

    public void LoadSettings()
    {
        Debug.Log("Settings clicked");
        PauseUI.SetActive(false);
        SettingsUI.SetActive(true);
        SavesUI.SetActive(false);
        // Scene settings = SceneManager.GetSceneByName("Settings");
        // if (settings.isLoaded)
        // {
        //     return;
        // }
        // SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
    }

    public void LoadSaves()
    {
        Debug.Log("Saves clicked");
        PauseUI.SetActive(false);
        SettingsUI.SetActive(false);
        SavesUI.SetActive(true);
    }

    public void QuitButton()
    {
        GameManager.Instance.Load(GameScene.MAIN_MENU);
    }

    void Pause()
    {
        PauseUI.SetActive(true);
        PauseBar.SetActive(true);

        GameIsPaused = true;
        Time.timeScale = 0f;
    }

    public void ReturnToMenu()
    {
        Debug.Log("Main menu clicked");
        GameManager.Instance.Load(GameScene.MAIN_MENU);
        //SceneLoader.Instance.ChangeScene("MainMenu");
        // removing scenes
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

