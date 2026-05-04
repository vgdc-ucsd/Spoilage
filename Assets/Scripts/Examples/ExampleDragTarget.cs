using UnityEngine;

public class ExampleDragTarget : MonoBehaviour
{
    // This is not an official function and can be named and do whatever you want
    public void OnDrop(ExampleDraggable draggable)
    {
        Debug.Log("Received drop from: " + draggable.name);
    }
}
