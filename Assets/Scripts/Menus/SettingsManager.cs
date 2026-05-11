using UnityEngine;

public class SettingsManager : Singleton<SettingsManager>
{
    private SettingsData _settings; 

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        SaveManager.OnLoad(() => InitializeSettings());
    }

    public void SetFullscreen(FullScreenMode mode)
    {
        _settings.Fullscreen = mode;
        Screen.fullScreenMode = mode;
    }

    public void InitializeSettings()
    {
        _settings = SaveManager.Instance.Settings;
        Screen.SetResolution(_settings.Res.width, _settings.Res.height, _settings.Fullscreen);
        // TODO: Load volume
    }
}