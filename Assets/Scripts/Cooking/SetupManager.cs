using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum GamePhase
{
    Setup,
    Cooking
}

public class SetupManager : MonoBehaviour
{
    public static SetupManager Instance { get; private set; }
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Setup;

    [Header("References")]
    [SerializeField] private GameObject _gameCanvas;

    [Header("Start Sign")]
    [SerializeField] private Image _startSignImage;
    [SerializeField] private Sprite[] _signSprites;
    [SerializeField] private float _flipSignAnimTime = 0.25f;

    [Header("New Station Popup")]
    public GameObject _newStationPrefab;
    [SerializeField] private float _closePopupAnimTime = 0.5f;
    [SerializeField] private List<GameObject> _stationPopupPrefabs;
    private RectTransform _currStationPopup;
    [HideInInspector] public GameObject InstantiatedStation; // This is used to detect when to remove the popup

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        FoodGrab.CanMoveFood = false;
        ObjectGrab.CanMoveAppliances = true;
    }

    private void Start()
    {
        // Initialize new station popup
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
        FoodGrab.CanMoveFood = true;
        ObjectGrab.CanMoveAppliances = false;

        StartCoroutine(UpdateSignSprite());

        Debug.Log("Phase: Cooking");
    }

    public void StartSetup()
    {
        CurrentPhase = GamePhase.Setup;
        FoodGrab.CanMoveFood = false;
        ObjectGrab.CanMoveAppliances = true;

        Debug.Log("Phase: Setup");
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