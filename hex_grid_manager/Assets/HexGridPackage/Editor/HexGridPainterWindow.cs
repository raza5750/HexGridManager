// HexGridPainterWindow.cs
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HexGridPackage.Editor;  // for HexGridConfig (Editor‐side)

public class HexGridPainterWindow : EditorWindow
{
    private HexGridConfig config;
    private string parentName = "HexGrid";

    [MenuItem("Window/Hex Grid Painter")]
    public static void ShowWindow() =>
        GetWindow<HexGridPainterWindow>("Hex Grid Painter");

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Hex Grid Generator", EditorStyles.boldLabel);

        config     = (HexGridConfig)EditorGUILayout.ObjectField("Config", config, typeof(HexGridConfig), false);
        parentName = EditorGUILayout.TextField("Parent Name", parentName);

        if (config == null)
        {
            EditorGUILayout.HelpBox("Assign a HexGridConfig to generate.", MessageType.Info);
            return;
        }

        if (GUILayout.Button("Generate Grid"))
            GenerateGrid();
    }

    private void GenerateGrid()
    {
        // 1) remove old grid
        var old = GameObject.Find(parentName);
        if (old != null) Undo.DestroyObjectImmediate(old);

        // 2) create parent object
        var parentGO = new GameObject(parentName);
        Undo.RegisterCreatedObjectUndo(parentGO, "Create HexGrid Parent");

        // 3) configure runtime manager
        var mgr = Undo.AddComponent<HexGridManager>(parentGO);
        mgr.gridWidth         = config.columns;
        mgr.gridHeight        = config.rows;
        // cast Editor enums → Runtime enums:
        HexOrientation orient = (HexOrientation)config.orientation;
        ColumnOffset  offset = (ColumnOffset) config.columnOffset;
        mgr.orientation       = orient;
        mgr.columnOffset      = offset;
        mgr.horizontalSpacing = config.horizontalSpacing;
        mgr.verticalSpacing   = config.verticalSpacing;

        // 4) spawn tiles with the proper offset logic
        for (int r = 0; r < config.rows; r++)
        {
            for (int c = 0; c < config.columns; c++)
            {
                // instantiate prefab
                var tileGO = (GameObject) PrefabUtility.InstantiatePrefab(
                    config.tilePrefab, parentGO.transform);
                Undo.RegisterCreatedObjectUndo(tileGO, "Instantiate HexTile");

                float x = 0f, y = 0f;

                if (orient == HexOrientation.FlatTop)
                {
                    // FLAT-TOP: columns control vertical offset
                    x = c * config.horizontalSpacing;
                    bool isOffset = (offset == ColumnOffset.OddQ  && (c % 2) != 0)
                                 || (offset == ColumnOffset.EvenQ && (c % 2) == 0);
                    y = -r * config.verticalSpacing + (isOffset ? -config.verticalSpacing * 0.5f : 0f);
                }
                else
                {
                    // POINTY-TOP: rows control horizontal offset
                    y = -r * config.verticalSpacing;
                    bool isOffset = (offset == ColumnOffset.OddQ  && (r % 2) != 0)
                                 || (offset == ColumnOffset.EvenQ && (r % 2) == 0);
                    x = c * config.horizontalSpacing + (isOffset ? config.horizontalSpacing * 0.5f : 0f);
                }

                tileGO.transform.localPosition = new Vector3(x, y, 0f);
                tileGO.name = $"Hex_{c}_{r}";
            }
        }

        // 5) finalize
        Selection.activeGameObject = parentGO;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
