using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBook : MonoBehaviour
{
    [SerializeField] private RecipeTab _recipeTabPrefab;
    [SerializeField] private Transform _recipeTabContainer;
    [SerializeField] private RecipeStep _recipeStepPrefab;
    [SerializeField] private Transform _recipeStepContainer;
    [SerializeField] private TextMeshProUGUI _recipeNameText;
    [SerializeField] private TextMeshProUGUI _recipeDescriptionText;
    [SerializeField] private Image _recipeImage;
    [SerializeField] private Image _recipePlate;
    [SerializeField] private Button _toggleSpoiledButton; 
    [SerializeField] private TextMeshProUGUI _toggleSpoiledButtonLabel; 

    private List<Recipe> _recipes;
    private List<Recipe> _steps;
    private bool _spoiled = false;
    private IngredientData _ingredient;

    public void Show()
    {
        AudioManager.Instance.PlaySFX("PageTurn");
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
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
        _ingredient = IngredientLookup.Get(recipe.name);

        _recipeNameText.text = _ingredient.Name;
        _recipeDescriptionText.text = _ingredient.RecipeBookDescriptionUnspoiled;
        _recipeImage.sprite = _ingredient.NormalSprite;
        _recipePlate.sprite = _ingredient.PlateSprite;
        _toggleSpoiledButton.gameObject.SetActive(!recipe.spoiled);
        _spoiled = false;

        _steps = FindSteps(recipe);

        foreach (Transform child in _recipeStepContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _steps.Count; i++)
        {
            Recipe step = _steps[i];
            RecipeStep recipeStep = Instantiate(_recipeStepPrefab, _recipeStepContainer);
            recipeStep.SetStep(step, i + 1);
        }
    }

    private List<Recipe> FindSteps(Recipe root)
    {
        List<Recipe> steps = new List<Recipe>();
        
        foreach (RecipeRequirement requirement in root.requiredIngredients)
        {
            Recipe recipe = RecipeManager.Instance.FindRecipe(requirement.name);
            if (recipe.requiredIngredients != null && recipe.requiredIngredients.Length > 0)
            {
                steps.AddRange(FindSteps(recipe));    
            }
        }

        steps.Add(root);
        return steps;
    }

    public void ToggleSpoiled()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        if (_spoiled)
        {
            _recipeDescriptionText.text = _ingredient.RecipeBookDescriptionUnspoiled;
            _recipeImage.sprite = _ingredient.NormalSprite;
            _toggleSpoiledButtonLabel.text = "Show Spoiled";
        }
        else
        {
            _recipeDescriptionText.text = _ingredient.RecipeBookDescriptionSpoiled;
            _recipeImage.sprite = _ingredient.SpoiledSprite;
            _toggleSpoiledButtonLabel.text = "Show Unspoiled";
        }

        _spoiled = !_spoiled;
    } 
}
