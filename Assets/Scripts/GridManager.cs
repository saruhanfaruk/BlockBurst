using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    #region Fields
    public GridGenerator gridGenerator;
    float shapeScale;
    public float ShapeScale {  get { return shapeScale; } set { shapeScale = value; } }
    private AreaController[,] gridCells;

    [HideInInspector]
    public List<Direction> allDirections = new();
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++)
            allDirections.Add((Direction)i);
    }
    #endregion

    #region Grid Operations
    public void CreateGrid()
    {
        gridCells = gridGenerator.GenerateGrid();
    }
    /// <summary>
    /// Finds the nearest area to the given position.
    /// </summary>
    /// <param name="shapePosition">The position to check.</param>
    /// <returns>The nearest AreaController.</returns>
    public AreaController FindNearestArea(Vector3 shapePosition)
    {
        AreaController nearestArea = null;
        float nearestDistance = Mathf.Infinity;
        for (int x = 0; x < gridGenerator.gridSize; x++)
        {
            for (int y = 0; y < gridGenerator.gridSize; y++)
            {
                AreaController currentArea = gridCells[x, y];
                float distanceToStart = Vector2.Distance(shapePosition, currentArea.Position);
                if (distanceToStart < nearestDistance)
                {
                    nearestDistance = distanceToStart;
                    nearestArea = currentArea;
                }
            }
        }
        return nearestArea;
    }
    /// <summary>
    /// Finds the nearest valid area where a shape can be placed.
    /// </summary>
    /// <param name="shapePosition">The position of the shape.</param>
    /// <param name="edgeDirections">List of edges occupied by the shape.</param>
    /// <param name="nearestArea">Optional nearest area (if already found).</param>
    /// <returns>The nearest valid AreaController or null if no valid area exists.</returns>
    public AreaController FindValidNearestArea(Vector3 shapePosition, List<Direction> edgeDirections,AreaController nearestArea=null)
    {
        if(nearestArea==null)
            nearestArea = FindNearestArea(shapePosition);
        if (nearestArea != null)
        {
            float nearestDistance = Vector2.Distance(shapePosition, nearestArea.Position);
            RectTransform nearestAreaRectTransform = nearestArea.GetComponent<RectTransform>();
            float halfSize = (nearestAreaRectTransform.sizeDelta.x * nearestAreaRectTransform.localScale.x) / 2;
            if (nearestDistance > halfSize)
                return null;
            foreach (var item in edgeDirections)
            {
                if (nearestArea.occupancyByDirection[item])
                    return null;
            }
        }
        return nearestArea;
    }
    /// <summary>
    /// Returns the neighboring areas and their directions for a given area.
    /// </summary>
    /// <param name="area">The area whose neighbors will be found.</param>
    /// <param name="edgeDirections">The directions to check.</param>
    /// <returns>A dictionary containing neighboring directions and their respective areas.</returns>
    public Dictionary<Direction, AreaController> GetNeighboringAreas(AreaController area, List<Direction> edgeDirections)
    {
        Dictionary<Direction, AreaController> neighbors = new Dictionary<Direction, AreaController>();
        Vector2Int areaIndex = area.Index;

        foreach (var direction in edgeDirections)
        {
            Vector2Int neighborIndex = GetNeighborIndex(areaIndex, direction);

            if (IsValidGridIndex(neighborIndex))
            {
                neighbors.Add(direction, gridCells[neighborIndex.x, neighborIndex.y]);
            }
        }
        return neighbors;
    }

    /// <summary>
    /// Returns the neighboring area in the specified direction.
    /// </summary>
    /// <param name="area">The area whose neighbor will be found.</param>
    /// <param name="edgeDirection">The direction to check.</param>
    /// <returns>The neighboring area or null if invalid.</returns>
    public AreaController GetNeighboringArea(AreaController area, Direction edgeDirection)
    {
        Vector2Int neighborIndex = GetNeighborIndex(area.Index, edgeDirection);

        if (IsValidGridIndex(neighborIndex))
            return gridCells[neighborIndex.x, neighborIndex.y];

        return null;
    }

    /// <summary>
    /// Calculates the neighboring index in the given direction.
    /// </summary>
    /// <param name="currentIndex">The index of the current area.</param>
    /// <param name="direction">The direction to move.</param>
    /// <returns>The new neighboring index.</returns>
    private Vector2Int GetNeighborIndex(Vector2Int currentIndex, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                currentIndex.x--;
                break;
            case Direction.Down:
                currentIndex.x++;
                break;
            case Direction.Left:
                currentIndex.y--;
                break;
            case Direction.Right:
                currentIndex.y++;
                break;
        }
        return currentIndex;
    }

    #endregion
    #region Grid Completion Check

    /// <summary>
    /// Checks if any row or column is completely filled and triggers clearing if necessary.
    /// </summary>
    public void CheckAndTriggerFullRowOrColumn()
    {
        CheckFullRows();
        CheckFullColumns();
    }

    /// <summary>
    /// Checks for completely filled rows and clears them.
    /// </summary>
    private void CheckFullRows()
    {
        int gridSize = gridGenerator.gridSize;

        for (int x = 0; x < gridSize; x++)
        {
            bool isRowCompleted = true;

            for (int y = 0; y < gridSize; y++)
            {
                if (!gridCells[x, y].IsCompleted)
                {
                    isRowCompleted = false;
                    break;
                }
            }

            if (isRowCompleted)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    gridCells[x, y].ClearCompletedArea();
                }
            }
        }
    }

    /// <summary>
    /// Checks for completely filled columns and clears them.
    /// </summary>
    private void CheckFullColumns()
    {
        int gridSize = gridGenerator.gridSize;

        for (int y = 0; y < gridSize; y++)
        {
            bool isColumnCompleted = true;

            for (int x = 0; x < gridSize; x++)
            {
                if (!gridCells[x, y].IsCompleted)
                {
                    isColumnCompleted = false;
                    break;
                }
            }

            if (isColumnCompleted)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    gridCells[x, y].ClearCompletedArea();
                }
            }
        }
    }

    #endregion
    #region Utility Methods

    /// <summary>
    /// Checks if the given grid index is within valid bounds.
    /// </summary>
    /// <param name="index">The grid index to check.</param>
    /// <returns>True if the index is valid, otherwise false.</returns>
    private bool IsValidGridIndex(Vector2Int index)
    {
        return index.x >= 0 && index.x < gridGenerator.gridSize &&
               index.y >= 0 && index.y < gridGenerator.gridSize;
    }

    #endregion
}
