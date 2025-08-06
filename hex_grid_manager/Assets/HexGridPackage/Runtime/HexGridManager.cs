// HexGridManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexOrientation { FlatTop, PointyTop }
public enum ColumnOffset   { OddQ, EvenQ }

public class HexGridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth  = 11;
    public int gridHeight =  9;

    [Header("Shape")]
    public HexOrientation orientation   = HexOrientation.FlatTop;
    public ColumnOffset   columnOffset = ColumnOffset.OddQ;

    [Header("Spacing")]
    public float horizontalSpacing = 1f;
    public float verticalSpacing   = 0.866f;

    [Header("Demo Unit")]
    public DemoUnit SelectedUnit { get; set; }
    public bool     CanSelectCell { get; private set; }
    public bool     IsMoving       { get; private set; }
    public bool     IsAttackMode   { get; private set; }

    [SerializeField] private float moveSpeed = 2f;

    private List<GridTile>                            gridTiles;
    private Dictionary<(int col, int row), GridTile>  tileLookup;

    private void Awake()
    {
        // 1) collect all child tiles
        gridTiles  = new List<GridTile>(GetComponentsInChildren<GridTile>());
        tileLookup = new Dictionary<(int, int), GridTile>(gridTiles.Count);

        // 2) parse their names "Hex_col_row" → fill lookup
        foreach (var t in gridTiles)
        {
            var parts = t.name.Split('_');
            if (parts.Length >= 3
             && int.TryParse(parts[1], out var c)
             && int.TryParse(parts[2], out var r))
            {
                t.ColNum = c;
                t.RowNum = r;
                tileLookup[(c, r)] = t;
            }
            else
            {
                Debug.LogWarning($"Bad tile name: {t.name}");
            }
        }

        // 3) precompute neighbors at range=1
        foreach (var t in gridTiles)
            t.Neighbors = GetMovementRange(t, 1);
    }

    // offset → cube coords
    private (int x,int y,int z) OffsetToCube(int col, int row)
    {
        if (orientation == HexOrientation.FlatTop)
        {
            // Flat-topped: use q-offset (columns control vertical shift)
            if (columnOffset == ColumnOffset.OddQ)
            {
                // odd-q: odd columns shifted down
                int x = col;
                int z = row - ((col - (col & 1)) / 2);
                int y = -x - z;
                return (x, y, z);
            }
            else
            {
                // even-q: even columns shifted down
                int x = col;
                int z = row - ((col + (col & 1)) / 2);
                int y = -x - z;
                return (x, y, z);
            }
        }
        else
        {
            // Pointy-topped: q-offset (rows control horizontal shift)
            if (columnOffset == ColumnOffset.OddQ)
            {
                // odd-q: odd columns right
                int x = col;
                int z = row - ((col - (col & 1)) / 2);
                int y = -x - z;
                return (x, y, z);
            }
            else
            {
                // even-q: even columns right
                int x = col;
                int z = row - ((col + (col & 1)) / 2);
                int y = -x - z;
                return (x, y, z);
            }
        }
    }
    // cube → offset coords
    private (int col,int row) CubeToOffset(int x, int y, int z)
    {
        if (orientation == HexOrientation.FlatTop)
        {
            // Flat-topped: reverse q-offset
            if (columnOffset == ColumnOffset.OddQ)
            {
                // odd-q
                int col = x;
                int row = z + ((x - (x & 1)) / 2);
                return (col, row);
            }
            else
            {
                // even-q
                int col = x;
                int row = z + ((x + (x & 1)) / 2);
                return (col, row);
            }
        }
        else
        {
            // Pointy-topped: reverse q-offset
            if (columnOffset == ColumnOffset.OddQ)
            {
                int col = x;
                int row = z + ((x - (x & 1)) / 2);
                return (col, row);
            }
            else
            {
                int col = x;
                int row = z + ((x + (x & 1)) / 2);
                return (col, row);
            }
        }
    }

    #region Range Methods
    /// <summary>All tiles ≤ range away (excludes center).</summary>
    public List<GridTile> GetMovementRange(GridTile center, int range)
    {
        var (ox, oy, oz) = OffsetToCube(center.ColNum, center.RowNum);
        var result = new List<GridTile>();

        for (int dx = -range; dx <= range; dx++)
        for (int dy = Mathf.Max(-range, -dx - range);
                 dy <= Mathf.Min(range, -dx + range);
                 dy++)
        {
            int dz = -dx - dy;
            var (c, r) = CubeToOffset(ox + dx, oy + dy, oz + dz);

            if ((c != center.ColNum || r != center.RowNum)
             && tileLookup.TryGetValue((c, r), out var tile))
            {
                result.Add(tile);
            }
        }
        return result;
    }

    /// <summary>Same as movement range but never returns center.</summary>
    public List<GridTile> GetAttackRange(GridTile center, int range)
    {
        var list = GetMovementRange(center, range);
        list.RemoveAll(t => t == center);
        return list;
    }
    #endregion

    #region Pathfinding
    /// <summary>Standard A* on the hex graph.</summary>
    public List<GridTile> FindPath(GridTile start, GridTile target)
    {
        var frontier  = new SimplePriorityQueue<GridTile>();
        var cameFrom  = new Dictionary<GridTile, GridTile>();
        var costSoFar = new Dictionary<GridTile, int> { [start] = 0 };

        frontier.Enqueue(start, 0);
        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            if (current == target) break;

            foreach (var nb in current.Neighbors)
            {
                if (nb == null || (nb.IsTaken && nb != target))
                    continue;

                int newCost = costSoFar[current] + nb.MovementCost;
                if (!costSoFar.ContainsKey(nb) || newCost < costSoFar[nb])
                {
                    costSoFar[nb] = newCost;
                    int priority = newCost + Heuristic(nb, target);
                    frontier.Enqueue(nb, priority);
                    cameFrom[nb] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(target))
            return null;

        var path = new List<GridTile>();
        for (var node = target; node != start; node = cameFrom[node])
            path.Add(node);
        path.Reverse();
        return path;
    }

    // cube-distance heuristic
    private int Heuristic(GridTile a, GridTile b)
    {
        var ca = OffsetToCube(a.ColNum, a.RowNum);
        var cb = OffsetToCube(b.ColNum, b.RowNum);
        return (Mathf.Abs(ca.x - cb.x)
              + Mathf.Abs(ca.y - cb.y)
              + Mathf.Abs(ca.z - cb.z)) / 2;
    }
    #endregion

    #region Movement & Attack
    public void StartAttackMode()
    {
        if (SelectedUnit == null) return;
        DehighlightAll();
        IsAttackMode = true;
        foreach (var t in GetAttackRange(SelectedUnit.CurrentTile, SelectedUnit.RemainingMovement))
            t.MarkAttack();
    }

    public void ExitAttackMode()
    {
        IsAttackMode = false;
        gridTiles.ForEach(t => t.ClearOverlay());
    }

    public void HandleUnitMovement()
    {
        Debug.Log("HandleUnitMovement called");
        Debug.Log($"SelectedUnit: {SelectedUnit?.name}, IsMoving: {IsMoving}, CanSelectCell: {CanSelectCell}");
        if (SelectedUnit == null) return;
        ExitAttackMode();
        DehighlightAll();
        foreach (var t in GetMovementRange(SelectedUnit.CurrentTile, 3))
            t.MarkMovement();
        CanSelectCell = true;
    }

    public void OnTileClicked(GridTile tile)
    {
        if (!CanSelectCell || tile.IsTaken) return;
        var path = FindPath(SelectedUnit.CurrentTile, tile);
        if (path == null || path.Count == 0) return;

        IsMoving = true;
        DehighlightAll();
        path.ForEach(t => t.MarkMovement());
        StartCoroutine(MoveAlongPath(path));
    }

    private IEnumerator MoveAlongPath(List<GridTile> path)
    {
        SelectedUnit.PlayWalkAnimation(path[^1]);
        foreach (var next in path)
        {
            SelectedUnit.CurrentTile.UnOccupy();
            yield return MoveUnit(next);
            SelectedUnit.CurrentTile = next;
            next.Occupy(SelectedUnit);
            SelectedUnit.RemainingMovement--;
            next.ClearOverlay();
        }
        IsMoving = false;
        SelectedUnit.PlayIdleAnimation();

        // Once we finish moving, disable cell selection so the user
        // must click Move Mode again to highlight a new path.
        CanSelectCell = false;
    }

    private IEnumerator MoveUnit(GridTile dest)
    {
        var start = SelectedUnit.transform.position;
        var end   = dest.transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            SelectedUnit.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }
    #endregion

    #region Utility
    public GridTile GetTile(int col, int row)
        => tileLookup.TryGetValue((col, row), out var t) ? t : null;

    public void DehighlightAll()
        => gridTiles.ForEach(t => { t.ClearOverlay(); t.Deselect(); });

    public void ClearGrid()
    {
        foreach (var t in gridTiles)
        {
            t.UnOccupy();
            t.ClearAll();
        }
        SelectedUnit   = null;
        CanSelectCell = false;
        IsAttackMode  = false;
    }
    #endregion
}
