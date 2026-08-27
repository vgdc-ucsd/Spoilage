using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum GamePhase
{
    Setup,
    Cooking
}

public class SetupManager : Singleton<SetupManager>
{
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Setup;

    [Header("References")]
    [SerializeField] private GameObject _gameCanvas;
    [SerializeField] private List<SpawnerTileUI> _spawnerTiles;
    [SerializeField] private List<KitchenTileUI> _defaultTiles;
    [SerializeField] private List<KitchenTileUI> _upgrade1Tiles;
    [SerializeField] private List<KitchenTileUI> _upgrade2Tiles;
    [SerializeField] private List<KitchenTileUI> _upgrade3Tiles;

    [Header("Start Sign")]
    [SerializeField] private Image _startSignImage;
    [SerializeField] private Sprite[] _signSprites;
    [SerializeField] private float _flipSignAnimTime = 0.25f;

    [Header("New Station Popup")]
    [SerializeField] private StationUnlockPopup _stationUnlockPopup;
    [SerializeField] private StationData _grill;
    
    // TODO old code remove or adapt animations
    public GameObject _newStationPrefab;
    [SerializeField] private float _closePopupAnimTime = 0.5f;
    [SerializeField] private List<GameObject> _stationPopupPrefabs;
    private RectTransform _currStationPopup;
    [HideInInspector] public GameObject InstantiatedStation; // This is used to detect when to remove the popup

    private void Start()
    {
        AudioManager.Instance.PlayMusicEntry("KitchenLayout");

        LockTiles(_spawnerTiles, true);
        LockTiles(_defaultTiles, false);
        LockTiles(_upgrade1Tiles, !ProgressionManager.Instance.Unlocked.Contains(UpgradeID.Restaurant1));
        LockTiles(_upgrade2Tiles, !ProgressionManager.Instance.Unlocked.Contains(UpgradeID.Restaurant2));
        LockTiles(_upgrade3Tiles, !ProgressionManager.Instance.Unlocked.Contains(UpgradeID.Restaurant3));

        List<KitchenTileUI> kitchenTileUIs = new List<KitchenTileUI>();
        kitchenTileUIs.AddRange(_defaultTiles);
        kitchenTileUIs.AddRange(_upgrade1Tiles);
        kitchenTileUIs.AddRange(_upgrade2Tiles);
        kitchenTileUIs.AddRange(_upgrade3Tiles);

        foreach (KitchenTileUI ui in kitchenTileUIs)
        {
            ui.Init();
        }

        foreach (SpawnerTileUI ui in _spawnerTiles)
        {
            ui.Init();
        }

        List<ITemporalTile> temporalTiles = kitchenTileUIs.Select(ui => ui.Tile as ITemporalTile).ToList();
        
        // TODO add plating tile
        CookingManager.Instance.SetTiles(temporalTiles);

        // Initialize new station popup
        _stationUnlockPopup.Show(_grill);

        if(_newStationPrefab != null)
        {
            GameObject prefab =_stationPopupPrefabs.Find(x => x.name.Contains(_newStationPrefab.name));
            GameObject instantiated = Instantiate(prefab, _gameCanvas.transform);
            _currStationPopup = instantiated.GetComponent<RectTransform>();
            InstantiatedStation = _currStationPopup.GetChild(0).GetChild(0).gameObject;
        }
    }

    public void StartCooking()
    {
        if(!CanStartDay()) return;

        CurrentPhase = GamePhase.Cooking;
        AudioManager.Instance.PlayMusicEntry("Cozy");
        LockTiles(_spawnerTiles, false);

        StartCoroutine(UpdateSignSprite());

        Debug.Log("Phase: Cooking");
    }

    public void StartSetup()
    {
        CurrentPhase = GamePhase.Setup;
    }

    private void LockTiles(IEnumerable<TileUI> tiles, bool locked)
    {
        foreach (TileUI tile in tiles)
        {
            tile.Lock(locked);
        }
    }

    private IEnumerator UpdateSignSprite()
    {
        yield return BasicAnimations.Interpolate(
            null,
            (t) =>
            {
                float curve = BasicAnimations.EaseInBack(t);
                _startSignImage.rectTransform.localEulerAngles = Vector3.Lerp(new(0, 0, 0), new(0, 90f, 0), curve);
            },
            () => {

            },
            _flipSignAnimTime / 2f
        );

        yield return BasicAnimations.Interpolate(
            () => _startSignImage.sprite = _signSprites[(int)CurrentPhase],
            (t) =>
            {
                float curve = BasicAnimations.EaseOutBack(t);
                _startSignImage.rectTransform.localEulerAngles = Vector3.Lerp(new(0, 90f, 0), new(0, 0, 0), curve);
            },
            null,
            _flipSignAnimTime / 2f
        );

        Vector3 _initSignPos = _startSignImage.rectTransform.localPosition;
        Vector3 _targetSignPos = new(_startSignImage.rectTransform.localPosition.x, _startSignImage.rectTransform.localPosition.y + _startSignImage.rectTransform.sizeDelta.y);

        yield return BasicAnimations.Interpolate(
            null,
            (t) =>
            {
                float curve = BasicAnimations.EaseInBack(t);
                _startSignImage.rectTransform.localPosition = Vector3.Lerp(
                    _initSignPos, 
                    _targetSignPos,
                    curve
                );
            },
            () => _startSignImage.gameObject.SetActive(false),
            _flipSignAnimTime
        );
    }

    private bool CanStartDay()
    {
        return _newStationPrefab == null && CurrentPhase == GamePhase.Setup;
    }

    public IEnumerator HideNewStationPopup()
    {
        Vector3 _initPopupPos = _currStationPopup.localPosition;
        Vector3 _targetPopupPos = new(_currStationPopup.localPosition.x-_currStationPopup.sizeDelta.x, _currStationPopup.localPosition.y);

        yield return BasicAnimations.Interpolate(
            () => _newStationPrefab = null,
            (t) =>
            {
                float curve = BasicAnimations.EaseInBack(t);
                _currStationPopup.localPosition = Vector3.Lerp(_initPopupPos, _targetPopupPos, curve);
            },
            null,
            _closePopupAnimTime
        );
    }
}