using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : Singleton<UpgradeManager>
{
    [SerializeField] private UpgradeData[] _allUpgrades;

    void Start()
    {
        SaveManager.OnLoad(() => ApplyAllUpgrades());
    }

    private void ApplyAllUpgrades()
    {
        List<string> purchasedIDs = SaveManager.Instance.Player.Upgrades;
        if (purchasedIDs == null || purchasedIDs.Count == 0) return;

        CookingStation[] stations = FindObjectsByType<CookingStation>();

        foreach (string id in purchasedIDs)
        {
            UpgradeData upgrade = System.Array.Find(_allUpgrades, u => u.upgradeID == id);
            if (upgrade == null)
            {
                Debug.LogWarning($"UpgradeManager: No UpgradeData found for ID '{id}'.");
                continue;
            }
            ApplyToStations(upgrade, stations);
        }
    }

    private void ApplyToStations(UpgradeData upgrade, CookingStation[] stations)
    {
        foreach (CookingStation station in stations)
        {
            if (string.IsNullOrEmpty(upgrade.targetStation) || upgrade.targetStation == station.StationName)
                station.ApplyUpgrade(upgrade);
        }
    }
}
