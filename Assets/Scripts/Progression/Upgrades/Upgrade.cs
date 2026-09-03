using System.Collections.Generic;
using UnityEngine;

public enum UpgradeID
{
    // Shop Upgrades
    Spoiler,
    Unspoiler,
    GuardAllocation1,
    GuardAllocation2,
    GuardAllocation3,
    Overtime,
    CookingInstant,
    Distraction, 
    UnderTheTablePromotion,
    Streak1,
    Streak2,
    Streak3,
    SickDay,
    TipYouWaiter,
    GrillSpeed,
    GrillQuality,
    HotterGrill,
    AutoCuttingBoard,
    CuttingBoardQuality,
    CuttingBoardSpeed,
    PotSpeed,
    PotQuality,
    PotQuality2,
    OvenSpeed,
    OvenQuality,
    Preheat,
    AutoBlender,
    BlenderQuality,
    BlenderProductivity,

    // Progression Upgrades
    CuttingBoard,
    RootVeggies,
    Stage1Spoiled,
    RefuseButton,
    Cheese,
    Pot,
    Meat,
    Stage2Spoiled,
    Restaurant1,
    Potatoes,
    Oven,
    Sauce,
    Restaurant2,
    Blender,
    Restaurant3,
}

public enum UpgradeType
{
    Effect,
    Station,
    Restaurant,
    Ingredient,
}

[CreateAssetMenu(fileName = "Upgrade", menuName = "Progression/Upgrade")]
public class Upgrade : ScriptableObject
{
    [SerializeField] private UpgradeID _upgradeID;
    [SerializeField] private UpgradeType _upgradeType;
    [SerializeField] private string _name;
    [SerializeField, TextArea(3, 10)] private string _description;
    [SerializeField] private bool _defaultShopUpgrade;
    [SerializeField] private bool _temporary;
    [SerializeField] private List<Upgrade> _unlocks;
    [SerializeField] private int _cost;

    public UpgradeID UpgradeID => _upgradeID;
    public UpgradeType UpgradeType => _upgradeType;
    public string Name => _name;
    public string Description => _description;
    public bool DefaultShopUpgrade => _defaultShopUpgrade;
    public bool Temporary => _temporary;
    public List<Upgrade> Unlocks => _unlocks;
    public int Cost => _cost;
}