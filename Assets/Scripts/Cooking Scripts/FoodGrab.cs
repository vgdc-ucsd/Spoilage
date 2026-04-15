using UnityEngine.InputSystem;
using UnityEngine;

public class FoodGrab : MonoBehaviour
{
    private Collider2D col;
    [SerializeField] private Transform homeSpot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider2D>();
        
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
        // col.enabled = false;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        // col.enabled = true;
        foreach (Collider2D hit in hits)
        {
            StoveTops stoveTops = hit.GetComponentInParent<StoveTops>();

            if (hit.gameObject.name.Contains("StoveTop"))
            {
                transform.position = hit.transform.position;
                Debug.Log("Snapped to: " + hit.name);
                return;
            }
        }
        if (homeSpot != null)
        {
            transform.position = homeSpot.position;
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
