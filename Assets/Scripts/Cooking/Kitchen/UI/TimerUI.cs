using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Image _timerImage;

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public void SetProgress(float progress)
    {
        _timerImage.fillAmount = Mathf.Clamp01(progress);
    }

    public void SetColor(Color color)
    {
        _timerImage.color = color;
    }
}
