using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : Singleton<PauseMenu>
{
    public static bool GameIsPaused = false;
    public GameObject Background;
    public GameObject PauseBar;
    public GameObject PauseUI;
    public GameObject SettingsUI;
    //public GameObject SavesUI;

    public GameObject ResumeButton;
    public GameObject LoadButton;
    public GameObject SettingsButton;
    public GameObject MainMenuButton;

    [SerializeField] private Slider _masterVolumeSlider;

    void Start()
    {
        if (SettingsUI != null)
            SettingsUI.SetActive(false);

        if (ResumeButton != null)
            ResumeButton.GetComponent<Button>().onClick.AddListener(Resume);

        if (LoadButton != null)
            MainMenuButton.GetComponent<Button>().onClick.AddListener(LoadSaves);

        if (SettingsButton != null)
            SettingsButton.GetComponent<Button>().onClick.AddListener(LoadSettings);

        if (MainMenuButton != null)
            MainMenuButton.GetComponent<Button>().onClick.AddListener(ReturnToMenu);

        // play the click on the buttons themselves so Resume (also called by Escape) and ReturnToMenu don't double up
        foreach (GameObject button in new[] { ResumeButton, LoadButton, SettingsButton, MainMenuButton })
        {
            if (button != null)
                button.GetComponent<Button>().onClick.AddListener(PlayClick);
        }

        _masterVolumeSlider.value = AudioManager.Instance.currentVolume;
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!GameIsPaused)
            {
                // stop from pausing in main menu
                Scene mainMenu = SceneManager.GetSceneByName("MainMenu");
                if (mainMenu.isLoaded)
                {
                    return;
                }

                Pause();
            }
            else
            {
                Resume();
            }
        }
    }
    
    public void Resume()
    {
        AudioManager.Instance.PlaySFX("Unpause");
        GameIsPaused = false;
        Time.timeScale = 1f;
        Background.SetActive(false);
        PauseUI.SetActive(false);
        SettingsUI.SetActive(false);
        //SavesUI.SetActive(false);
        PauseBar.SetActive(false);
    }

    public void LoadSettings()
    {
        PauseUI.SetActive(false);
        SettingsUI.SetActive(true);
        //SavesUI.SetActive(false);
    }

    public void LoadSaves()
    {
        PauseUI.SetActive(false);
        SettingsUI.SetActive(false);
        //SavesUI.SetActive(true);
    }

    private void PlayClick()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
    }

    public void QuitButton()
    {
        PlayClick();
        GameManager.Instance.Load(GameScene.MAIN_MENU);
    }

    void Pause()
    {
        AudioManager.Instance.PlaySFX("Pause");
        Background.SetActive(true);
        PauseUI.SetActive(true);
        PauseBar.SetActive(true);

        GameIsPaused = true;
        Time.timeScale = 0f;
    }

    public void ReturnToMenu()
    {
        GameManager.Instance.Load(GameScene.MAIN_MENU);

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

    public void UpdateMasterVolume()
    {
        AudioManager.Instance.SetVolume(_masterVolumeSlider.value);
    }
}

