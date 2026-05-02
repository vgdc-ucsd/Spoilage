using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IngredientRequirement
{
    public string ingredientName;
    public string requiredState; //required state is the cooked state
    public string requiredChoppedState;
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
        if (recipe == null || plateIngredients == null) return false;
        if (recipe.ingredients.Count != plateIngredients.Count) return false;

        foreach (var req in recipe.ingredients)
        {
            bool found = false;
            foreach (var food in plateIngredients)
            {
                if (food == null || food.IngredientInstance == null) continue;

                // 1. Get names and states
                string plateName = food.IngredientInstance.Data.Name.Trim();
                string plateCookState = food.IngredientInstance.CurrentCookState.ToString().Trim();
                string plateChoppedState = food.IngredientInstance.CurrentChoppedState.ToString().Trim();

                // 2. Basic Name Match
                bool nameMatches = plateName.Equals(req.ingredientName.Trim(), System.StringComparison.OrdinalIgnoreCase);

                // 3. Cook State Match
                bool cookMatches = plateCookState.Equals(req.requiredState.Trim(), System.StringComparison.OrdinalIgnoreCase);

                // 4. Chopped State Match (Optional: if JSON is empty, we assume it's a match)
                bool choppedMatches = true;
                if (!string.IsNullOrEmpty(req.requiredChoppedState))
                {
                    choppedMatches = plateChoppedState.Equals(req.requiredChoppedState.Trim(), System.StringComparison.OrdinalIgnoreCase);
                }

                if (nameMatches && cookMatches && choppedMatches)
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
