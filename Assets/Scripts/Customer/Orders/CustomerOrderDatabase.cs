using System.Collections.Generic;
using UnityEngine;

public class CustomerOrderDatabase : Singleton<CustomerOrderDatabase>
{
    private RecipeManager _recipeManager;

    private SaveManager _saveManager;

    [Header("Chance curves based on game progress from 0 to 1")]
    [SerializeField]
    private AnimationCurve _oneDishChance;

    [SerializeField]
    private AnimationCurve _twoDishChance;

    [SerializeField]
    private AnimationCurve _threeDishChance;

    [SerializeField]
    private AnimationCurve _fourDishChance;

    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        _recipeManager = RecipeManager.Instance;
        _saveManager = SaveManager.Instance;

        UpdateAvailableRecipes();
    }

    public int PickDishCount(float gameProgress)
    {
        gameProgress = Mathf.Clamp01(gameProgress);

        float one = Mathf.Max(0, _oneDishChance.Evaluate(gameProgress));
        float two = Mathf.Max(0, _twoDishChance.Evaluate(gameProgress));
        float three = Mathf.Max(0, _threeDishChance.Evaluate(gameProgress));
        float four = Mathf.Max(0, _fourDishChance.Evaluate(gameProgress));

        float total = one + two + three + four;

        if (total <= 0)
        {
            return 1;
        }

        float randomValue = UnityEngine.Random.Range(0, total);

        if (randomValue < one)
        {
            return 1;
        }

        randomValue -= one;

        if (randomValue < two)
        {
            return 2;
        }

        randomValue -= two;

        if (randomValue < three)
        {
            return 3;
        }

        return 4;
    }

    /// <summary>
    /// Adds Recipes to the global UnlockedRecipes based on the current unlocked
    /// ingredients and appliances. Should be called every time a new recipe or
    /// appliance is unlocked.
    /// </summary>
    public void UpdateAvailableRecipes()
    {
        SyncUnlockedRecipes();

        foreach (Recipe recipe in _recipeManager.allRecipes.allRecipes)
        {
            if (recipe.servable)
            {
                if (CheckPlayerCanMakeRecipe(recipe) && !IsRecipeUnlocked(recipe))
                {
                    _saveManager.Player.RecipesUnlocked.Add(recipe);
                    Debug.Log("Added " + recipe.name + " to unlocked recipes");
                }
            }
        }
    }

    /// <summary>
    /// Recursive function that goes through every required ingredient for a 
    /// recipe until it reaches the base ingredients, then checks to see if 
    /// that ingredient is unlocked.
    /// </summary>
    public bool CheckPlayerCanMakeRecipe(Recipe recipe)
    {
        // BASE CASE: Base ingredient, check if unlocked if not return false
        if (recipe.requiredIngredients == null || recipe.requiredIngredients.Length == 0)
        {
            return _saveManager.Player.IngredientsUnlocked.Contains(recipe.name);
        }

        bool result = true;

        // APPLIANCE CHECK: If an appliance is required check to make sure its 
        // unlocked
        if (recipe.appliance != "None" && recipe.appliance != "Spoil")
        {
            if (!_saveManager.Player.StationsUnlocked.Contains(recipe.appliance))
            {
                result = false;
            }
        }

        // INGREDIENT CHECK: Go through each ingredient until reaching base case
        foreach (RecipeRequirement ingredient in recipe.requiredIngredients)
        {
            if (!CheckPlayerCanMakeRecipe(_recipeManager.allRecipes.allRecipes[ingredient.id]))
            {
                result = false;
            }
        }
        return result;
    }

    public Recipe GenerateCustomerOrder()
    {
        return null;
    }

    private void SyncUnlockedRecipes()
    {
        List<Recipe> unlockedRecipes = _saveManager.Player.RecipesUnlocked;

        for (int i = 0; i < unlockedRecipes.Count; i++)
        {
            Recipe recipe = unlockedRecipes[i];

            unlockedRecipes[i] = _recipeManager.allRecipes.allRecipes[recipe.id];
        }
    }

    private bool IsRecipeUnlocked(Recipe recipe)
    {
        foreach (Recipe unlockedRecipe in _saveManager.Player.RecipesUnlocked)
        {
            if (unlockedRecipe.id == recipe.id)
            {
                return true;
            }
        }

        return false;
    }
}
