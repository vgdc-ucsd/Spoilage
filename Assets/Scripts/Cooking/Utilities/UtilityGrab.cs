using UnityEngine.InputSystem;
using UnityEngine;

public class UtilityGrab : MonoBehaviour
{

    private Collider2D _col;
    [SerializeField] private Transform _homeSpot;
    [SerializeField] private Transform _plateSpot;
    private UtilityStation _activeUtility;
    private bool _isPlaced = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _col = GetComponent<Collider2D>();
    }

    //On Click: Get mouse position to set up for dragging
    private void OnMouseDown()
    {
        //if (_isPlaced) return;
        
        //new
        if (_isPlaced)
        {
            _isPlaced = false;
            
            if (_col != null)
                _col.enabled = false;
        }
        //new

        if (_activeUtility != null)
        {
            _activeUtility.OnRemoveFood();
            _activeUtility = null;
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
        //if (_isPlaced) return;
        
        //new
        if (_isPlaced)
        {
            _isPlaced = false;
            
            if (_col != null)
                _col.enabled = false;
        }
        //new
        //new
        //If not at stove top, revert back to original position
        //(Keep track of original position)
        // _col.enabled = false;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        // _col.enabled = true;
        foreach (Collider2D hit in hits)
        {
            UtilityStation _util = hit.GetComponentInParent<UtilityStation>();

            if (_util != null)
            {
                _activeUtility = _util;
                transform.position = hit.transform.position;
                Debug.Log("Snapped to: " + hit.name);
                _activeUtility.OnPlaceFood(this);
                return;
            }

        }
        if (_homeSpot != null)
        {
            transform.position = _homeSpot.position;
            _activeUtility = null;
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
