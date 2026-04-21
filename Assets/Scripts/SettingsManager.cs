using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{

    

    public static SettingsManager Instance;

    public bool isFullscreen = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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