// HexGridConfig.cs
using UnityEngine;

namespace HexGridPackage.Editor
{
    public enum HexOrientation { FlatTop, PointyTop }
    public enum ColumnOffset   { OddQ, EvenQ }

    [CreateAssetMenu(
        fileName = "HexGridConfig",
        menuName = "Hex Grid/Config",
        order = 1)]
    public class HexGridConfig : ScriptableObject
    {
        [Header("Grid Shape")]
        public HexOrientation orientation = HexOrientation.PointyTop;
        [Tooltip("For pointy-top only: choose whether odd or even columns are shifted")]
        public ColumnOffset columnOffset = ColumnOffset.OddQ;

        [Header("Grid Dimensions")]
        [Tooltip("Number of columns (width)")]
        public int columns = 11;
        [Tooltip("Number of rows (height)")]
        public int rows    =  9;

        [Header("Tile & Spacing")]
        [Tooltip("Prefab containing your GridTile component")]
        public GameObject tilePrefab;
        [Tooltip("Horizontal spacing between tile centers")]
        public float horizontalSpacing = 1f;
        [Tooltip("Vertical spacing between tile centers")]
        public float verticalSpacing   = 0.866f;
    }
}
