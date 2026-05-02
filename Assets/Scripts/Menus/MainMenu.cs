using System.IO;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        string path = Application.persistentDataPath + "/save.json";

        // for testing, delete
        PlayerData testSave = new PlayerData
        {
            resolution = new Resolution { width = 1920, height = 1080 },
            volume = 10,
        };
        Debug.Log(
            string.Format(
                "{0}, {1}, {2}, {3}, {4}",
                testSave.progress,
                testSave.money,
                testSave.resolution.width,
                testSave.resolution.height,
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

            // debugging
            Debug.Log(
                string.Format(
                    "{0}, {1}, {2}, {3}, {4}",
                    save.progress,
                    save.money,
                    save.resolution.width,
                    save.resolution.height,
                    save.volume
                )
            );

            SaveManager.Instance.LoadGame();
            Debug.Log(PlayerPrefManagfer.Instance);
            PlayerPrefManagfer.Instance.loadPrefs(
                save.resolution.width,
                save.resolution.height,
                true,
                save.volume
            );

            Debug.Log("Loaded save data");
        }
    }
}
