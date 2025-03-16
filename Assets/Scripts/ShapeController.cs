using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShapeController : MonoBehaviour, IDragHandler, IPointerDownHandler,IPointerUpHandler
{
    #region Fields

    private ShapeSpawner shapeSpawner;
    public ShapeSpawner ShapeSpawner { get { return shapeSpawner; } set { shapeSpawner = value; } }

    public EdgeType edgeType;

    [ShowIf("@edgeType==EdgeType.Normal")]
    public List<Direction> edgeControlDirections = new List<Direction>();

    private RectTransform rectTransform;
    private Camera uiCamera;
    private Vector2 dragOffset;
    private Vector2 startingOffSet;
    private AreaController lastPreviewArea;
    private Vector3 startPos;

    #endregion

    #region Unity Methods

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        uiCamera = UIManager.Instance.uiCamera;
        startPos = rectTransform.position;
        startingOffSet = new Vector2(0, 1);
    }

    #endregion

    #region Pointer
    /// <summary>
    /// Called when the pointer is pressed down on the shape.
    /// Captures the initial drag offset and updates the shape's position.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, uiCamera, out Vector3 worldPoint);
        dragOffset = (Vector2)rectTransform.position - (Vector2)worldPoint;
        rectTransform.position = worldPoint + (Vector3)dragOffset + (Vector3)startingOffSet;
    }
    /// <summary>
    /// Called when the pointer is released from the shape.
    /// Finalizes the shape's position and handles the drop logic.
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, uiCamera, out Vector3 worldPoint))
        {
            rectTransform.position = worldPoint + (Vector3)dragOffset + (Vector3)startingOffSet;
            HandleDrop();
        }
    }

    #endregion

    #region Drag Operations

    /// <summary>
    /// Handles the dragging logic.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, uiCamera, out Vector3 worldPoint))
        {
            rectTransform.position = worldPoint + (Vector3)dragOffset + (Vector3)startingOffSet;
            UpdatePreviewArea();
        }
    }

    #endregion

    #region Shape Placement

    /// <summary>
    /// Updates the preview area while dragging.
    /// </summary>
    private void UpdatePreviewArea()
    {
        if (lastPreviewArea != null)
        {
            lastPreviewArea.HidePreviewShape(edgeControlDirections);
            lastPreviewArea = null;
        }

        if (edgeType != EdgeType.Normal)
        {
            edgeControlDirections.Clear();
            AreaController nearestArea = GridManager.Instance.FindNearestArea(GetShapePosition());
            if (nearestArea != null)
                edgeControlDirections.Add(GetClosestEdge(nearestArea));
        }

        AreaController currentPreviewArea = GridManager.Instance.FindValidNearestArea(GetShapePosition(), edgeControlDirections);
        if (currentPreviewArea != null)
        {
            if (lastPreviewArea != null && lastPreviewArea != currentPreviewArea)
                lastPreviewArea.HidePreviewShape(edgeControlDirections);

            lastPreviewArea = currentPreviewArea;
            currentPreviewArea.ShowPreviewShape(edgeControlDirections);
        }
    }

    /// <summary>
    /// Handles shape placement when dropped.
    /// </summary>
    private void HandleDrop()
    {
        AreaController targetArea = GridManager.Instance.FindValidNearestArea(GetShapePosition(), edgeControlDirections);

        if (targetArea != null)
        {
            ValidateEdges(targetArea);
            shapeSpawner.ClearShapes(this);
            Destroy(gameObject);
        }
        else
        {
            rectTransform.position = startPos;
        }
    }

    /// <summary>
    /// Validates edges and updates the grid accordingly.
    /// </summary>
    public void ValidateEdges(AreaController areaController)
    {
        foreach (var direction in edgeControlDirections)
            areaController.SetEdgeActive(direction);
        foreach (var neighbor in GridManager.Instance.GetNeighboringAreas(areaController, edgeControlDirections))
        {
            //Debug.Log("Komþu Ýsmi: " + neighbor.Value.name + "   Direction: " + Extensions.GetOppositeDirection(neighbor.Key) + "    Hedef Area: " + areaController.name);
            neighbor.Value.SetEdgeActive(Extensions.GetOppositeDirection(neighbor.Key));
        }
            
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Gets the current shape position in screen coordinates.
    /// </summary>
    public Vector2 GetShapePosition()
    {
        return RectTransformUtility.WorldToScreenPoint(uiCamera, rectTransform.position);
    }

    /// <summary>
    /// Determines the closest edge of the given area.
    /// </summary>
    public Direction GetClosestEdge(AreaController area)
    {
        Vector3 shapePosition = rectTransform.position;
        Direction closestEdge = Direction.Up;
        float minDistance = Mathf.Infinity;

        bool isHorizontal = edgeType == EdgeType.HorizontalLine;
        if (isHorizontal)
        {
            float upDistance = Vector2.Distance(shapePosition, area.shapeEdges[Direction.Up].rectTransform.position);
            float downDistance = Vector2.Distance(shapePosition, area.shapeEdges[Direction.Down].rectTransform.position);

            if (upDistance < minDistance)
            {
                minDistance = upDistance;
                closestEdge = Direction.Up;
            }
            if (downDistance < minDistance)
            {
                minDistance = downDistance;
                closestEdge = Direction.Down;
            }
        }
        else
        {
            float rightDistance = Vector2.Distance(shapePosition, area.shapeEdges[Direction.Right].rectTransform.position);
            float leftDistance = Vector2.Distance(shapePosition, area.shapeEdges[Direction.Left].rectTransform.position);

            if (rightDistance < minDistance)
            {
                minDistance = rightDistance;
                closestEdge = Direction.Right;
            }
            if (leftDistance < minDistance)
            {
                minDistance = leftDistance;
                closestEdge = Direction.Left;
            }
        }

        return closestEdge;
    }

    #endregion
}
