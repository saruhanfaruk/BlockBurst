using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    #region Fields

    [Header("Grid Settings")]
    public RectTransform gridArea;
    public GameObject gridPrefab;
    public int gridSize = 3;

    [Range(1, 99)]
    public int paddingRatio = 50;

    private const int CellSize = 750;
    private const int Spacing = 600;

    #endregion

    #region Grid Generation

    public AreaController[,] GenerateGrid()
    {
        AreaController[,] gridCells = new AreaController[gridSize, gridSize];

        float padding = (gridArea.rect.width * (paddingRatio / 100f)) * 0.5f;
        float availableSize = gridArea.rect.width - 2 * padding;
        float totalGridSize = CellSize + (gridSize - 1) * Spacing;
        float scale = availableSize / totalGridSize;
        float halfCellSize = (CellSize * scale) * 0.5f;

        float startX = -gridArea.rect.width / 2 + padding + (availableSize - totalGridSize * scale) / 2 + halfCellSize;
        float startY = gridArea.rect.height / 2 - padding - (availableSize - totalGridSize * scale) / 2 - halfCellSize;

        Camera uiCamera = UIManager.Instance.uiCamera;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject newCell = Instantiate(gridPrefab, gridArea);
                AreaController areaController = newCell.GetComponent<AreaController>();
                RectTransform rectTransform = newCell.GetComponent<RectTransform>();

                rectTransform.localScale = Vector3.one * scale;
                float posX = startX + y * (Spacing * scale);
                float posY = startY - x * (Spacing * scale);
                rectTransform.anchoredPosition = new Vector2(posX, posY);

                areaController.Position = RectTransformUtility.WorldToScreenPoint(uiCamera, rectTransform.position);
                areaController.Index = new Vector2Int(x, y);
                areaController.name = $"Cell_{x}_{y}";

                gridCells[x, y] = areaController;
            }
        }

        GridManager.Instance.ShapeScale = scale;
        return gridCells;
    }

    #endregion
}
