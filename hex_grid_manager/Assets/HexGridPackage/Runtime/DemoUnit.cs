using UnityEngine;

public class DemoUnit : MonoBehaviour
{
    public GridTile CurrentTile { get; set; }
    public bool IsAiUnit => false;
    public int RemainingMovement = 3;

    public void PlayWalkAnimation(GridTile _tile) { /* no-op */ }
    public void PlayIdleAnimation() { /* no-op */ }
    public void UpdateSortingOrder() { /* no-op */ }
}
