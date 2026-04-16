using UnityEngine.InputSystem;
using UnityEngine;

public class FoodGrab : MonoBehaviour
{
    private Collider2D _col;
    [SerializeField] private Transform _homeSpot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _col = GetComponent<Collider2D>();
        
    }

    //On Click: Get mouse position to set up for dragging
    private void OnMouseDown()
    {
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
            StoveTops stove = hit.GetComponentInParent<StoveTops>();

            if (hit.gameObject.name.Contains("StoveTop"))
            {
                transform.position = hit.transform.position;
                Debug.Log("Snapped to: " + hit.name);
                stove.OnPlaceFood(this);
                return;
            }
        }
        if (_homeSpot != null)
        {
            transform.position = _homeSpot.position;
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
