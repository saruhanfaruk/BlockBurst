using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EdgeChecker : MonoBehaviour
{
    #region Fields

    public RectTransform rectTransform;
    public Image edgeImage;
    public Color edgeColor;
    #endregion


    #region Active Control
    /// <summary>
    /// Activates the corner for a specific edge and changes its appearance.
    /// </summary>
    public void SetEdgeActive(float colorAlpha)
    {
        gameObject.SetActive(true);
        edgeImage.color = edgeColor.SetAlpha(colorAlpha);
    }
    /// <summary>
    /// Deactivates the corner for a specific edge and reverts its appearance.
    /// </summary>
    public void SetEdgeInactive(float colorAlpha)
    {
        gameObject.SetActive(false);
        edgeImage.color = edgeColor.SetAlpha(colorAlpha);
    }
    #endregion
}
