using System.Collections.Generic;
using UnityEngine;

public enum StationCategory
{
    CuttingBoard,
    Pot,
    Blender,
    Oven,
    Grill,
    SeasoningStation,
}

public class StationLookup : Singleton<StationLookup>
{
    [SerializeField] private List<StationData> _stations;

    public StationData NameToData(string name)
    {
        return _stations.Find(station => station.Name == name);
    }

    public static string CategoryName(StationCategory stationType)
    {
        switch (stationType)
        {
            case StationCategory.CuttingBoard:
                return "Cutting Board";
            case StationCategory.Pot:
                return "Pot";
            case StationCategory.Blender:
                return "Blender";
            case StationCategory.Oven:
                return "Oven";
            case StationCategory.Grill:
                return "Grill";
            case StationCategory.SeasoningStation:
                return "Seasoning Station";
            default:
                Debug.LogError("Invalid StationCategory");
                return null;
        }
    }
}
