using UnityEngine.InputSystem;
using UnityEngine;

public class FoodGrab : MonoBehaviour
{
    private Collider2D _col;
    [SerializeField] private Transform _homeSpot;
    [SerializeField] private Transform _plateSpot;
    private StoveTops _activeStove;
    private Countertops _activeCountertop;
    private CookingAppliance _activeAppliance;
    private bool _isPlaced = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _col = GetComponent<Collider2D>();
        
    }

    //On Click: Get mouse position to set up for dragging
    private void OnMouseDown()
    {
        if (_isPlaced) return;
        if (_activeAppliance != null)
        {
            _activeStove.OnRemoveFood();
            _activeStove = null;            
        } else if(_activeCountertop != null)
        {
            _activeCountertop.OnRemoveFood();
            _activeCountertop = null;
        }
        Debug.Log("Click on Food");
    }

    private void OnMouseDrag()
    {
        if (_isPlaced) return;
        transform.position = GetMousePositionInWorldSpace();
    }

    private void OnMouseUp()
    {
        if (_isPlaced) return;
        //If not at stove top, revert back to original position
        //(Keep track of original position)
        // _col.enabled = false;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        // _col.enabled = true;
        foreach (Collider2D hit in hits)
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
            
            /*else if (hit.gameObject.name.Contains("Plate"))
            {
                IngredientObject _currentFood = GetComponent<IngredientObject>();
                if (_currentFood.IngredientInstance.CurrentState == IngredientState.Cooked)
                {
                    transform.position = hit.transform.position;
                    Debug.Log("Snapped to: " + hit.name);
                    return;
                }
            } else if (hit.gameObject.name.Contains("Countertop"))
            {
                Countertops _countertop = hit.GetComponent<Countertops>();
                if (_countertop != null)
                {
                    _activeCountertop = _countertop;
                    transform.position = hit.transform.position;
                    Debug.Log("Snapped to: " + hit.name);
                    _activeCountertop.OnPlaceFood(this);
                    return;
                }
            }*/

            else if (hit.GetComponentInParent<Plate>() != null)
            {
                Plate plate = hit.GetComponentInParent<Plate>();
                IngredientObject food = GetComponent<IngredientObject>();

                plate.AddIngredient(food);

                _activeAppliance = null;

                plate.PrintIngredients();
                return;
            }
        }
        if (_homeSpot != null)
        {
            transform.position = _homeSpot.position;
            _activeStove = null;
            _activeCountertop = null;
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

    public void LockToPlate()
    {
        _isPlaced = true;
        
        if (_col != null)
            _col.enabled = false;
    }

}
