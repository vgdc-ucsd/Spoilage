using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class SettingsManager : Singleton<SettingsManager>
{
    public bool isFullscreen = true;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    
    public void ToggleFullscreen()
    {
        isFullscreen = !isFullscreen;
        UnityEngine.Screen.fullScreen = isFullscreen;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}