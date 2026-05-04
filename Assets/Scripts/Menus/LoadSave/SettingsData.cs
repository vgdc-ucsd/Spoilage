using System;
using UnityEngine;

[Serializable]
public class SettingsData
{
    public float Volume;
    public FullScreenMode Fullscreen;
    public Resolution Res;

    public SettingsData()
    {
        Volume = 0.75f;
        Fullscreen = FullScreenMode.ExclusiveFullScreen;
        Res = new Resolution
        {
            width = 1920,
            height = 1080
        };
    }
}
