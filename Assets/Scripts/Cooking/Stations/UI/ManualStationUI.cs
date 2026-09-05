using UnityEngine;

public class ManualStationUI : StationUI
{
    [SerializeField] private TimerUI _timerUI;
    private ManualStation _manualStation;

    public override void SetStation(Station station)
    {
        base.SetStation(station);
        _manualStation = station as ManualStation;
    }

    void Start()
    {
        _timerUI.SetColor(s_normalColor);
    }

    public void SetClicks(float amount)
    {
        _timerUI.SetProgress(amount);
    }

    public void ShowClicks(bool show)
    {
        _timerUI.Show(show);
    }

    public void Click()
    {
        _manualStation.Click();
    }
}