using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapManager
{
    public GameObject Map { get; private set; }
    public GameObject MapName { get; private set; }
    public Grid CellGrid { get; private set; }

    Dictionary<Vector3Int, BaseController> cells = new Dictionary<Vector3Int, BaseController>();

    private int MinX;
    private int MaxX;
    private int MinY;
    private int MaxY;

    public Vector3Int World2Cell(Vector3 worldPos) { return CellGrid.WorldToCell(worldPos); }
    public Vector3 Cell2World(Vector3Int cellPos) { return CellGrid.CellToWorld(cellPos); }

    //ECellCollisionType[,] _collision;
}
