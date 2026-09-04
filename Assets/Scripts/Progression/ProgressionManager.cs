using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : Singleton<ProgressionManager>
{
    [SerializeField] private UpgradeNode _upgradeRoot;
    [SerializeField] private List<Upgrade> _upgrades;
    
    private List<InteractionsNode> _interactionTimelines;
    private UpgradeNode _upgradeTimeline;

    public HashSet<UpgradeID> Unlocked = new HashSet<UpgradeID>();
    public List<UpgradeID> ShopPool { get; private set; }
    public List<UpgradeID> StationQueue { get; private set; }
    public Dictionary<UpgradeID, Upgrade> Upgrades { get; private set; }

    public override void Awake()
    {
        // TODO init from save script instead of awake and pass loaded data
        base.Awake();
        Init();
    }

    public void Init()
    {
        // TODO load ShopPool and StationQueue from save
        bool saveData = false;

        _upgradeTimeline = _upgradeRoot;
        _interactionTimelines = StoryManager.Instance.InitRunTimelineGraphs();

        ShopPool = new List<UpgradeID>();
        StationQueue = new List<UpgradeID>();
        Upgrades = new Dictionary<UpgradeID, Upgrade>();

        foreach (Upgrade upgrade in _upgrades)
        {
            Upgrades.Add(upgrade.UpgradeID, upgrade);

            if (!saveData && upgrade.DefaultShopUpgrade)
            {
                ShopPool.Add(upgrade.UpgradeID);
            }
        }

        // Unlock day 1 upgrades
        if (saveData == false)
        {    
            foreach (Upgrade upgrade in _upgradeTimeline.Data)
            {
                Unlock(upgrade.UpgradeID);
            }
        }
    }

    public void Unlock(UpgradeID id)
    {
        if (Unlocked.Contains(id)) return;
        
        Upgrade upgrade = Upgrades[id];
        foreach (Upgrade unlocked in upgrade.Unlocks)
        {
            ShopPool.Add(unlocked.UpgradeID);    
        }

        Unlocked.Add(id);

        // TODO
        switch (upgrade.UpgradeType)
        {
            case UpgradeType.Effect:
                break;
            case UpgradeType.Station:
                SaveManager.Instance.Player.PendingStation = upgrade.Name;
                break;
            case UpgradeType.Ingredient:
                break;
            case UpgradeType.Restaurant:
                break;
        }
    }

    public InteractionSet GetInteractions()
    {
        return new InteractionSet(_interactionTimelines, SaveManager.Instance.Player.Day);
    }

    public void AdvanceDay()
    {
        int day = SaveManager.Instance.Player.Day; 
        for (int i = 0; i < _interactionTimelines.Count; i++)
        {
            _interactionTimelines[i] = (InteractionsNode) _interactionTimelines[i]?.Advance(day);
        }
        
        _upgradeTimeline = (UpgradeNode) _upgradeTimeline?.Advance(day);
        if (_upgradeTimeline?.Day == day + 1)
        {    
            foreach (Upgrade upgrade in _upgradeTimeline.Data)
            {
                Unlock(upgrade.UpgradeID);
            }
        }

        SaveManager.Instance.Player.Day = day + 1;
        // TODO save
    }
}
