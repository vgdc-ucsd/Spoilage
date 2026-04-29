using System.Collections.Generic;

[System.Serializable]
public class IngredientRequirement
{
    public string ingredientName;
    public string requiredState; // ex: toasted, grilled, boiled
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
