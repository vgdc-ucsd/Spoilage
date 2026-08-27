using UnityEngine;
using UnityEngine.UI;

public class StationUnlockPopup : MonoBehaviour
{
    [SerializeField] private StationUnlockTileUI _tile;
    [SerializeField] private Image _popupBG;
    [SerializeField] private Sprite _grillUnlock;
    [SerializeField] private Sprite _potUnlock;
    [SerializeField] private Sprite _blenderUnlock;
    [SerializeField] private Sprite _cuttingBoardUnlock;
    [SerializeField] private Sprite _ovenUnlock;
    [SerializeField] private Sprite _seasoningStationUnlock;

    public void Show(StationData station)
    {
        gameObject.SetActive(true);
        
        switch (station.StationCategory)
        {
            case StationCategory.CuttingBoard:
                _popupBG.sprite = _cuttingBoardUnlock;
                break;
            case StationCategory.Pot:
                _popupBG.sprite = _potUnlock;
                break;
            case StationCategory.Grill:
                _popupBG.sprite = _grillUnlock;
                break;
            case StationCategory.Oven:
                _popupBG.sprite = _ovenUnlock;
                break;
            case StationCategory.Blender:
                _popupBG.sprite = _blenderUnlock;
                break;
            case StationCategory.SeasoningStation:
                _popupBG.sprite = _seasoningStationUnlock;
                break;
            default:
                Debug.LogError("Unrecognized Station");
                break;    
        }

        _tile.Init(station, this);

        // TODO animation
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        // TODO animation
    }
}
