using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ServingStation : KitchenTile
{
    public UnityEvent PlateSubmitted;
    private CustomerOrderDatabase _customerOrderDatabase;

    private void Start()
    {
        _customerOrderDatabase = CustomerOrderDatabase.Instance;
        if (PlateSubmitted == null)
            PlateSubmitted = new UnityEvent();

        CustomerLineManager.Instance.PlateSubmitted = PlateSubmitted;

        PlateSubmitted.AddListener(OnPlateSubmitted);
    }

    public override void PlaceObject(GameObject obj)
    {
        base.PlaceObject(obj);

        //all the serving station needs to do is just plate the ingredient at this point
        IngredientBehaviour existingFood = null;

        // Find existing food on tile
        foreach (var item in objectsOnTile)
            if (item != null && item.TryGetComponent(out existingFood)) break;

        //change to plated sprite
        if (existingFood != null)
        {
            existingFood.PlateIngredient();
        }
    }

    public IngredientObject GetIngredient()
    {
        IngredientObject existingFood = null;

        // Find existing food on tile
        foreach (var item in objectsOnTile)
            if (item != null && item.TryGetComponent(out existingFood)) break;

        return existingFood;
    }

    public override bool CanPlaceObject(string type, GameObject movingObj = null)
    {
        //this type doesn't accept appliances
        if (type == "Appliance") return false;

        return base.CanPlaceObject(type, movingObj);
    }

    public override void RemoveObject(GameObject obj)
    {
        IngredientBehaviour behaviour = obj.GetComponent<IngredientBehaviour>();
        behaviour.UnplateIngredient();
        
        base.RemoveObject(obj);
    }

    void OnPlateSubmitted()
    {
        IngredientObject existingFood = null;

        // Find existing food on tile
        foreach (var item in objectsOnTile)
            if (item != null && item.TryGetComponent(out existingFood)) break;

        if (_customerOrderDatabase.SubmitOrder(existingFood))
        {
            foreach (GameObject obj in objectsOnTile)
            {
                Destroy(gameObject);
            }
        }
    }
}