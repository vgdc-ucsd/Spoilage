using UnityEngine.InputSystem;
using UnityEngine;

public class FoodGrab : MonoBehaviour
{
    private Collider2D _col;
    [SerializeField] private Transform _homeSpot;
    [SerializeField] private Transform _plateSpot;
    private CookingAppliance _activeAppliance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _col = GetComponent<Collider2D>();
        
    }

    //On Click: Get mouse position to set up for dragging
    private void OnMouseDown()
    {
        if (_activeAppliance != null)
        {
            _activeAppliance.OnRemoveFood();
            _activeAppliance = null;
        }
        Debug.Log("Click on Food");
    }

    private void OnMouseDrag()
    {
        transform.position = GetMousePositionInWorldSpace();
    }

    private void OnMouseUp()
    {
        //If not at stove top, revert back to original position
        //(Keep track of original position)
        // _col.enabled = false;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        // _col.enabled = true;
        foreach (Collider2D hit in hits)
        {
            CookingAppliance app = hit.GetComponentInParent<CookingAppliance>();

            if (hit.gameObject.name.Contains("StoveTop") || hit.gameObject.name.Contains("Pot"))
            {
                CookingAppliance _app = hit.GetComponentInParent<CookingAppliance>();
                if (_app != null)
                {
                    _activeAppliance = _app;
                    transform.position = hit.transform.position;
                    Debug.Log("Snapped to: " + hit.name);
                    _activeAppliance.OnPlaceFood(this);
                    return;
                }
            }
            else if (hit.gameObject.name.Contains("Plate"))
            {
                IngredientObject _currentFood = GetComponent<IngredientObject>();
                if (_currentFood.IngredientInstance.CurrentState == IngredientState.Cooked)
                {
                    transform.position = hit.transform.position;
                    Debug.Log("Snapped to: " + hit.name);
                    return;
                }
            }
        }
        if (_homeSpot != null)
        {
            transform.position = _homeSpot.position;
            _activeAppliance = null;
        }
        
    }

    private Vector3 GetMousePositionInWorldSpace()
    {
        if (Camera.main == null || Mouse.current == null)
        {
            return transform.position;
        }
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        worldPosition.z = 0f;
        return worldPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
