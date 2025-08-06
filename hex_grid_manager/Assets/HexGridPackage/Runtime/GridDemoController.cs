using UnityEngine;

public class GridDemoController : MonoBehaviour
{
    [Header("Assign your HexGridManager here")]
    public HexGridManager grid;

    [Header("Assign the DemoUnit prefab here")]
    public DemoUnit demoUnitPrefab;

    private DemoUnit demo;

    void Start()
    {
        // Instantiate a single unit at (0,0)
        demo = Instantiate(demoUnitPrefab, Vector3.zero, Quaternion.identity);

        // *** TELL THE GRID MANAGER ABOUT THIS UNIT ***
        grid.SelectedUnit = demo;

        // Occupy the start tile
        var startTile = grid.GetTile(0, 0);
        if (startTile != null)
        {
            startTile.Occupy(demo);
            demo.CurrentTile = startTile;
        }
    }

    // UI Button callbacks:
    public void OnMoveMode()   => grid.HandleUnitMovement();
    public void OnAttackMode() => grid.StartAttackMode();
    public void OnReset()      => OnRestartScene();

    public void OnRestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
