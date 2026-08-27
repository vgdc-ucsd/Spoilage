using UnityEngine;

public class AutomaticStationUI : StationUI
{
    [SerializeField] private TimerUI _timerUI;

    // Colors for timer fill: normal (green) and overcooking (red)
    private static readonly Color32 s_normalColor = new Color32(22, 165, 31, 255);
    private static readonly Color32 s_overcookColor = new Color32(103, 14, 14, 255);

    public void AddIngredient(Placeable placeable)
    {
        placeable.UI.gameObject.SetActive(false);
        SetSprite(_station.Data.SpriteOn);
    }

    public void Empty()
    {
        SetSprite(_station.Data.SpriteOff);
    }

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
