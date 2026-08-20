using System.Collections.Generic;

public class CookingManager : Singleton<CookingManager>
{
    public Food Process(List<Food> ingredients, Station station)
    {
        IngredientData data = RecipeManager.Instance.LookupResult(ingredients, station);

        float quality = 0f;
        float spoilage = 0f;
        // Seasoning?

        foreach (Food food in ingredients)
        {
            quality += food.QualityPercent;
            spoilage += food.SpoilagePercent;
        }

        quality /= ingredients.Count;
        spoilage /= ingredients.Count;
        
        return new Food(data, quality, spoilage);
    }
}
