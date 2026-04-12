using System.Numerics;
using UnityEngine;

public class FoodGrab : MonoBehaviour
{
    private Collider2D col;
    private Vector3 startDragPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider>();
        
    }

    //On Click: Get mouse position to set up for dragging
    private void OnMouseDown()
    {
        startDragPosition = transform.position;
        transform.position = GetMousePositionInWorldSpace();
    }

    private void OnMouseDrag()
    {
        transform.position = GetMousePositionInWorldSpace();
    }

    private void OnMouseUp()
    {
        //If not at stove top, revert back to original position
        //(Keep track of original position)
        
    }

    private Vector3 GetMousePositionInWorldSpace()
    {
        Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        p * z = 0f;
        return p;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
