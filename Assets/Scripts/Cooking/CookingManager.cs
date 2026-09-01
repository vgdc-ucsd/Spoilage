using System.Collections.Generic;
using UnityEngine;

public class CookingManager : Singleton<CookingManager>
{
    [SerializeField] private IngredientData _slopData;
    private PlatingTile _platingTile;
    private List<ITemporalTile> _tiles = new List<ITemporalTile>();

    public Food Process(List<Food> ingredients, Station station)
    {
        IngredientData data = RecipeManager.Instance.LookupResult(ingredients, station);
        if (data == _slopData) return new Food(_slopData, 0f, 1f);

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

    public Food CreateSlop(Transform uiTransform)
    {
        Food slop = new Food(_slopData, 0f, 1f);
        slop.SetUI(PlaceableUIFactory.Instance.Generate(_slopData, uiTransform));
        slop.UI.gameObject.SetActive(false);
        return slop;
    }

    public bool IsSlop(Food food)
    {
        return food.Data == _slopData;
    }

    public void SetTiles(List<ITemporalTile> tiles, PlatingTile platingTile)
    {
        _tiles = tiles;
        _platingTile = platingTile;
    }

    public void SubmitOrder()
    {
        if (_platingTile.Food == null) return;

        Customer customer = CustomerLineManager.Instance.CurrentCustomer;
        Food food = _platingTile.Food;

        List<Recipe> orders = customer.customerData.orders;
        Recipe match = orders.Find(order => order.name == food.Data.Name);

        if (match != null)
        {
            // TODO
            // customer.customerData.patience = (customerData.patience + 0.5 > 1) ? 1 : customerData.patience += 0.5f;
            orders.Remove(match);
            if (orders.Count == 0)
            {
                // _resourceManager.Reputation += orderStreak;
                // _resourceManager.Wealth += (int)(match.reward * dish.QualityPercent);
                // orderStreak++;
                // StoryManager.Instance.OnCustomerServed(customerData, success);
                DialogueManager.Instance.PlayDialogue(
                    customer.Dialogue.Success, 
                    () => CustomerLineManager.Instance.Advance()
                );
            }
        }
        else
        {
            // orderStreak = 0;
            // _resourceManager.Reputation -= ;
            //StoryManager.Instance.OnCustomerServed(customerData, success);
            DialogueManager.Instance.PlayDialogue(
                customer.Dialogue.Fail, 
                () => CustomerLineManager.Instance.Advance()
            );
        }

        _platingTile.Remove();
        food.Destroy();
    }

    public void Update()
    {
        foreach (ITemporalTile tile in _tiles)
        {
            tile.Process(Time.deltaTime);
        }
    }
}
