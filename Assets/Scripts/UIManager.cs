using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    #region Fields
    public RectTransform gameBackground;
    public RectTransform shapeSpawnerPanel;
    public ShapeSpawner shapeSpawner;
    public Camera uiCamera;
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        AdjustUIElements();
    }

    private void Start()
    {
        StartGame();
    }
    #endregion

    #region UI Adjustment
    /// <summary>
    /// Adjusts the size of UI elements based on screen dimensions.
    /// </summary>
    private void AdjustUIElements()
    {
        float size = (Screen.height - Screen.width) / 2;
        if (gameBackground != null)
            gameBackground.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.width);
        if (shapeSpawnerPanel != null)
            shapeSpawnerPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
    }
    #endregion

    #region Game Control
    /// <summary>
    /// Starts the game by creating the grid and spawning shapes.
    /// </summary>
    public void StartGame()
    {
        if (GridManager.Instance != null)
            GridManager.Instance.CreateGrid();

        if (shapeSpawner != null)
            shapeSpawner.SpawnShapes();
    }
    #endregion
}
