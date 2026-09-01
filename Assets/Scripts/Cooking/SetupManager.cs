using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum GamePhase
{
    Setup,
    Cooking
}

public class SetupManager : Singleton<SetupManager>
{
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Setup;

    [SerializeField] private CallBell _callBell;
    [SerializeField] private StartSign _startSign;
    [SerializeField] private DayTimer _dayTimer;
    [SerializeField] private StationUnlockPopup _stationUnlockPopup;
    [SerializeField] private StationData _grill;
    [SerializeField] private PlatingTileUI _platingTile;
    [SerializeField] private List<SpawnerTileUI> _spawnerTiles;
    [SerializeField] private List<KitchenTileUI> _defaultTiles;
    [SerializeField] private List<KitchenTileUI> _upgrade1Tiles;
    [SerializeField] private List<KitchenTileUI> _upgrade2Tiles;
    [SerializeField] private List<KitchenTileUI> _upgrade3Tiles;
    private List<TileUI> _allTiles;

    private void Start()
    {
        AudioManager.Instance.PlayMusicEntry("KitchenLayout");
        _stationUnlockPopup.gameObject.SetActive(false);

        _allTiles = new List<TileUI>();
        List<KitchenTileUI> kitchenTileUIs = new List<KitchenTileUI>();

        kitchenTileUIs.AddRange(_defaultTiles);
        kitchenTileUIs.AddRange(_upgrade1Tiles);
        kitchenTileUIs.AddRange(_upgrade2Tiles);
        kitchenTileUIs.AddRange(_upgrade3Tiles);
        
        _allTiles.Add(_platingTile);
        _allTiles.AddRange(kitchenTileUIs);
        _allTiles.AddRange(_spawnerTiles);

        _callBell.Lock(true);
        _startSign.Lock(true);
        LockTiles(_allTiles, true);

        _platingTile.Init();

        foreach (KitchenTileUI ui in kitchenTileUIs)
        {
            ui.Init();
        }

        foreach (SpawnerTileUI ui in _spawnerTiles)
        {
            ui.Init();
        }

        List<ITemporalTile> temporalTiles = kitchenTileUIs.Select(ui => ui.Tile as ITemporalTile).ToList();
        temporalTiles.Add(_platingTile.PlatingTile);
        CookingManager.Instance.SetTiles(temporalTiles, _platingTile.PlatingTile);        

        CustomerLineManager.Instance.GenerateQueues();
        CustomerLineManager.Instance.SetTime(TimeOfDay.Beginning);
        CustomerLineManager.Instance.Advance();
    }

    public void StartSetupPhase()
    {
        CurrentPhase = GamePhase.Setup;
        
        // TODO unlocks stations with progression
        _startSign.Lock(true);
        _stationUnlockPopup.Show(_grill);
        
        LockTiles(_spawnerTiles, true);
        LockTiles(_defaultTiles, false);
        LockTiles(_upgrade1Tiles, !ProgressionManager.Instance.Unlocked.Contains(UpgradeID.Restaurant1));
        LockTiles(_upgrade2Tiles, !ProgressionManager.Instance.Unlocked.Contains(UpgradeID.Restaurant2));
        LockTiles(_upgrade3Tiles, !ProgressionManager.Instance.Unlocked.Contains(UpgradeID.Restaurant3));   
    }

    public void StartCookingPhase()
    {
        CurrentPhase = GamePhase.Cooking;
        _dayTimer.StartTimer();
        _callBell.Lock(false);
        _platingTile.Lock(false);
        LockTiles(_spawnerTiles, false);
        AudioManager.Instance.PlayMusicEntry("Cozy");
        CustomerLineManager.Instance.SetTime(TimeOfDay.Middle);
        CustomerLineManager.Instance.Advance();
    }

    public void EndCookingPhase()
    {
        _dayTimer.StopTimer();
        _callBell.Lock(true);
        LockTiles(_allTiles, true);
        CustomerLineManager.Instance.SetTime(TimeOfDay.End);
        CustomerLineManager.Instance.Advance();
    }

    public void SetAllStationsPlaced()
    {
        _startSign.Lock(false);
    }

    public void FinishDay()
    {
        
    }

    private void LockTiles(IEnumerable<TileUI> tiles, bool locked)
    {
        foreach (TileUI tile in tiles)
        {
            tile.Lock(locked);
        }
    }
}