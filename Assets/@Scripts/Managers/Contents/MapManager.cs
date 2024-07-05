using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapManager
{
    public GameObject Map { get; private set; }
    public GameObject MapName { get; private set; }
    public Grid CellGrid { get; private set; }

//      아래 부분 BaseObject가 무엇이길래 누락되어있는지 확인중
//    Dictionary<Vector3Int, BaseObject> cells = new Dictionary<Vector3Int, BaseObject>(); 4

    private int MinX;
    private int MaxX;
    private int MinY;
    private int MaxY;

    public Vector3Int World2Cell(Vector3 worldPos) { return CellGrid.WorldToCell(worldPos); }
    public Vector3 Cell2World(Vector3Int cellPos) { return CellGrid.CellToWorld(cellPos); }


}
