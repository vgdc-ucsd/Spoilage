using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class SaveManager : Singleton<SaveManager>
{
    public PlayerData Player;
    public SettingsData Settings;
    private string _playerSavePath;
    private string _settingsSavePath;
    private static Queue<Action> s_loadQueue = new Queue<Action>();

    void Start()
    {
        _playerSavePath = Application.persistentDataPath + "/savefile.json";
        _settingsSavePath = Application.persistentDataPath + "/settings.json";

        LoadAll();
        
        while (s_loadQueue.Count > 0)
        {
            s_loadQueue.Dequeue()?.Invoke();
        }
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(Player);

        // Write the string to disk
        File.WriteAllText(_playerSavePath, json);
        Debug.Log("Game Saved to: " + _playerSavePath);
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(Settings);
        File.WriteAllText(_settingsSavePath, json);
    }

    public void LoadPlayer()
    {
        if (File.Exists(_playerSavePath))
        {
            string json = File.ReadAllText(_playerSavePath);
            Player = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            // No save file exists, start fresh
            Player = new PlayerData();
        }
    }

    public void LoadSettings()
    {
        if (File.Exists(_settingsSavePath))
        {
            string json = File.ReadAllText(_settingsSavePath);
            Settings = JsonUtility.FromJson<SettingsData>(json);
        }
        else
        {
            // No save file exists, start fresh
            Settings = new SettingsData();
        }
    }

    public void LoadAll()
    {
        LoadPlayer();
        LoadSettings();
    }

    public static void OnLoad(Action action)
    {
        if (Instance == null)
        {
            s_loadQueue.Enqueue(action);
            return;
        }

        action?.Invoke();
    }
}
