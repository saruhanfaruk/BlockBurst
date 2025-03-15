using UnityEngine;
using System.Collections.Generic;

public class ShapeSpawner : MonoBehaviour
{
    public RectTransform panelArea; 
    public List<GameObject> shapePrefabs; 
    public int shapeCount = 5; 

    private List<ShapeController> activeShapes = new List<ShapeController>();

    /// <summary>
    /// Spawns new shapes inside the panel area.
    /// </summary>
    public void SpawnShapes()
    {
        if (shapePrefabs.Count == 0) return;

        float targetScale = GridManager.Instance.ShapeScale;
        float currentScale = targetScale * 0.9f;

        float panelWidth = panelArea.rect.width;
        float panelHeight = panelArea.rect.height;

        float spacing = 20f; // Spacing between shapes
        float maxSize = panelWidth / shapeCount - spacing; // Maximum shape size to fit in the panel

        for (int i = 0; i < shapeCount; i++)
        {
            int randomIndex = Random.Range(0, shapePrefabs.Count);
            GameObject shape = Instantiate(shapePrefabs[randomIndex], panelArea);
            RectTransform shapeRect = shape.GetComponent<RectTransform>();

            shapeRect.localScale = Vector3.one * currentScale;

            // Calculate position (aligned from left to right)
            float xPos = -panelWidth / 2 + (i * (maxSize + spacing)) + maxSize / 2;
            shapeRect.anchoredPosition = new Vector2(xPos, 0);

            ShapeController shapeController = shape.GetComponent<ShapeController>();
            shapeController.ShapeSpawner = this;
            activeShapes.Add(shapeController);
        }
    }

    /// <summary>
    /// Clears a shape and respawns new ones if none are left.
    /// </summary>
    public void ClearShapes(ShapeController shapeController)
    {
        activeShapes.Remove(shapeController);
        Destroy(shapeController.gameObject); 

        if (activeShapes.Count == 0)
            SpawnShapes();
    }
}
