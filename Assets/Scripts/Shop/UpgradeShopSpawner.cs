using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Placed once in the shop scene.
/// On SpawnUpgrades() it picks up to three upgrades the player can still buy,
/// instantiates an Upgrade Item prefab at each slot, and configures it.
///
/// Call order (guaranteed by ShopManager.InitializeShop):
///   1. UpgradeManager.ClearTemporaryUpgrades()   — wipes last session's temp upgrades
///   2. UpgradeShopSpawner.SpawnUpgrades()         — filters with clean state
/// </summary>
public class UpgradeShopSpawner : MonoBehaviour
{
    [Tooltip("Every UpgradeData asset in the game.")]
    [SerializeField] private UpgradeData[] _allUpgrades;

    [Tooltip("Prefab with UpgradeItemScript. Instantiated once per slot.")]
    [SerializeField] private GameObject _upgradeItemPrefab;

    [Tooltip("Three world-space transforms that mark where upgrade items appear.")]
    [SerializeField] private Transform[] _slots;

    // -----------------------------------------------------------------------

    /// <summary>Called by ShopManager after ClearTemporaryUpgrades.</summary>
    public void SpawnUpgrades()
    {
        if (_upgradeItemPrefab == null)
        {
            Debug.LogError("UpgradeShopSpawner: _upgradeItemPrefab is not assigned.");
            return;
        }

        PlayerData player = SaveManager.Instance.Player;

        // --- Build the pool of upgrades available to buy this visit ---
        List<UpgradeData> pool = new();
        foreach (UpgradeData upgrade in _allUpgrades)
        {
            if (upgrade == null) continue;

            // Permanent upgrades already owned — skip forever
            if (player.Upgrades.Contains(upgrade.upgradeID)) continue;

            // Temporary upgrades are cleared on shop entry, so they always re-appear.
            // (player.TemporaryUpgrades is empty at this point)

            // Prerequisites must be permanently owned
            if (!PermanentPrerequisitesMet(upgrade, player)) continue;

            pool.Add(upgrade);
        }

        // --- Shuffle in-place (Fisher-Yates) ---
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        // --- Spawn one per slot, up to three ---
        int count = Mathf.Min(_slots.Length, pool.Count);
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(
                _upgradeItemPrefab,
                _slots[i].position,
                _slots[i].rotation
            );
            go.GetComponent<UpgradeItemScript>().Initialize(pool[i]);
        }
    }

    // -----------------------------------------------------------------------

    private static bool PermanentPrerequisitesMet(UpgradeData upgrade, PlayerData player)
    {
        if (upgrade.prerequisites == null || upgrade.prerequisites.Length == 0) return true;
        foreach (string prereqID in upgrade.prerequisites)
            if (!player.Upgrades.Contains(prereqID)) return false;
        return true;
    }
}
