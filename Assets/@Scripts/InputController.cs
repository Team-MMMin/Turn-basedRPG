using UnityEngine;

public class InputController : MonoBehaviour
{
    public Entity selectedEntity;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = MapManager.Instance.tilemap.WorldToCell(mouseWorldPos);
            OverlayTile overlayTile = MapManager.Instance.GetOverlayTileFromGridPosition((Vector2Int)gridPosition);

            if (overlayTile != null && !overlayTile.isBlocked)
            {
                selectedEntity.MoveToTile(overlayTile);
            }
        }
    }
}
