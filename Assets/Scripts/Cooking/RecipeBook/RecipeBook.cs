using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBook : MonoBehaviour
{
    [SerializeField] private RecipeTab _recipeTabPrefab;
    [SerializeField] private Transform _recipeTabContainer;
    [SerializeField] private TextMeshProUGUI _recipeNameText;
    [SerializeField] private TextMeshProUGUI _recipeDescriptionText;
    [SerializeField] private Image _recipeImage;
    [SerializeField] private Image _recipePlate;

    private List<Recipe> _recipes;
    private List<Recipe> _steps;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Init(List<Recipe> recipes)
    {
        _recipes = recipes;
        SelectRecipe(0);

        foreach (Transform child in _recipeTabContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < recipes.Count; i++)
        {
            Recipe recipe = recipes[i];
            IngredientData ingredient = IngredientLookup.Get(recipe.name);
            RecipeTab recipeTab = Instantiate(_recipeTabPrefab, _recipeTabContainer);
            recipeTab.Init(this, ingredient.NormalSprite, i);        
        }
    }

    public void SelectRecipe(int index)
    {
        Recipe recipe = _recipes[index];
        IngredientData ingredient = IngredientLookup.Get(recipe.name);

        _recipeNameText.text = ingredient.Name;
        _recipeImage.sprite = ingredient.NormalSprite;
        _recipePlate.sprite = ingredient.PlateSprite;

        _steps = FindSteps(recipe);
    }

    private List<Recipe> FindSteps(Recipe root)
    {
        List<Recipe> steps = new List<Recipe>{ root };
        
        foreach (RecipeRequirement requirement in root.requiredIngredients)
        {
            Recipe recipe = RecipeManager.Instance.FindRecipe(requirement.name);
            steps.AddRange(FindSteps(recipe));    
        }

        return steps;
    } 
}
