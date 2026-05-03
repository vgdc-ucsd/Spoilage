using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public PlayerSaveData currentData;

    private string _saveFilePath;

    void Awake()
    {
        // Globally available and persistent across scenes; only one instance allowed
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _saveFilePath = Application.persistentDataPath + "/savefile.json";
            Debug.Log(_saveFilePath);
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(currentData);

        // Write the string to disk
        File.WriteAllText(_saveFilePath, json);
        Debug.Log("Game Saved to: " + _saveFilePath);
    }

    public void LoadGame()
    {
        if (File.Exists(_saveFilePath))
        {
            string json = File.ReadAllText(_saveFilePath);
            currentData = JsonUtility.FromJson<PlayerSaveData>(json);
        }
        else
        {
            // No save file exists, start fresh
            currentData = new PlayerSaveData();
        }
    }
}
