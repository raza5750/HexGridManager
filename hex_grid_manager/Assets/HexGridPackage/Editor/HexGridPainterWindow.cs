// HexGridPainterWindow.cs
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HexGridPackage.Editor; // for HexGridConfig, Editor‐side enums

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
        // Destroy existing parent if it exists
        var existing = GameObject.Find(parentName);
        if (existing != null)
            Undo.DestroyObjectImmediate(existing);

        // Create new parent
        var parentGO = new GameObject(parentName);
        Undo.RegisterCreatedObjectUndo(parentGO, "Create HexGrid Parent");

        // Add & configure runtime manager
        var mgr = Undo.AddComponent<HexGridManager>(parentGO);
        mgr.gridWidth         = config.columns;
        mgr.gridHeight        = config.rows;
        // <— explicit casts from editor‐side enum to runtime enum:
        mgr.orientation       = (HexOrientation)config.orientation;
        mgr.columnOffset      = (ColumnOffset)config.columnOffset;
        mgr.horizontalSpacing = config.horizontalSpacing;
        mgr.verticalSpacing   = config.verticalSpacing;

        // // Instantiate tiles
        // for (int r = 0; r < config.rows; r++)
        // {
        //     for (int c = 0; c < config.columns; c++)
        //     {
        //         var tileGO = (GameObject)PrefabUtility.InstantiatePrefab(
        //             config.tilePrefab.gameObject, parentGO.transform);
        //         Undo.RegisterCreatedObjectUndo(tileGO, "Instantiate HexTile");

        //         float x, y;
        //         if ((HexOrientation)config.orientation == HexOrientation.FlatTop)
        //         {
        //             // odd-r flat-top
        //             x = c * config.horizontalSpacing
        //               + ((r & 1) == 1 ? config.horizontalSpacing * 0.5f : 0f);
        //             y = -r * config.verticalSpacing;
        //         }
        //         else
        //         {
        //             // odd-q pointy-top
        //             x = c * config.horizontalSpacing;
        //             y = -(r * config.verticalSpacing
        //                + ((c & 1) == 1 ? config.verticalSpacing * 0.5f : 0f));
        //         }

        //         tileGO.transform.localPosition = new Vector3(x, y, 0f);
        //         tileGO.name = $"Hex_{c}_{r}";
        //     }
        // }

        for (int r = 0; r < config.rows; r++)
{
    for (int c = 0; c < config.columns; c++)
    {
        var tileGO = (GameObject)PrefabUtility.InstantiatePrefab(
            config.tilePrefab.gameObject, parentGO.transform);
        Undo.RegisterCreatedObjectUndo(tileGO, "Instantiate HexTile");

        float x = 0f, y = 0f;

        if ((HexOrientation)config.orientation == HexOrientation.FlatTop)
        {
            // FLAT-TOP: r = rows, c = columns
            x = c * config.horizontalSpacing;

            // Odd-r (odd rows shifted), Even-r (even rows shifted)
            bool isOffset = ((ColumnOffset)config.columnOffset == ColumnOffset.OddQ && c % 2 != 0) ||
                            ((ColumnOffset)config.columnOffset == ColumnOffset.EvenQ && c % 2 == 0);
            y = -r * config.verticalSpacing + (isOffset ? -config.verticalSpacing * 0.5f : 0f);
        }
        else
        {
            // POINTY-TOP: r = rows, c = columns
            y = -r * config.verticalSpacing;

            // Odd-q (odd columns shifted), Even-q (even columns shifted)
            bool isOffset = ((ColumnOffset)config.columnOffset == ColumnOffset.OddQ && r % 2 != 0) ||
                            ((ColumnOffset)config.columnOffset == ColumnOffset.EvenQ && r % 2 == 0);
            x = c * config.horizontalSpacing + (isOffset ? config.horizontalSpacing * 0.5f : 0f);
        }

        tileGO.transform.localPosition = new Vector3(x, y, 0f);
        tileGO.name = $"Hex_{c}_{r}";
    }
}



        // Select the new parent and mark the scene dirty
        Selection.activeGameObject = parentGO;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
