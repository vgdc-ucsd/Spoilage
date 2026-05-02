using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PauseUI;
    public GameObject SettingsScreen;

    [SerializeField]
    private Button _returnToMainMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _returnToMainMenu.onClick.AddListener(ReturnToMenu);
    }

    // Update is called once per frame
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
        Debug.Log(" da settings ");
        //switch scene to settings
        //SceneManager.LoadScene("SettingsScreen");
    }

    public void Quit()
    {
        Debug.Log(" buhbye ");
        Application.Quit();
    }

    void Pause()
    {
        GameIsPaused = true;
        PauseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ReturnToMenu()
    {
        Debug.Log("Return to main menu");
        SceneManager.LoadScene("Menu");
    }
}
