using System.Collections;
using UnityEngine;

public class GuardsStaminaBar : MonoBehaviour
{
    [SerializeField] private Transform _staminaArrow;

    private const float MOVE_DURATION = 0.25f;
    private const float MAX_ROTATION = -15f;
    private const float SPAN = 57f;

    public void SetStamina(float amount)
    {
        float amt = (amount * 2.0f) - 1.0f;
        StopAllCoroutines();
        StartCoroutine(MoveArrow(amt));
    }

    private IEnumerator MoveArrow(float amount)
    {
        Vector3 startPos = _staminaArrow.localPosition;
        Vector3 endPos = new Vector3(
            amount * SPAN,
            _staminaArrow.localPosition.y,
            _staminaArrow.localPosition.z
        );

        Quaternion startRotation = _staminaArrow.localRotation;
        Quaternion endRotation = Quaternion.Euler(new Vector3(0f, 0f, MAX_ROTATION * amount));

        yield return BasicAnimations.Interpolate(
            null,
            (t) =>
            {
                float bounce = BasicAnimations.EaseOutElastic(t);
                _staminaArrow.localPosition = Vector3.Lerp(startPos, endPos, bounce);
                _staminaArrow.localRotation = Quaternion.Slerp(startRotation, endRotation, bounce);
            },
            () => 
            {
                _staminaArrow.localPosition = endPos;
                _staminaArrow.localRotation = endRotation;
            },
            MOVE_DURATION
        );
    }
}
