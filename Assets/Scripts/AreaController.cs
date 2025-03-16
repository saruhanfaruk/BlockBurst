using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class AreaController : SerializedMonoBehaviour
{
    #region Fields
    public bool isCompleted;
    public bool IsCompleted => isCompleted;
    public Image background;
    private Vector2 position; 
    public Vector2 Position {  get { return position; } set { position = value; } }

    private Vector2Int index;
    public Vector2Int Index { get { return index; } set { index = value; } }


    public Dictionary<Direction, EdgeChecker> shapeEdges = new Dictionary<Direction, EdgeChecker>();
    public Dictionary<Direction, List<CornerChecker>> cornerCheckers = new Dictionary<Direction, List<CornerChecker>>();
    [HideInInspector]
    public Dictionary<Direction, bool> occupancyByDirection = new Dictionary<Direction, bool>();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        position = transform.position;
        foreach (var item in shapeEdges)
            occupancyByDirection.Add(item.Key, false);
    }
    #endregion

    #region Edge
    /// <summary>
    /// Marks the specified edge as targeted and updates related UI elements.
    /// </summary>
    public void SetEdgeActive(Direction edgeDirection)
    {
        occupancyByDirection[edgeDirection] = true;
        shapeEdges[edgeDirection].SetEdgeActive(1);
        foreach (var item in cornerCheckers[edgeDirection])
            item.SetCornerActive(1,edgeDirection);
        CheckIfAllEdgesAreFilled();
    }
    /// <summary>
    /// Unmarks the specified edge and resets UI elements.
    /// </summary>
    public void SetEdgeInactive(Direction edgeDirection)
    {
        shapeEdges[edgeDirection].SetEdgeInactive(1);
        occupancyByDirection[edgeDirection] = false;
        foreach (var item in cornerCheckers[edgeDirection])
            item.SetCornerInactive(1, edgeDirection);
    }

    /// <summary>
    /// Resets all edges and updates neighbors accordingly.
    /// </summary>
    private void ResetEdges()
    {
        List<Direction> allDirections = GridManager.Instance.allDirections;
        foreach (var direction in allDirections)
        {
            AreaController neighboringArea = GridManager.Instance.GetNeighboringArea(this, direction);
            if(neighboringArea != null)
            {
                if(!neighboringArea.isCompleted)
                {
                    SetEdgeInactive(direction);
                    neighboringArea.SetEdgeInactive(Extensions.GetOppositeDirection(direction));
                }
            }
            else
                SetEdgeInactive(direction);
        }
    }
    #endregion

    #region Preview
    /// <summary>
    /// Displays a preview of the shape with given edges.
    /// </summary>
    public void ShowPreviewShape(List<Direction> activeShapeEdges)
    {
        foreach (var activeShapeEdge in activeShapeEdges)
        {
            shapeEdges[activeShapeEdge].gameObject.SetActive(true);
            shapeEdges[activeShapeEdge].edgeImage.color = shapeEdges[activeShapeEdge].edgeColor.SetAlpha(.3f);
            foreach (var item in cornerCheckers[activeShapeEdge])
                item.SetCornerActive(.3f, activeShapeEdge);
        }
    }
    /// <summary>
    /// Hides the preview shape with given edges.
    /// </summary>
    public void HidePreviewShape(List<Direction> activeShapeEdges)
    {
        foreach (var activeShapeEdge in activeShapeEdges)
        {
            shapeEdges[activeShapeEdge].gameObject.SetActive(false);
            shapeEdges[activeShapeEdge].edgeImage.color = shapeEdges[activeShapeEdge].edgeColor.SetAlpha(1);
            foreach (var item in cornerCheckers[activeShapeEdge])
                item.SetCornerInactive(1, activeShapeEdge);
        }
    }
    #endregion
    #region Control
    /// <summary>
    /// Checks if all edges are filled and triggers area completion.
    /// </summary>
    public void CheckIfAllEdgesAreFilled()
    {
        foreach (var item in occupancyByDirection)
            if (!item.Value)
                return;
        background.enabled = true;
        background.transform.localScale = Vector3.zero;
        background.transform.DOScale(Vector3.one, .35f).OnComplete(() => {

            isCompleted = true;
            GridManager.Instance.CheckAndTriggerFullRowOrColumn();
        });
    }
    /// <summary>
    /// Clears the completed area and resets its state.
    /// </summary>
    public void ClearCompletedArea()
    {
        Vector3 initialPosition = transform.position;
        isCompleted = false;
        transform.DOShakePosition(.3f, 7, 25).OnComplete(() =>
        {
            transform.position = initialPosition;
            ResetEdges();
            background.transform.DOScale(Vector3.zero, .3f).OnComplete(() => background.enabled = false);
        });

    }
    #endregion

    [Button]
    public void FillAllEdgesForTest()
    {
        for (int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++)
        {
            SetEdgeActive((Direction)i);
            foreach (var neighbor in GridManager.Instance.GetNeighboringAreas(this, new List<Direction> { (Direction)i }))
                neighbor.Value.SetEdgeActive(Extensions.GetOppositeDirection(neighbor.Key));
        }
    }
}
