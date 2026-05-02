using UnityEngine;

public class KitchenGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject tilePrefab;
    public int columns;
    public int rows;

    [Range(0.0f, 1.0f)]
    public float tileSpacing = 0.95f;

    [Header("Screen Layout Settings")]
    [Range(0.1f, 1.0f)]
    public float gridAreaWidthPercentage = 0.9f;
    [Range(0.1f, 1.0f)]
    public float gridAreaHeightPercentage = 0.8f;

    [Header("Positioning")]
    public float paddingBottom = 1.5f; // Gap from the bottom edge (leave room for trash/ing)

    void Start()
    {
        GenerateGrid();
    }
    void GenerateGrid()
    {
        if (tilePrefab == null) return;

        Camera cam = Camera.main;
        float screenHeight = 2f * cam.orthographicSize;
        float screenWidth = screenHeight * cam.aspect;

        // 1. Calculate usable width based on your percentage
        float playableWidth = screenWidth * gridAreaWidthPercentage;
        float finalTileSize = playableWidth / columns;

        float totalGridWidth = finalTileSize * columns;
        float totalGridHeight = finalTileSize * rows;

        // 2. NEW POSITIONING LOGIC:
        // Start from the Right Edge and move left by the total width + padding
        float rightEdge = cam.transform.position.x + (screenWidth / 2);
        float startX = rightEdge - totalGridWidth;

        // Start from the Bottom Edge and move up by the total height + padding
        float bottomEdge = cam.transform.position.y - (screenHeight / 2);
        float startY = bottomEdge + totalGridHeight + paddingBottom;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 spawnPos = new Vector3(
                    startX + (x * finalTileSize) + (finalTileSize / 2),
                    startY - (y * finalTileSize) - (finalTileSize / 2),
                    0
                );

                GameObject newTile = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                newTile.transform.localScale = new Vector3(finalTileSize * tileSpacing, finalTileSize * tileSpacing, 1);
                newTile.name = $"Tile_{x}_{y}";
            }
        }
    }
}
