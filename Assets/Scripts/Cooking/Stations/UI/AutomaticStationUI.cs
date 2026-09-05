using UnityEngine;

public class AutomaticStationUI : StationUI
{
    [SerializeField] private TimerUI _timerUI;

    public void SetTimer(float amount, bool overcook)
    {
        _timerUI.SetProgress(amount);
        _timerUI.SetColor(overcook ? s_overcookColor : s_normalColor);
    }

    public void ShowTimer(bool show)
    {
        _timerUI.Show(show);
    }
}
