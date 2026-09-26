using System.Collections;
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
    [SerializeField] private float _closePopupAnimTime = 0.5f;
    [SerializeField] private RectTransform _rt;

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
    }

    public void Hide()
    {
        StartCoroutine(HideNewStationPopup());
    }

    public IEnumerator HideNewStationPopup()
    {
        Canvas.ForceUpdateCanvases();

        Canvas canvas = _rt.GetComponentInParent<Canvas>().rootCanvas;
        RectTransform canvasRect = (RectTransform)canvas.transform;
        Transform parent = _rt.parent;
        Vector3 initPopupPos = _rt.localPosition;
        Vector3[] corners = new Vector3[4];
        _rt.GetWorldCorners(corners);

        float popupRight = float.NegativeInfinity;
        foreach (Vector3 corner in corners)
        {
            float cornerX = canvasRect.InverseTransformPoint(corner).x;
            popupRight = Mathf.Max(popupRight, cornerX);
        }

        const float padding = 20f;
        float moveX = Mathf.Min(0f, canvasRect.rect.xMin - popupRight - padding);
        Vector3 worldOffset = canvasRect.TransformVector(new Vector3(moveX, 0f, 0f));
        Vector3 targetPopupPos = initPopupPos + parent.InverseTransformVector(worldOffset);

        yield return BasicAnimations.Interpolate(
            null,
            (t) =>
            {
                float curve = BasicAnimations.EaseInBack(t);
                _rt.localPosition = Vector3.LerpUnclamped(initPopupPos, targetPopupPos, curve);
            },
            null,
            _closePopupAnimTime
        );

        _rt.localPosition = targetPopupPos;
    }
}