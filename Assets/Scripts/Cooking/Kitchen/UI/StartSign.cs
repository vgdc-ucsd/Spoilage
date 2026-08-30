using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartSign : MonoBehaviour
{
    [SerializeField] private Image _startSignImage;
    [SerializeField] private Sprite[] _signSprites;
    [SerializeField] private float _flipSignAnimTime = 0.25f;
    private bool _locked;

    public void Flip()
    {
        if (_locked) return;
        SetupManager.Instance.StartCookingPhase();
        StartCoroutine(UpdateSignSprite());
    }

    public void Lock(bool locked)
    {
        _locked = locked;
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
            () => _startSignImage.sprite = _signSprites[(int)SetupManager.Instance.CurrentPhase],
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
}
