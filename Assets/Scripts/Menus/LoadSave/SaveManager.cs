using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class SaveManager : Singleton<SaveManager>
{
    public PlayerData Player;
    public SettingsData Settings;

    private string _saveFolderPath;
    private string _settingsSavePath;

    private static Queue<Action> s_loadQueue = new Queue<Action>();

    private const int SAVE_SLOT_COUNT = 128;
    private const string SAVE_FOLDER = "saves";

    private string GetSlotPath(int saveId)
    {
        return Path.Combine(_saveFolderPath, $"save_{saveId:D3}.json");
    }

    private bool IsValidSaveId(int saveId)
    {
        return saveId >= 1 && saveId <= SAVE_SLOT_COUNT;
    }

    public bool SaveExists(int saveId)
    {
        return IsValidSaveId(saveId) && File.Exists(GetSlotPath(saveId));
    }

    public void Start()
    {
        _saveFolderPath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER);
        _settingsSavePath = Path.Combine(Application.persistentDataPath, "/settings.json");
        Player = DebugManager.Instance.DebugPlayerSave?.Clone();
        LoadSettings();
    }

    public void SaveGame(int saveId)
    {
        if (!IsValidSaveId(saveId))
        {
            Debug.LogError($"Invalid save ID: {saveId}");
            return;
        }

        if (Player == null)
        {
            Player = new PlayerData();
        }

        Directory.CreateDirectory(_saveFolderPath);

        Player.SaveID = saveId;

        string json = JsonUtility.ToJson(Player, true);

        File.WriteAllText(GetSlotPath(saveId), json);
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(Settings);
        File.WriteAllText(_settingsSavePath, json);
    }

    public void LoadPlayer(int saveId)
    {
        string path = GetSlotPath(saveId);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Player = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            Player = new PlayerData
            {
                SaveID = saveId,
                SaveName = $"Save {saveId}"
            };
        }

        while (s_loadQueue.Count > 0)
        {
            s_loadQueue.Dequeue()?.Invoke();
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

    private int GetNextAvailableSaveId()
    {
        for (int saveId = 1; saveId <= SAVE_SLOT_COUNT; saveId++)
        {
            if (!SaveExists(saveId))
            {
                return saveId;
            }
        }

        return -1;
    }

    public void SaveToNew()
    {
        int saveId = GetNextAvailableSaveId();

        if (saveId == -1)
        {
            Debug.LogError("No empty save slots available.");
            return;
        }

        Player = new PlayerData
        {
            SaveID = saveId,
            SaveName = $"Save {saveId}"
        };

        SaveGame(saveId);
    }

    public static void OnPlayerLoad(Action action)
    {
        if (Instance == null || Instance.Player == null)
        {
            s_loadQueue.Enqueue(action);
            return;
        }

        action?.Invoke();
    }

    public void RenameSave(int saveId, string newName)
    {
        LoadPlayer(saveId);
        Player.SaveName = newName;
        SaveGame(saveId);
    }

    public void DeleteSave(int saveId)
    {
        if (!SaveExists(saveId))
        {
            return;
        }

        File.Delete(GetSlotPath(saveId));
    }
}
