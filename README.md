# Hex Grid Package

**A Unity Editor and runtime toolkit for generating and using hexagonal grids.**

Supports flat-top and pointy-top orientations, odd/even offset (q/r), pathfinding, range queries, and interactive tiles.

---

## Features

* **Editor Tooling**

  * **HexGridPainterWindow**: Custom EditorWindow to generate grids from a ScriptableObject config.
  * **HexGridConfig**: ScriptableObject containing grid dimensions, orientation, offset mode, prefab reference, and spacing.

* **Runtime Core**

  * **HexGridManager**: MonoBehaviour that reads child `GridTile` objects, parses coordinates, builds lookup, and provides:

    * Movement and attack range queries (`GetMovementRange`, `GetAttackRange`)
    * A\* pathfinding (`FindPath`)
  * **GridTile**: Interactive tile component handling hover, click, occupancy, and overlay highlights.
  * **SimplePriorityQueue**: Lightweight priority queue used by pathfinding.

* **Sample/Demo**

  * **DemoUnit** & **GridDemoController**: Example scripts showing how to spawn a unit, occupy tiles, and trigger movement/attack modes.
  * **Demo Scene & Prefabs**: Prebuilt scene demonstrating grid generation, unit placement, and pathfinding.

---

## Installation

1. **Add to Project**: Copy the `Assets/HexGridPackage` folder into your Unity project.
2. **(Optional) Assembly Definitions**: Create `.asmdef` files in `Runtime` and `Editor` folders for faster compile times.
3. **Open the Painter**: In Unity Editor, go to **Window > Hex Grid Painter** to launch the grid generator.

---

## Quick Start

1. **Create a Grid Config**

   * Right-click in Project window: **Create > Hex Grid > Config**.
   * Configure:

     * **Columns** & **Rows**
     * **Orientation**: `FlatTop` or `PointyTop`
     * **ColumnOffset**: `OddQ` or `EvenQ`
     * **Tile Prefab**: Reference a prefab containing the `GridTile` component.
     * **Horizontal/Vertical Spacing**

2. **Generate the Grid**

   * Open **Window > Hex Grid Painter**.
   * Assign your `HexGridConfig` asset.
   * Specify a **Parent Name** for the grid GameObject.
   * Click **Generate Grid**.
   * The grid appears in your scene; the parent object has a `HexGridManager` attached.

3. **Run the Demo**

   * Open `Samples~/DemoScene.unity`.
   * Press Play and use UI buttons to move the demo unit around.

---

## Folder Structure

```
Assets/HexGridPackage/
├─ Runtime/               // Core runtime scripts
│   ├─ HexGridManager.cs
│   ├─ GridTile.cs
│   ├─ SimplePriorityQueue.cs
│   └─ DemoUnit.cs
├─ Editor/                // Editor-only scripts
│   ├─ HexGridConfig.cs
│   └─ HexGridPainterWindow.cs
├─ Samples~/              // Demo scene and prefabs (optional)
│   ├─ DemoScene.unity
│   ├─ Prefabs/
│   └─ GridDemoController.cs
└─ Documentation/         // This README and additional docs
```

---

## API Reference

### HexGridConfig (ScriptableObject)

* `int columns` & `int rows`
* `HexOrientation orientation` (`FlatTop` or `PointyTop`)
* `ColumnOffset columnOffset` (`OddQ` or `EvenQ`)
* `GameObject tilePrefab`
* `float horizontalSpacing`, `float verticalSpacing`

### HexGridPainterWindow (EditorWindow)

* `GenerateGrid()` & Inspector fields to drive grid creation.

### HexGridManager (MonoBehaviour)

* `List<GridTile> GetMovementRange(GridTile center, int range)`
* `List<GridTile> GetAttackRange(GridTile center, int range)`
* `List<GridTile> FindPath(GridTile start, GridTile target)`
* `void HandleUnitMovement()`, `void StartAttackMode()`, etc.

### GridTile (MonoBehaviour)

* `void Occupy(DemoUnit unit)`, `void UnOccupy()`
* Highlight methods: `MarkMovement()`, `MarkAttack()`, `ClearOverlay()`
* Pointer events: `OnPointerClick`, `OnPointerEnter/Exit`

---

## License

This package is released under the **MIT License**. See `LICENSE.txt` for details.

---

## Support & Contributions

Feel free to open issues or pull requests on GitHub. For questions, reach out via the repository issues page.
