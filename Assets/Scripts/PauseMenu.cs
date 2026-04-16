using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PauseMenu : MonoBehaviour
{

    public static bool IsPaused = false;
    public GameObject PauseMenuUI;

    // Update is called once per frame
    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 0f;
        IsPaused = false;
    }

    private void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

}
