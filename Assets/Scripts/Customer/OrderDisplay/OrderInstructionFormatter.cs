using System.Collections.Generic;
using System.Text;

public static class OrderInstructionFormatter
{
    public static string FormatOrders(IReadOnlyList<CustomerOrder> orders, RecipeManager recipeManager)
    {
        if (orders == null || orders.Count == 0)
        {
            return "No order instructions available.";
        }

        StringBuilder builder = new StringBuilder();
        int displayedOrders = 0;

        for (int i = 0; i < orders.Count; i++)
        {
            CustomerOrder order = orders[i];
            if (order == null)
            {
                continue;
            }

            if (displayedOrders > 0)
            {
                builder.AppendLine();
            }

            displayedOrders++;
            AppendOrder(builder, order, recipeManager);
        }

        return displayedOrders == 0 ? "No order instructions available." : builder.ToString();
    }

    private static void AppendOrder(StringBuilder builder, CustomerOrder order, RecipeManager recipeManager)
    {
        string dishName = string.IsNullOrWhiteSpace(order.DishName) ? order.name : order.DishName;

        builder.AppendLine($"<b>{dishName}</b>");

        if (recipeManager != null && recipeManager.TryGetRecipe(dishName, out Recipe recipe))
        {
            AppendRecipeRequirements(builder, recipe);
            return;
        }

        AppendCustomerOrderIngredients(builder, order);
        builder.AppendLine("Prep: Not specified");
        builder.AppendLine("Station: Not specified");
    }

    private static void AppendRecipeRequirements(StringBuilder builder, Recipe recipe)
    {
        builder.AppendLine("Ingredients:");

        if (recipe.ingredients == null || recipe.ingredients.Count == 0)
        {
            builder.AppendLine("- Not specified");
        }
        else
        {
            foreach (IngredientRequirement requirement in recipe.ingredients)
            {
                builder.AppendLine($"- {FormatRequirement(requirement)}");
            }
        }

        // Recipes.json currently tracks ingredient state requirements, but not station names.
        builder.AppendLine("Station: Not specified");
    }

    private static string FormatRequirement(IngredientRequirement requirement)
    {
        if (requirement == null)
        {
            return "Not specified";
        }

        string ingredientName = string.IsNullOrWhiteSpace(requirement.ingredientName)
            ? "Unnamed ingredient"
            : requirement.ingredientName.Trim();

        List<string> prepRequirements = new List<string>();

        if (!string.IsNullOrWhiteSpace(requirement.requiredState))
        {
            prepRequirements.Add(requirement.requiredState.Trim());
        }

        if (!string.IsNullOrWhiteSpace(requirement.requiredChoppedState))
        {
            prepRequirements.Add(requirement.requiredChoppedState.Trim());
        }

        if (prepRequirements.Count == 0)
        {
            return ingredientName;
        }

        return $"{ingredientName} ({string.Join(", ", prepRequirements)})";
    }

    private static void AppendCustomerOrderIngredients(StringBuilder builder, CustomerOrder order)
    {
        builder.AppendLine("Ingredients:");

        if (order.Ingredients == null || order.Ingredients.Length == 0)
        {
            builder.AppendLine("- Not specified");
            return;
        }

        int displayedIngredients = 0;

        foreach (Ingredient ingredient in order.Ingredients)
        {
            if (ingredient == null)
            {
                continue;
            }

            string ingredientName = ingredient.Data != null && !string.IsNullOrWhiteSpace(ingredient.Data.Name)
                ? ingredient.Data.Name
                : ingredient.GetType().Name;

            builder.AppendLine($"- {ingredientName}");
            displayedIngredients++;
        }

        if (displayedIngredients == 0)
        {
            builder.AppendLine("- Not specified");
        }
    }
}
