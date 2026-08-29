using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : Singleton<ProgressionManager>
{
    [SerializeField] private List<Upgrade> _upgrades;
    
    private List<InteractionsNode> _interactionTimelines;

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
    }

    public void Unlock(UpgradeID upgrade)
    {
        if (Unlocked.Contains(upgrade)) return;
        
        foreach (Upgrade unlocked in Upgrades[upgrade].Unlocks)
        {
            ShopPool.Add(unlocked.UpgradeID);    
        }

        Unlocked.Add(upgrade);
    }

    public InteractionSet GetInteractions()
    {
        return new InteractionSet(_interactionTimelines, SaveManager.Instance.Player.Day);
    }
}
