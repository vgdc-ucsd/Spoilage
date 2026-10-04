using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CookingManager : Singleton<CookingManager>
{
    [SerializeField] private IngredientData _slopData;
    private PlatingTile _platingTile;
    private List<ITemporalTile> _tiles = new List<ITemporalTile>();

    public Food Process(List<Food> ingredients, Station station, float bonusQuality = 0f)
    {
        IngredientData data = RecipeManager.Instance.LookupResult(ingredients, station);
        if (data == _slopData) return new Food(_slopData, 0f, 1f);

        float quality = bonusQuality;
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
        Item item = _platingTile.Item;

        List<Recipe> orders = customer.customerData.orders;
        Recipe foodMatch = orders.Find(order => order.name == food.Data.Name);
        DialogueItemData itemMatch = item != null ? customer.WantsItem(item.Data.ID) : null;

        if (customer.customerData.id == "Warlord")
        {
            HandleServeWarlord(customer, food, item, foodMatch, itemMatch);
            return;
        }

        if (foodMatch != null)
        {
            // TODO
            // customer.customerData.patience = (customerData.patience + 0.5 > 1) ? 1 : customerData.patience += 0.5f;
            orders.Remove(foodMatch);
            AudioManager.Instance.PlaySFX("OrderBellComplete");
            if (orders.Count == 0)
            {
                customer.EnablePatienceTimer(false);
                SaveManager.Instance.Player.Reputation += 1;
                SaveManager.Instance.Player.DayData.Streak++;
                float streakRewardMult = HandleStreakRewardMult();
                SaveManager.Instance.Player.DayData.Profits += Mathf.FloorToInt(foodMatch.reward * (1 + food.QualityPercent) * streakRewardMult);
                SaveManager.Instance.Player.DayData.CustomersServed++;

                if (itemMatch != null)
                {
                    // right food, right item
                    DialogueManager.Instance.PlayDialogue(
                        itemMatch.Success,
                        customer.customerData, 
                        () => CustomerLineManager.Instance.Advance()
                    );

                    // item is removed from plating tile
                    _platingTile.Remove(item);
                    item.Destroy();
                }
                else
                {
                    // right food, wrong item
                    DialogueManager.Instance.PlayDialogue(
                        customer.Dialogue.Success,
                        customer.customerData, 
                        () => CustomerLineManager.Instance.Advance()
                    );
                    // item stays on counter
                }
            }
        }
        else
        {
            customer.EnablePatienceTimer(false);
            SaveManager.Instance.Player.Reputation -= 1;
            SaveManager.Instance.Player.DayData.Streak = 0;

            if (itemMatch != null)
            {
                // wrong food, right item
                DialogueManager.Instance.PlayDialogue(
                    itemMatch.Fail,
                    customer.customerData, 
                    () => CustomerLineManager.Instance.Advance()
                );

                // item is removed from plating tile
                _platingTile.Remove(item);
                item.Destroy();
            }
            else
            {
                // wrong food, wrong item
                DialogueManager.Instance.PlayDialogue(
                    customer.Dialogue.Fail,
                    customer.customerData, 
                    () => CustomerLineManager.Instance.Advance()
                );
                // item stays on plating tile
            }
        }

        _platingTile.Remove(food);
        food.Destroy();
    }

    public void OrderFailed()
    {
        AudioManager.Instance.PlaySFX("RefuseButton");
        Customer customer = CustomerLineManager.Instance.CurrentCustomer;
        customer.EnablePatienceTimer(false);
        SaveManager.Instance.Player.Reputation -= 1;
        SaveManager.Instance.Player.DayData.Streak = 0;
        DialogueManager.Instance.PlayDialogue(
            customer.Dialogue.Fail,
            customer.customerData, 
            () => CustomerLineManager.Instance.Advance()
        );
    }

    public void Update()
    {
        if (DebugManager.Instance.AllowSkipDay && Keyboard.current.dKey.wasPressedThisFrame)
        {
            SetupManager.Instance.FinishDay();
        }

        foreach (ITemporalTile tile in _tiles)
        {
            tile.Process(Time.deltaTime);
        }
    }

    private float HandleStreakRewardMult()
    {
        if (ProgressionManager.Instance.Purchased.Contains(UpgradeID.Streak3))
        {
            return 1.5f;
        } else if (ProgressionManager.Instance.Purchased.Contains(UpgradeID.Streak2))
        {
            if (SaveManager.Instance.Player.DayData.Streak % 3 == 0)
                return 1.5f;
        } else if (ProgressionManager.Instance.Purchased.Contains(UpgradeID.Streak1))
        {
            if (SaveManager.Instance.Player.DayData.Streak % 6 == 0)
                return 1.5f;
        }

        return 1f; // no mult
    }

    private void HandleServeWarlord(Customer customer, Food food, Item item, Recipe foodMatch, DialogueItemData itemMatch)
    {
        GameEndScenario scenario = customer.Dialogue.ID switch
        {
            "SVE2-1" => GameEndScenario.Resistance,
            "SVE2-2" => GameEndScenario.Neutral,
            "SVE2-3" => GameEndScenario.Warlord,
            _ => GameEndScenario.Neutral
        };

        GameEndItemServed itemServed = GameEndItemServed.None;
        if (itemMatch != null)
        {
            itemServed = item.Data.ID switch
            {
                "ChildhoodDishPoisoned" => GameEndItemServed.Poisoned,
                "ChildhoodDishUnpoisoned" => GameEndItemServed.Unpoisoned,
                _ => GameEndItemServed.None
            };
        }

        if (foodMatch != null)
        {
            customer.EnablePatienceTimer(false);
            SaveManager.Instance.Player.Reputation += 1;
            SaveManager.Instance.Player.DayData.Streak++;
            float streakRewardMult = HandleStreakRewardMult();
            SaveManager.Instance.Player.DayData.Profits += Mathf.FloorToInt(foodMatch.reward * (1 + food.QualityPercent) * streakRewardMult);
            SaveManager.Instance.Player.DayData.CustomersServed++;

            if (itemMatch != null)
            {
                // right food, right item
                DialogueManager.Instance.PlayDialogue(
                    itemMatch.Success,
                    customer.customerData, 
                    () => GameManager.Instance.GameEnd(scenario, itemServed)
                );

                // item is removed from plating tile
                _platingTile.Remove(item);
                item.Destroy();
            }
            else
            {
                // right food, wrong item
                DialogueManager.Instance.PlayDialogue(
                    customer.Dialogue.Success,
                    customer.customerData, 
                    () => GameManager.Instance.GameEnd(scenario, itemServed)
                );
                // item stays on counter
            }
        }
        else
        {
            customer.EnablePatienceTimer(false);
            SaveManager.Instance.Player.Reputation -= 1;
            SaveManager.Instance.Player.DayData.Streak = 0;

            if (itemMatch != null)
            {
                // wrong food, right item
                DialogueManager.Instance.PlayDialogue(
                    itemMatch.Fail,
                    customer.customerData, 
                    () => GameManager.Instance.GameEnd(scenario, itemServed)
                );

                // item is removed from plating tile
                _platingTile.Remove(item);
                item.Destroy();
            }
            else
            {
                // wrong food, wrong item
                DialogueManager.Instance.PlayDialogue(
                    customer.Dialogue.Fail,
                    customer.customerData, 
                    () => GameManager.Instance.GameEnd(scenario, itemServed)
                );
                // item stays on plating tile
            }
        }

        _platingTile.Remove(food);
        food.Destroy();
    }
}
