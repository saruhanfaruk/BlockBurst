using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CornerChecker : MonoBehaviour
{
    #region Fields
    private List<Direction> connectedEdges = new List<Direction>();
    public Image pointImage;

    public Color targetedColor;
    public Material targetedMaterial;

    public Color untargetedColor;
    public Material untargetedMaterial;
    #endregion

    #region Active Control
    /// <summary>
    /// Activates the corner for a specific edge and changes its appearance.
    /// </summary>
    public void SetCornerActive(float colorAlpha,Direction edgeDirection)
    {
        if(!connectedEdges.Contains(edgeDirection))
            connectedEdges.Add(edgeDirection);
        pointImage.material = targetedMaterial;
        pointImage.color = targetedColor.SetAlpha(colorAlpha);
    }
    /// <summary>
    /// Deactivates the corner for a specific edge and reverts its appearance.
    /// </summary>
    public void SetCornerInactive(float colorAlpha, Direction edgeDirection)
    {
        if (connectedEdges.Contains(edgeDirection))
            connectedEdges.Remove(edgeDirection);
        if (connectedEdges.Count==0)
        {
            pointImage.material = untargetedMaterial;
            pointImage.color = untargetedColor.SetAlpha(colorAlpha);
        }
    }
    #endregion
}
