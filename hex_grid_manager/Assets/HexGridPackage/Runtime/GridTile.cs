// GridTile.cs
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GridTile : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Coords")]
    public int ColNum { get; set; }
    public int RowNum { get; set; }
    public List<GridTile> Neighbors { get; set; }

    [Header("Cost")]
    public int MovementCost = 1;

    [Header("Occupancy")]
    public bool IsTaken { get; private set; }
    public DemoUnit CurrentUnit { get; private set; }

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer tileRenderer;
    [SerializeField] private SpriteRenderer baseRenderer;
    [SerializeField] private SpriteRenderer overlayRenderer;
    [SerializeField] private SpriteRenderer hoverRenderer;

    [Header("Colors")]
    [SerializeField] private Color hoverColor    = Color.yellow;
    [SerializeField] private Color moveColor     = Color.blue;
    [SerializeField] private Color attackColor   = Color.red;
    [SerializeField] private Color targetColor   = new Color(0f,0.5f,0.8f,0.7f);
    [SerializeField] private Color friendlyColor = Color.green;
    [SerializeField] private Color enemyColor    = new Color(1f,0f,1f,0.3f);
    [SerializeField] private Color selectedColor = new Color(1f,0.65f,0f);

    private bool isSelected;
    private HexGridManager gridManager;

    private void Awake()
    {
        baseRenderer.gameObject.SetActive(false);
        overlayRenderer.gameObject.SetActive(false);
        hoverRenderer.gameObject.SetActive(false);
        gridManager = FindFirstObjectByType<HexGridManager>();
    }

    #region Occupancy
    public void Occupy(DemoUnit unit)
    {
        IsTaken = true;
        CurrentUnit = unit;
        UpdateBase();
    }

    public void UnOccupy()
    {
        IsTaken = false;
        CurrentUnit = null;
        isSelected = false;
        UpdateBase();
    }

    private void UpdateBase()
    {
        if (CurrentUnit == null)
            baseRenderer.gameObject.SetActive(false);
        else
        {
            baseRenderer.color = CurrentUnit.IsAiUnit ? enemyColor : friendlyColor;
            baseRenderer.gameObject.SetActive(true);
        }
    }

    public void MarkSelected()
    {
        isSelected = true;
        baseRenderer.color = selectedColor;
        baseRenderer.gameObject.SetActive(true);
    }

    public void Deselect()
    {
        isSelected = false;
        UpdateBase();
    }
    #endregion

    #region Highlights
    private void ShowHover() => Set(hoverRenderer, hoverColor);
    private void HideHover() => hoverRenderer.gameObject.SetActive(false);

    public void MarkMovement() => Set(overlayRenderer, moveColor);
    public void MarkAttack()   => Set(overlayRenderer, attackColor);
    public void MarkTarget()   => Set(overlayRenderer, targetColor);

    public void ClearOverlay() => overlayRenderer.gameObject.SetActive(false);

    public void ClearAll()
    {
        ClearOverlay();
        HideHover();
        if (!isSelected) baseRenderer.gameObject.SetActive(false);
    }

    private void Set(SpriteRenderer r, Color c)
    {
        r.color = c;
        r.gameObject.SetActive(true);
    }
    #endregion

    #region Pointer
    public void OnPointerEnter(PointerEventData e) => ShowHover();
    public void OnPointerExit(PointerEventData e)  => HideHover();

    public void OnPointerClick(PointerEventData e)
    {
        if (!gridManager.CanSelectCell)
        {
            if (IsTaken && !CurrentUnit.IsAiUnit)
                gridManager.SelectedUnit = CurrentUnit;
        }
        else if (!IsTaken)
        {
            gridManager.OnTileClicked(this);
        }
        else
        {
            gridManager.SelectedUnit = CurrentUnit;
        }
    }
    #endregion
}
