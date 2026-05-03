using System.Collections.Generic;
using UnityEngine;

public class LockLayout : Singleton<LockLayout>
{
    public static bool IsLocked = false;

    [SerializeField] private GameObject _lockButtonObject;

    private List<KitchenTile> _gridTiles;

    public override void Awake()
    {
        base.Awake();

        // Reset everything when the game starts
        IsLocked = false;
    }

    public void StartDay()
    {
        // 1. Flip the switches to TRUE
        IsLocked = true;
        FoodGrab.CanMoveFood = true;

        // 2. Hide the button so it can't be clicked again
        _lockButtonObject.SetActive(false);

        // set all empty tiles to have counters on them
        foreach (KitchenTile tile in _gridTiles)
        {
            tile.SetCountertopIfEmpty();
        }

        Debug.Log("Day Started: Layout is now PERMANENTLY locked.");
    }

    public void RegisterTile(KitchenTile tile)
    {
        if (_gridTiles == null) _gridTiles = new List<KitchenTile>();
        if (!_gridTiles.Contains(tile)) _gridTiles.Add(tile);
    }
}
