using UnityEngine;
#if DISCORD_SDK_ENABLED
using Discord.Sdk;
#endif

public class DiscordManager : Singleton<DiscordManager>
{
#if DISCORD_SDK_ENABLED
    private Client _client;
    private const long CLIENT_ID = 1550990869564498051;
    private const string LARGE_IMAGE_ID = "spoilage";

    void Start()
    {
        _client = new Client();
        _client.AddLogCallback(OnLog, LoggingSeverity.Error);
        _client.SetApplicationId(CLIENT_ID);
        SetStatus(0);
        SaveManager.OnPlayerLoad(() => SetStatus(SaveManager.Instance.Player.Day));
    }

    private void OnLog(string message, LoggingSeverity severity)
    {
        Debug.Log($"Discord Log: {severity} - {message}");
    }

    public void SetStatus(int day)
    {
        ActivityAssets activityAssets = new ActivityAssets();
        activityAssets.SetLargeImage(LARGE_IMAGE_ID);

        Activity activity = new Activity();
        activity.SetType(ActivityTypes.Playing);
        activity.SetAssets(activityAssets);

        if (day != 0) activity.SetDetails($"Day {SaveManager.Instance.Player.Day}");
        else activity.SetDetails(null);
        
        _client.UpdateRichPresence(activity, (ClientResult result) => {
            if (!result.Successful()) 
            {
                Debug.LogError($"Failed to update discord rich presence: {result.Error()}");
            }
        });
    }

    void OnDisable()
    {
        if (_client != null) _client.Dispose();
    }
#else
    public void SetStatus(int _) { }
#endif
}