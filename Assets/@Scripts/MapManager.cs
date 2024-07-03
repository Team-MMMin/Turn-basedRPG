using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    public Tilemap tilemap;
    public OverlayTile overlayTilePrefab;
    public Dictionary<Vector2Int, OverlayTile> map;

    public TileData normalTileData;
    public TileData blockedTileData;
    public TileData healingTileData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InitializeMap();
    }

    public void InitializeMap()
    {
        map = new Dictionary<Vector2Int, OverlayTile>();
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            var localPlace = new Vector3Int(position.x, position.y, position.z);

            if (!tilemap.HasTile(localPlace)) continue;

            var gridLocation = (Vector2Int)localPlace;
            var overlayTile = Instantiate(overlayTilePrefab, tilemap.CellToWorld(localPlace), Quaternion.identity, transform);
            overlayTile.Init(normalTileData, gridLocation); // Assign normalTileData or any specific data

            map.Add(gridLocation, overlayTile);
        }
    }

    public OverlayTile GetOverlayTileFromGridPosition(Vector2Int position)
    {
        return map.ContainsKey(position) ? map[position] : null;
    }
}
