using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Central upgrade registry. Lives in the kitchen scene.
/// Call HasUpgrade(id) anywhere to gate upgrade behaviour.
/// Call modifier getters (GetSpoilRateMultiplier, etc.) from the relevant systems.
/// </summary>
public class UpgradeManager : Singleton<UpgradeManager>
{
    [Tooltip("Assign every UpgradeData asset here so the manager can look them up by ID.")]
    [SerializeField] private UpgradeData[] _allUpgrades;

    // -----------------------------------------------------------------------
    // Lifecycle
    // -----------------------------------------------------------------------

    void Start()
    {
        SaveManager.OnLoad(() => ApplyAllUpgrades());
    }

    private void ApplyAllUpgrades()
    {
        PlayerData player = SaveManager.Instance.Player;

        // Activate InstantCooking uses from freshly-purchased temp upgrade
        if (player.TemporaryUpgrades.Contains("cooking_instant") &&
            player.InstantCookingUsesRemaining == 0)
        {
            UpgradeData inst = FindUpgradeData("cooking_instant");
            if (inst != null)
                player.InstantCookingUsesRemaining = Mathf.RoundToInt(inst.value);
        }

        // Apply station-level generic effects (SpeedBoost, CapacityIncrease, QualityBonus)
        CookingStation[] stations = FindObjectsByType<CookingStation>();
        foreach (string id in player.Upgrades)
            ApplyToStations(FindUpgradeData(id), stations);
        foreach (string id in player.TemporaryUpgrades)
            ApplyToStations(FindUpgradeData(id), stations);

        // Apply guard power boost
        int guardBoost = GetGuardPowerBoost();
        if (guardBoost > 0)
        {
            GuardsStaminaBar bar = FindAnyObjectByType<GuardsStaminaBar>();
            if (bar != null)
                bar.setMaxPresses(bar.maxPresses + guardBoost);
        }
    }

    private void ApplyToStations(UpgradeData upgrade, CookingStation[] stations)
    {
        if (upgrade == null) return;
        foreach (CookingStation station in stations)
        {
            if (string.IsNullOrEmpty(upgrade.targetStation) ||
                upgrade.targetStation == station.StationName)
            {
                station.ApplyUpgrade(upgrade);
            }
        }
    }

    /// <summary>
    /// Called by ShopManager when entering the shop after a kitchen day.
    /// Clears one-day temporary upgrades so they don't carry over.
    /// </summary>
    public static void ClearTemporaryUpgrades()
    {
        if (SaveManager.Instance == null) return;
        PlayerData player = SaveManager.Instance.Player;
        player.TemporaryUpgrades.Clear();
        player.InstantCookingUsesRemaining = 0;
    }

    // -----------------------------------------------------------------------
    // Query helpers
    // -----------------------------------------------------------------------

    /// <summary>True if the upgrade is permanently owned OR active as a temporary upgrade this day.</summary>
    public bool HasUpgrade(string id)
    {
        PlayerData p = SaveManager.Instance.Player;
        return p.Upgrades.Contains(id) || p.TemporaryUpgrades.Contains(id);
    }

    public UpgradeData FindUpgradeData(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return System.Array.Find(_allUpgrades, u => u != null && u.upgradeID == id);
    }

    // -----------------------------------------------------------------------
    // Global modifier getters — call these from the relevant game systems
    // -----------------------------------------------------------------------

    /// <summary>Multiplier applied to ingredient spoil rate. Hook: IngredientBehaviour.HandleSpoilage.</summary>
    public float GetSpoilRateMultiplier()
    {
        if (HasUpgrade("spoiler"))   return 2.0f;
        if (HasUpgrade("unspoiler")) return 0.5f;
        return 1.0f;
    }

    /// <summary>Total flat increase to guard max presses from all Guard Allocation upgrades.</summary>
    public int GetGuardPowerBoost()
    {
        int total = 0;
        if (HasUpgrade("guard_alloc_1")) total += 1;
        if (HasUpgrade("guard_alloc_2")) total += 2;
        if (HasUpgrade("guard_alloc_3")) total += 3;
        return total;
    }

    /// <summary>
    /// Multiplier on day duration. Hook: your day-timer system.
    /// Returns 1.2 when Overtime is active, 1.0 otherwise.
    /// </summary>
    public float GetDayDurationMultiplier() => HasUpgrade("overtime") ? 1.2f : 1.0f;

    /// <summary>
    /// Multiplier on patience drain rate. Hook: wherever PatienceBar.beginDecay is called.
    /// Returns 0.75 when Distraction is active, 1.0 otherwise.
    /// </summary>
    public float GetPatienceDecayMultiplier() => HasUpgrade("distraction") ? 0.75f : 1.0f;

    /// <summary>
    /// Flat increase to the player's reputation cap. Hook: wherever max reputation is enforced.
    /// Returns 25 (arbitrary) per Under the Table Promotion owned; stacks if bought multiple times.
    /// </summary>
    public int GetReputationLimitBonus()
    {
        int count = 0;
        foreach (string id in SaveManager.Instance.Player.Upgrades)
            if (id == "under_table_promotion") count++;
        return count * 25;
    }

    /// <summary>
    /// Number of perfect dishes between streak rewards (0 = no streak system active).
    /// Returns the tightest purchased tier: Streak3→1, Streak2→3, Streak1→6.
    /// </summary>
    public int GetStreakInterval()
    {
        if (HasUpgrade("streak_3")) return 1;
        if (HasUpgrade("streak_2")) return 3;
        if (HasUpgrade("streak_1")) return 6;
        return 0;
    }

    /// <summary>
    /// Fractional reduction to money lost on failed/incomplete dishes.
    /// Returns 0.5 (50% less loss) when Sick Day is active. Hook: dish-fail payment logic.
    /// </summary>
    public float GetFailMoneyReductionMultiplier() => HasUpgrade("sick_day") ? 0.5f : 1.0f;

    /// <summary>
    /// Fraction of remaining patience paid as a tip bonus when Tip You Waiter is active.
    /// Returns 0 when inactive. Hook: customer satisfaction/payment code.
    /// </summary>
    public float GetPatienceTipFraction() => HasUpgrade("tip_waiter") ? 0.1f : 0f;

    /// <summary>
    /// Unspoiled spawn threshold used by CustomerManager.GenerateCustomerData.
    /// Base is 0.7. Following Protocol raises it (fewer spoiled). Breaking Protocol lowers it (more spoiled).
    /// </summary>
    public float GetUnspoiledCustomerThreshold()
    {
        if (HasUpgrade("breaking_protocol")) return 0.60f; // spoiled chance ~40%
        if (HasUpgrade("following_protocol")) return 0.80f; // spoiled chance ~20%
        return 0.70f;
    }

    // -----------------------------------------------------------------------
    // Instant cooking (Cooking Instant temp upgrade)
    // -----------------------------------------------------------------------

    /// <summary>
    /// Tries to consume one instant-cook charge. Returns true and decrements if charges remain.
    /// Hook: AutomaticStation and ManualStation can call this when food is placed.
    /// </summary>
    public bool TryUseInstantCooking()
    {
        PlayerData p = SaveManager.Instance.Player;
        if (p.InstantCookingUsesRemaining <= 0) return false;
        p.InstantCookingUsesRemaining--;
        return true;
    }
}
