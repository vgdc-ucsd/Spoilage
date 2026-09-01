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
        Customer customer = CustomerLineManager.Instance.CurrentCustomer;
        List<Recipe> CustomerOrder = customer.customerData.orders;
        /* Predicate<Recipe> predicate = x => x.name == dish.name;
        Recipe match = CustomerOrder.Find(predicate);
        bool success = match != null;
 */
        /* if (success)
        {
            // increase the necessary resources
            orderStreak++;
            customerData.patience = (customerData.patience + 0.5 > 1) ? 1 : customerData.patience += 0.5f;
            
            // for some reason its not able to find resourcemanager and i dont have the time to fix that
            // _resourceManager.Reputation += orderStreak;
            // _resourceManager.Wealth += (int)(match.reward * dish.QualityPercent);


            //not sure if they wrote this method knowing the customer could order multiple things, but oh well
            StoryManager.Instance.OnCustomerServed(customerData, success);

            CustomerOrder.Remove(match);

            //check if the order is done
            if (CustomerOrder.Count == 0)
            {
                //new customer!
                _lineManager.Advance();
            }
        }   else
        {
            orderStreak = 0;

            StoryManager.Instance.OnCustomerServed(customerData, success);
        }

        return success; */
    }

    public void Update()
    {
        foreach (ITemporalTile tile in _tiles)
        {
            tile.Process(Time.deltaTime);
        }
    }
}
