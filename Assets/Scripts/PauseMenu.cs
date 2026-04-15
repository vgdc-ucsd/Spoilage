using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PauseUI;
    public GameObject SettingsScreen;

    // Update is called once per frame
    void Update()
    {
        if( Keyboard.current.escapeKey.wasPressedThisFrame )
        {
            if( GameIsPaused )
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
        //invoke exit script or replace this with the exit script
    }

    void Pause()
    {
        GameIsPaused = true;
        PauseUI.SetActive(true);
        Time.timeScale = 0f;
    }
}
