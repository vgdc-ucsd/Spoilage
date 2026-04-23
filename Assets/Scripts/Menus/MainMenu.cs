using System;
using System.IO;
using NUnit.Framework.Internal;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Awake()
    {
        string path = Application.persistentDataPath + "/save.json";

        // FileStream file = new FileStream(path, FileMode.Open);
        SaveDataFormat testSave = new SaveDataFormat
        {
            resolutionWidth = 1920,
            resolutionHeight = 1080,
            fullScreen = true,
            volume = 10,
        };
        Debug.Log(
            string.Format(
                "{0}, {1}, {2}, {3}",
                testSave.resolutionWidth,
                testSave.resolutionHeight,
                testSave.fullScreen,
                testSave.volume
            )
        );
        string testSaveJSON = JsonUtility.ToJson(testSave);
        Debug.Log(testSaveJSON);
        File.WriteAllText(path, testSaveJSON);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log(json);
            SaveDataFormat save = JsonUtility.FromJson<SaveDataFormat>(json);

            GameSaveManger.Instance.loadGame();
            PlayerPrefManagfer.Instance.loadPrefs(
                save.resolutionWidth,
                save.resolutionHeight,
                save.fullScreen,
                save.volume
            );

            Debug.Log("Loaded save data");
        }
    }
}
