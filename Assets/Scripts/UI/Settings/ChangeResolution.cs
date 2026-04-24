using UnityEngine;

public class ChangeResolution : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Resolution[] resolutions = GetResolutions();
        for (int i = 0; i < resolutions.Length; i++)
        {
            Debug.Log(resolutions[i]);
        }
    }


    public void SetRes(int height, int width, bool fullscreen)
    {
        Screen.SetResolution(height, width, fullscreen);
    }

    public Resolution[] GetResolutions()
    {
        Resolution[] res = Screen.resolutions;
        return res;
    }

    public void SetWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    public void SetFullscreen()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    }

    public void SetBorderlessFullscreen()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    public void SetResolution720p()
    {
        SetRes(720, 1280, Screen.fullScreen);
    }

    public void SetResolution1080p()
    {
        SetRes(1080, 1920, Screen.fullScreen);
    }

    public void SetResolution4K()
    {
        SetRes(2160, 3840, Screen.fullScreen);
    }
}
