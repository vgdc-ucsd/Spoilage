using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeStep : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _stepName;
    [SerializeField] private Image _ingredientA;
    [SerializeField] private Image _ingredientB;
    [SerializeField] private Image _ingredientC;
    [SerializeField] private Image _station;
    [SerializeField] private Image _result;
    [SerializeField] private GameObject _operationB;
    [SerializeField] private GameObject _operationC;
    [SerializeField] private GameObject _operationStation;

    public void SetStep(Recipe step, int num)
    {        
        _stepName.text = $"Step {num}";
        _result.sprite = IngredientLookup.Get(step.name).NormalSprite;
        _ingredientA.sprite = IngredientLookup.Get(step.requiredIngredients[0].name).NormalSprite;
        
        if (step.requiredIngredients.Length >= 2)
        {
            _ingredientB.sprite = IngredientLookup.Get(step.requiredIngredients[1].name).NormalSprite;
            _ingredientB.gameObject.SetActive(true);
            _operationB.SetActive(true);
        }
        else
        {
            _ingredientB.gameObject.SetActive(false);
            _operationB.SetActive(false);
        }

        if (step.requiredIngredients.Length >= 3)
        {
            _ingredientC.sprite = IngredientLookup.Get(step.requiredIngredients[1].name).NormalSprite;
            _ingredientC.gameObject.SetActive(true);
            _operationC.SetActive(true);
        }
        else
        {
            _ingredientC.gameObject.SetActive(false);
            _operationC.SetActive(false);
        }

        if (!string.IsNullOrEmpty(step.appliance) && step.appliance != "Kitchen Counter")
        {
            _station.sprite = StationLookup.Instance.NameToData(step.appliance).SpriteOff;
            _station.gameObject.SetActive(true);
            _operationStation.SetActive(true);
        }
        else
        {
            _station.gameObject.SetActive(false);
            _operationStation.SetActive(false);
        }
    }
}
