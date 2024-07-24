using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class MapManager
{
    public GameObject Map { get; private set; }
    public string MapName { get; private set; }
    public Grid CellGrid { get; private set; }

    Dictionary<Vector3Int, BaseController> _cells = new Dictionary<Vector3Int, BaseController>();

    int MinX;
    int MaxX;
    int MinY;
    int MaxY;

    public Vector3Int WorldToCell(Vector3 worldPos) { return CellGrid.WorldToCell(worldPos); }  // ���� ��ǥ�� Ÿ�ϸ��� �� ��ǥ�� ��ȯ
    public Vector3 CellToWorld(Vector3Int cellPos) { return CellGrid.CellToWorld(cellPos); }    // �� ��ǥ�� Ÿ�ϸ��� ���� ��ǥ�� ��ȯ  

    ECellCollisionType[,] _collision;

    public void LoadMap(string mapName)
    {
        DestroyMap();

        GameObject map = Managers.Resource.Instantiate(mapName);
        map.transform.position = Vector3.zero;
        map.name = $"@Map_{mapName}";

        Map = map;
        MapName = mapName;
        CellGrid = map.GetComponent<Grid>();

        ParseCollisionData(map, mapName);
    }

    public void DestroyMap()
    {
        ClearObjects();

        if (Map != null)
            Managers.Resource.Destroy(Map);
    }

    void ParseCollisionData(GameObject map, string mapName, string tilemap = "Tilemap_Collision")
    {
        GameObject collision = Util.FindChild(map, tilemap, true);
        if (collision != null)
            collision.SetActive(false);

        TextAsset txt = Managers.Resource.Load<TextAsset>($"{mapName}Collision");
        StringReader reader = new StringReader(txt.text);

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());

        int xCount = MaxX - MinX + 1;
        int yCount = MaxY - MinY + 1;
        _collision = new ECellCollisionType[xCount, yCount];

        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < xCount; x++)
            {
                switch (line[x])
                {
                    case Define.MAP_TOOL_WALL:
                        _collision[x, y] = ECellCollisionType.Wall;
                        break;
                    case Define.MAP_TOOL_NONE:
                        _collision[x, y] = ECellCollisionType.None;
                        break;
                    case Define.MAP_TOOL_SEMI_WALL:
                        _collision[x, y] = ECellCollisionType.SemiWall;
                        break;
                }
            }
        }
    }

    public bool MoveTo(CreatureController obj,Vector3Int cellPos, bool forceMove = false)
    {
        if (CanGo(cellPos) == false) 
            return false;

        RemoveObject(obj);

        AddObject(obj,cellPos);

        obj.SetCellPos(cellPos,forceMove);
        return true;
    }

    #region Helpers
    public BaseController GetObject(Vector3Int cellPos)
    {
        _cells.TryGetValue(cellPos, out BaseController value);
        return value;
    }

    public BaseController GetObject(Vector3 worldPos)
    {
        Vector3Int cellPos = WorldToCell(worldPos);
        return GetObject(cellPos);
    }
    
    public bool RemoveObject(BaseController obj)
    {
        BaseController prev = GetObject(obj.CellPos);

        if (prev != obj)
            return false;

        _cells[obj.CellPos] = null;
        return true;
    }

    public bool AddObject(BaseController obj, Vector3Int cellPos)   
    {
        if (CanGo(cellPos) == false)
        {
            Debug.LogWarning($"AddObject Failed");
            return false;
        }
        
        BaseController prev = GetObject(cellPos);
        if (prev != null)
        {
            Debug.LogWarning($"AddObject Failed");
            return false;
        }

        _cells[cellPos] = obj;
        return true;
    }

    public bool CanGo(Vector3 worldPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
    {
        return CanGo(WorldToCell(worldPos), ignoreObjects, ignoreSemiWall);
    }

    public bool CanGo(Vector3Int cellPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
    {
        if (cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        if (ignoreObjects == false)
        {
            BaseController obj = GetObject(cellPos);
            if (obj != null)
                return false;
        }

        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y;
        ECellCollisionType type = _collision[x, y];
        if (type == ECellCollisionType.None)
            return true;

        if (ignoreSemiWall && type == ECellCollisionType.SemiWall)
            return true; 

        return false;
    }

    public void ClearObjects()
    {
        _cells.Clear();
    }
    #endregion

    #region A* PathFinding
    public struct PQNode : IComparable<PQNode>
    {
        public int H;
        public Vector3Int CellPos;
        public int Depth;

        public int CompareTo(PQNode other)
        {
            if (H == other.H)
                return 0;
            return H < other.H ? 1 : -1;
        }
    }

    List<Vector3Int> _delta = new List<Vector3Int>()
    {
        new Vector3Int(0, 1, 0), // U
		//new Vector3Int(1, 1, 0), // UR
		new Vector3Int(1, 0, 0), // R
		//new Vector3Int(1, -1, 0), // DR
		new Vector3Int(0, -1, 0), // D
		//new Vector3Int(-1, -1, 0), // LD
		new Vector3Int(-1, 0, 0), // L
		//new Vector3Int(-1, 1, 0), // LU
    };

    public List<Vector3Int> FindPath(BaseController self, Vector3Int startCellPos, Vector3Int destCellPos, int maxDepth = 10)
    {
        // 지금까지 제일 좋은 후보 기록
        Dictionary<Vector3Int, int> best = new Dictionary<Vector3Int, int>();
        // 경로 추적 용도
        Dictionary<Vector3Int, Vector3Int> parent = new Dictionary<Vector3Int, Vector3Int>();

        // 현재 발견된 후보 중에서 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

        Vector3Int pos = startCellPos;
        Vector3Int dest = destCellPos;

        // destCellPos에 도착 못하더라도 제일 가까운 애
        //Vector3Int closestCellPos = startCellPos;
        //int closestH = (dest - pos).sqrMagnitude;

        // 시작점 발견 (예약 진행)
        {
            int h = (dest - pos).sqrMagnitude;
            pq.Push(new PQNode() { H = h, CellPos = pos, Depth = 1 });
            parent[pos] = pos;
            best[pos] = h;
        }

        while (pq.Count > 0)
        {
            // 제일 좋은 후보를 찾는다
            PQNode node = pq.Pop();
            pos = node.CellPos; 
            
            if (pos == dest)
                break;

            if (node.Depth >= maxDepth)
                break;

            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약
            foreach (Vector3Int delta in _delta)
            {
                Vector3Int next = pos + delta;

                if (CanGo(next) == false)
                    continue;

                int h = (dest - next).sqrMagnitude;

                // 더 좋은 후보 찾았는지
                if (best.ContainsKey(next) == false)
                    best[next] = int.MaxValue;

                if (best[next] <= h)
                    continue;

                best[next] = h;

                pq.Push(new PQNode() { H = h, CellPos = next, Depth = node.Depth + 1 });
                parent[next] = pos;

                // 목적지까지는 못 가더라도, 그나마 제일 좋았던 후보 기억.
                //if (closestH > h)
                //{
                //    closestH = h;
                //    closestCellPos = next;
                //}
            }
        }

        // 제일 가까운 애라도 찾는다
        //if (parent.ContainsKey(dest) == false)
        //    return CalcCellPathFromParent(parent, closestCellPos);

        return CalcCellPathFromParent(parent, dest);
    }

    List<Vector3Int> CalcCellPathFromParent(Dictionary<Vector3Int, Vector3Int> parent, Vector3Int dest)
    {
        List<Vector3Int> cells = new List<Vector3Int>();

        if (parent.ContainsKey(dest) == false)
            return cells;

        Vector3Int now = dest;

        while (parent[now] != now)
        {
            cells.Add(now);
            now = parent[now];
        }

        cells.Add(now);
        cells.Reverse();

        return cells;
    }
    #endregion
}
