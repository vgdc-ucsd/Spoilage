using System;
using UnityEngine;

[Serializable]
public enum StationType
{
    CuttingBoard,
    Pot,
    Blender,
    Oven,
    Grill
}

public static class StationLookup
{
    public static string GetName(StationType stationType)
    {
        switch (stationType)
        {
            case StationType.CuttingBoard:
                return "Cutting Board";
            case StationType.Pot:
                return "Pot";
            case StationType.Blender:
                return "Blender";
            case StationType.Oven:
                return "Oven";
            case StationType.Grill:
                return "Grill";
            default:
                Debug.LogError("Invalid StationType");
                return null;
        }
    }
}
