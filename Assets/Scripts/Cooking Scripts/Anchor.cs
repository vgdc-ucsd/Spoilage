using UnityEngine;

public class WorldAnchor : MonoBehaviour
{
    public enum AnchorPoint { BottomLeft, BottomRight, Center }
    public AnchorPoint point;
    public Vector2 offset;

    void Start()
    {
        UpdatePosition();
    }

    // We use LateUpdate so it adjusts if you resize the window in the editor
    void LateUpdate()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // 1. Calculate World Space dimensions of the camera view
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        Vector3 newPos = cam.transform.position;

        // 2. Adjust based on your chosen anchor point
        if (point == AnchorPoint.BottomLeft)
        {
            newPos.x -= width / 2;
            newPos.y -= height / 2;
        }
        else if (point == AnchorPoint.BottomRight)
        {
            newPos.x += width / 2;
            newPos.y -= height / 2;
        }

        // 3. Apply offset and lock Z to 0
        transform.position = new Vector3(newPos.x + offset.x, newPos.y + offset.y, 0);
    }
}
