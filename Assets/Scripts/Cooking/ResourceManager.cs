public class ResourceManager : Singleton<ResourceManager>
{
    private int _reputation;
    private int _wealth;

    public int Reputation
    {
        get => _reputation; 
        set
        {
            ResourceUI.Instance.UpdateReputation(value);
            _reputation = value;
        }
    }

    public int Wealth
    {
        get => _wealth; 
        set
        {
            ResourceUI.Instance.UpdateWealth(value);
            _wealth = value;
        }
    }

    public void Load()
    {
        PlayerData data = SaveManager.Instance.Player;
        data.Reputation = Reputation;
        data.Wealth = Wealth;
    }

    public void Save()
    {
        PlayerData data = SaveManager.Instance.Player;
        data.Reputation = Reputation;
        data.Wealth = Wealth;
        SaveManager.Instance.SaveGame();
    }
}
