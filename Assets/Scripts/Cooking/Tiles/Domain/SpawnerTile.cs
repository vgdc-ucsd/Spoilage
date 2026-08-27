public class SpawnerTile : ITile
{
    private IngredientData _spawnedIngredient;
    private Food _spawnedFood;
    private SpawnerTileUI _ui;

    public SpawnerTile(IngredientData data, SpawnerTileUI ui)
    {
        _spawnedIngredient = data;
        _ui = ui;
        _spawnedFood = GenerateFood();
    }

    public bool Accepts(Placeable _)
    {
        return false;
    }

    public Placeable Produces()
    {
        if (SetupManager.Instance.CurrentPhase == GamePhase.Cooking)
        {
            return _spawnedFood;
        }

        return null;
    }
    
    public void Remove()
    {
        _spawnedFood = GenerateFood();
    }

    public void Place(Placeable placeable) { }

    private Food GenerateFood()
    {
        Food food = new Food(_spawnedIngredient);
        food.SetUI(_ui.GenerateFoodUI());
        return food;
    }
}
