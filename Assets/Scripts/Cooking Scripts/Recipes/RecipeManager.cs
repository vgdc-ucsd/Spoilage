using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IngredientRequirement
{
    public string ingredientName;
    public string requiredState;
}

[System.Serializable]
public class Recipe
{
    public string dishName;
    public List<IngredientRequirement> ingredients;
}

[System.Serializable]
public class RecipeList
{
    public List<Recipe> recipes;
}

public class RecipeManager : MonoBehaviour
{
    public TextAsset recipeJsonFile; // Drag your JSON file here in the Inspector!
    public RecipeList allRecipes;

    void Awake()
    {
        LoadRecipes();
    }

    void LoadRecipes()
    {
        if (recipeJsonFile != null)
        {
            // THIS is what fills the "brain" of the manager
            allRecipes = JsonUtility.FromJson<RecipeList>(recipeJsonFile.text);
            Debug.Log("MANAGER: Loaded " + allRecipes.recipes.Count + " recipes from JSON.");
        }
        else
        {
            Debug.LogError("MANAGER: No JSON file assigned in the Inspector!");
        }
    }

    public string CheckRecipe(List<IngredientObject> plateIngredients)
    {
        Debug.Log($"MANAGER: Starting check for {plateIngredients.Count} items on plate.");

        if (allRecipes == null || allRecipes.recipes == null)
        {
            return "JSON Error";
        }

        foreach (Recipe recipe in allRecipes.recipes)
        {
            // Now this will actually run because the list isn't empty!
            if (IsMatch(recipe, plateIngredients))
            {
                return recipe.dishName;
            }
        }

        return "Slop";
    }

    private bool IsMatch(Recipe recipe, List<IngredientObject> plateIngredients)
    {
        if (recipe == null || recipe.ingredients == null || plateIngredients == null) return false;
        if (recipe.ingredients.Count != plateIngredients.Count) return false;

        foreach (var req in recipe.ingredients)
        {
            // JSON CHECK: If your JSON is missing a name or state, this stops the crash
            if (req == null || string.IsNullOrEmpty(req.ingredientName) || string.IsNullOrEmpty(req.requiredState))
            {
                Debug.LogError($"MANAGER: Recipe '{recipe.dishName}' has a broken ingredient entry in the JSON file!");
                return false;
            }

            bool found = false;
            foreach (var food in plateIngredients)
            {
                // PLATE CHECK: If the food on the plate is broken, this stops the crash
                if (food == null || food.IngredientInstance == null || food.IngredientInstance.Data == null)
                {
                    continue;
                }

                string plateName = food.IngredientInstance.Data.Name.Trim();
                string jsonName = req.ingredientName.Trim();
                string plateState = food.IngredientInstance.CurrentCookState.ToString().Trim();
                string jsonState = req.requiredState.Trim();

                if (plateName.Equals(jsonName, System.StringComparison.OrdinalIgnoreCase) &&
                    plateState.Equals(jsonState, System.StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }
        return true;
    }
}
