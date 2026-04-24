using UnityEngine;
using UnityEngine.Audio;

public class PlayerPrefManagfer : Singleton<PlayerPrefManagfer>
{
    [SerializeField]
    private AudioMixer _volume;

    public void loadPrefs(int width, int height, bool fullScreen, float volume)
    {
        Screen.SetResolution(width, height, fullScreen);
        _volume.SetFloat("volume", volume);
        Debug.Log("Loaded player prefs");
    }
}
