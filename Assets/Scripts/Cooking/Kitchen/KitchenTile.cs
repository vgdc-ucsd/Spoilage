using UnityEngine;
using System.Collections.Generic;

public class KitchenTile : MonoBehaviour
{
    [SerializeField] private GameObject _counterPrefab;

    [Header("Tile Inventory")]
    public List<GameObject> objectsOnTile = new List<GameObject>();

    private RectTransform _rectTransform;
    private string _station = "Kitchen Tile";

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (LockLayout.Instance != null)
            LockLayout.Instance.RegisterTile(this);
        else
            Debug.LogWarning($"KitchenTile '{name}': LockLayout instance not found!");
    }

    public bool CanPlaceObject(string type, GameObject movingObj = null)
    {
        // Always allow re-snapping the same object
        if (movingObj != null && objectsOnTile.Contains(movingObj)) return true;

        if (SetupManager.Instance == null)
        {
            Debug.LogWarning("SetupManager instance not found!");
            return false;
        }

        if (SetupManager.Instance.CurrentPhase == GamePhase.Setup)
        {
            // Setup: appliances only, one per tile
            if (type != "Appliance") return false;

            foreach (var obj in objectsOnTile)
                if (obj != null && obj.TryGetComponent(out ObjectGrab _)) return false;

            return true;
        }

        if (SetupManager.Instance.CurrentPhase == GamePhase.Cooking)
        {
            // Cooking: food only, one per tile
            if (type != "Food") return false;

            IngredientObject existingFood = null;
            foreach (var obj in objectsOnTile)
                if (obj != null && obj.TryGetComponent(out existingFood)) break;

            //if empty, place food
            if (existingFood == null) return true;

            //if a food item is alr placed we combine
            IngredientObject movingFood = movingObj.GetComponent<IngredientObject>();
            RecipeManager recipeManager = FindAnyObjectByType<RecipeManager>();

            if (movingFood != null && recipeManager != null)
            {
                // Create a temporary list to check the recipe
                List<IngredientObject> checkList = new List<IngredientObject> { existingFood, movingFood };
                string result = recipeManager.CheckRecipe(checkList, _station);
                return result != "JSON Error";
            }
            return false;
        }

        return false;
    }

    public void PlaceObject(GameObject obj)
    {
        if (obj == null) return;

        IngredientObject newFood = obj.GetComponent<IngredientObject>();
        IngredientObject existingFood = null;

        // Find existing food on tile
        foreach (var item in objectsOnTile)
            if (item != null && item != obj && item.TryGetComponent(out existingFood)) break;

        // If both exist, try to combine via RecipeManager
        if (newFood != null && existingFood != null)
        {
            RecipeManager rm = FindAnyObjectByType<RecipeManager>();
            List<IngredientObject> combo = new List<IngredientObject> { existingFood, newFood };

            float averageSpoilage = rm.GetAverageSpoilage(combo);

            string resultName = rm.CheckRecipe(combo, _station);

            IngredientData resultData;

            if (resultName != "Slop" && resultName != "JSON Error")
            {
                //Valid recipe
                resultData = IngredientLookup.Get(resultName); 
                Recipe matchedRecipe = System.Array.Find(rm.allRecipes.allRecipes, r => r.name == resultName);

                if (resultData != null)
                {
                    // PRINT SUCCESS HERE
                    Debug.Log($"<color=green>SUCCESS:</color> Combined {existingFood.IngredientInstance.Data.Name} + {newFood.IngredientInstance.Data.Name} into <b>{resultData.Name}</b>");
                    resultData.QualityPercent = rm.CalculateTotalQuality(combo);
                    Debug.Log("Quality:" + resultData.QualityPercent);
                    existingFood.ChangeIngredient(resultData);

                    if (matchedRecipe != null && matchedRecipe.spoiled)
                    {
                        // Stage II dishes are born rotten
                        existingFood.IngredientInstance.SetSpoilagePercent(100f);
                    }
                    else
                    {
                        // Normal dishes use the averaging math
                        float avg = rm.GetAverageSpoilage(combo);
                        existingFood.IngredientInstance.SetSpoilagePercent(avg);
                    }
                }
            }
            else
            {
                // Invalid combo! Turn into "Slop"
                resultData = IngredientLookup.Get("Slop");
                Debug.Log("Invalid combination! Turning into Slop.");
            }

            if (resultData != null)
            {
                existingFood.ChangeIngredient(resultData);

                existingFood.IngredientInstance.SetSpoilagePercent(averageSpoilage);

                IngredientBehaviour newBehaviour = obj.GetComponent<IngredientBehaviour>();
                if (newBehaviour != null) newBehaviour.RemoveFromSpoilSurface();

                Destroy(obj); // Remove the held ingredient

                IngredientBehaviour existingBehaviour = existingFood.GetComponent<IngredientBehaviour>();
                if (existingBehaviour != null) existingBehaviour.PutOnSpoilSurface();

                return;
            }
        }

        if (!objectsOnTile.Contains(obj))
            objectsOnTile.Add(obj);

        // Snap to tile using RectTransform (all objects are UI)
        RectTransform objRect = obj.GetComponent<RectTransform>();
        if (objRect != null && _rectTransform != null)
        {
            objRect.SetParent(_rectTransform, false);
            objRect.anchoredPosition = Vector2.zero;
            objRect.SetAsLastSibling();

            Canvas foodCanvas = obj.GetComponent<Canvas>();
            if (foodCanvas == null)
                foodCanvas = obj.AddComponent<Canvas>();
            foodCanvas.overrideSorting = true;
            foodCanvas.sortingOrder = 10;

            if (obj.GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
            obj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
        else
        {
            Debug.LogWarning($"KitchenTile '{name}': Could not snap '{obj.name}' — missing RectTransform.");
        }
        IngredientBehaviour behaviour = obj.GetComponent<IngredientBehaviour>();
        if (behaviour != null) behaviour.PutOnSpoilSurface();

        // Plating test
        // behaviour.PlateIngredient();
    }

    public void RemoveObject(GameObject obj)
    {
        objectsOnTile.Remove(obj);
        IngredientBehaviour behaviour = obj.GetComponent<IngredientBehaviour>();
        if (behaviour != null) behaviour.RemoveFromSpoilSurface();
        
        // Unplating test
        // behaviour.UnplateIngredient();
    }

    public GameObject GetTopObject()
    {
        // Clean out any destroyed objects first
        objectsOnTile.RemoveAll(o => o == null);
        return objectsOnTile.Count > 0 ? objectsOnTile[objectsOnTile.Count - 1] : null;
    }

    public bool HasAppliance()
    {
        foreach (var obj in objectsOnTile)
            if (obj != null && obj.TryGetComponent(out ObjectGrab _)) return true;
        return false;
    }

    public bool HasFood()
    {
        foreach (var obj in objectsOnTile)
            if (obj != null && obj.GetComponent<IngredientObject>() != null) return true;
        return false;
    }

    public void SetCountertopIfEmpty()
    {
        if (_counterPrefab == null)
        {
            Debug.LogWarning($"KitchenTile '{name}': Counter prefab not assigned.");
            return;
        }

        if (objectsOnTile.Count == 0)
        {
            GameObject counter = Instantiate(_counterPrefab, transform);
            RectTransform counterRect = counter.GetComponent<RectTransform>();
            if (counterRect != null) counterRect.anchoredPosition = Vector2.zero;
            PlaceObject(counter);
        }
    }
}