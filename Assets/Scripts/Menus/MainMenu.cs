using System.IO;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        string path = Application.persistentDataPath + "/save.json";

        // FileStream file = new FileStream(path, FileMode.Open);
        PlayerData testSave = new PlayerData
        {
            resolutionWidth = 1920,
            resolutionHeight = 1080,
            volume = 10,
        };
        Debug.Log(
            string.Format(
                "{0}, {1}, {2}, {3}, {4}",
                testSave.progress,
                testSave.money,
                testSave.resolutionWidth,
                testSave.resolutionHeight,
                testSave.volume
            )
        );
        string testSaveJSON = JsonUtility.ToJson(testSave);
        Debug.Log(testSaveJSON);
        File.WriteAllText(path, testSaveJSON);

        Debug.Log(Screen.resolutions[0].width);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log(json);
            PlayerData save = JsonUtility.FromJson<PlayerData>(json);

            Debug.Log(
                string.Format(
                    "{0}, {1}, {2}, {3}, {4}",
                    save.progress,
                    save.money,
                    save.resolutionWidth,
                    save.resolutionHeight,
                    save.volume
                )
            );

            GameSaveManger.Instance.loadGame();
            Debug.Log(PlayerPrefManagfer.Instance);
            PlayerPrefManagfer.Instance.loadPrefs(
                save.resolutionWidth,
                save.resolutionHeight,
                save.volume
            );

            Debug.Log("Loaded save data");
        }
    }
}
