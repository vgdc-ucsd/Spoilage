using UnityEngine;

public class CookingAppliance : MonoBehaviour
{
    [Header("Base Appliance Settings")]
    // This basically tells us the appliance if it's a toaster (Toasted), grill (Grilled), etc.
    [SerializeField] protected CookState targetState;


    // These virtual methods allow child scripts to add their own unique logic
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnPlaceFood(FoodGrab food)
    {
        Debug.Log($"{gameObject.name}: Food placed.");
    }

    public virtual void OnRemoveFood()
    {
        Debug.Log($"{gameObject.name}: Food removed.");
    }
}
