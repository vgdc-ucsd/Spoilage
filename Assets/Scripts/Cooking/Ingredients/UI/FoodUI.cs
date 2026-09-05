using UnityEngine;
using UnityEngine.UI;

public class FoodUI : PlaceableUI
{
    [SerializeField] private TimerUI _timerUI;
    [SerializeField] private Material _overcookMaterial;
    [SerializeField] private Image _plate;
    private Food _food;
    private bool _plated;

    private static readonly int s_burntProperty = Shader.PropertyToID("_Boolean");

    public override void SetDrag(bool drag)
    {
        if (drag) 
        {
            _timerUI.Show(false);
            SetPlated(false);
        }
        else
        {
            if (_plated) 
            {
                SetPlated(true);
            }
        }

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
        if (_food.Spoiling) 
        {
            ShowTimer(true);
        }
    }

    public void Spoil()
    {
        SetSprite(_food.Data.SpoiledSprite);
        ShowTimer(false);
    }

    public void SetFood(Food food)
    {
        _food = food;
        
        if (food.Data.IsSmallIngredient)
        {
            _image.gameObject.transform.localScale = Vector3.one * 0.5f;
        }
        
        if (food.Data.PlateSprite != null) 
        {
            _plate.sprite = food.Data.PlateSprite;
        }
    }

    public void SetPlated(bool plated)
    {
        _plated = plated;
        _plate.gameObject.SetActive(plated);
    }
}
