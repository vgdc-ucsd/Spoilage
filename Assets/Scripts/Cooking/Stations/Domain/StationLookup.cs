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

public static class StationLookup
{
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
