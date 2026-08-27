using UnityEngine;

public class FoodUI : PlaceableUI
{
    [SerializeField] private TimerUI _timerUI;
    [SerializeField] private Material _overcookMaterial;
    private Food _food;

    private static readonly int s_burntProperty = Shader.PropertyToID("_Boolean");

    public override void SetDrag(bool drag)
    {
        if (drag) _timerUI.Show(false);
        else if (_food.Spoiling) _timerUI.Show(true);
        base.SetDrag(drag);
    }

    public void SetBurnt(bool isBurnt)
    {
        _overcookMaterial.SetInt(s_burntProperty, isBurnt ? 1 : 0);
    }

    public void ShowTimer(bool show)
    {
        _timerUI.Show(show);
    }

    public void SetSpoilage(float progress)
    {
        _timerUI.SetProgress(progress);
    }

    public void Spoil()
    {
        SetSprite(_food.Data.SpoiledSprite);
        ShowTimer(false);
    }

    public void SetFood(Food food)
    {
        _food = food;
    }
}
