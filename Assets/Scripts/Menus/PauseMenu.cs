using TMPro;
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
    [SerializeField] private TextMeshProUGUI _dayText;

    void Start()
    {
        if (SettingsUI != null)
            SettingsUI.SetActive(false);

        if (ResumeButton != null)
            ResumeButton.GetComponent<Button>().onClick.AddListener(Resume);

        if (LoadButton != null)
            LoadButton.GetComponent<Button>().onClick.AddListener(LoadSaves);

        if (SettingsButton != null)
            SettingsButton.GetComponent<Button>().onClick.AddListener(LoadSettings);

        if (MainMenuButton != null)
            MainMenuButton.GetComponent<Button>().onClick.AddListener(QuitButton);

        // play the click on the buttons themselves so Resume (also called by Escape) and ReturnToMenu don't double up
        foreach (GameObject button in new[] { ResumeButton, LoadButton, SettingsButton, MainMenuButton })
        {
            if (button != null)
                button.GetComponent<Button>().onClick.AddListener(PlayClick);
        }

        _masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
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
        Resume();
        GameManager.Instance.Load(GameScene.LOAD_SAVE);
    }

    private void PlayClick()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
    }

    public void QuitButton()
    {
        Resume();
        GameManager.Instance.Load(GameScene.MAIN_MENU);
    }

    void Pause()
    {
        AudioManager.Instance.PlaySFX("Pause");
        Background.SetActive(true);
        PauseUI.SetActive(true);
        PauseBar.SetActive(true);

        GameIsPaused = true;
        _dayText.text = $"Day {SaveManager.Instance.Player.Day}";
        _masterVolumeSlider.value = AudioManager.Instance.currentVolume;
        Time.timeScale = 0f;
    }

    public void UpdateMasterVolume(float volume)
    {
        AudioManager.Instance.SetVolume(volume);
    }
}

