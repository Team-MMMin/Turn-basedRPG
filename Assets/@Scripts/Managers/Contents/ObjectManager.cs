using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public HashSet<PlayerUnitController> PlayerUnits { get; } = new HashSet<PlayerUnitController>();
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();

    #region Roots
    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject() { name = name };

        return root.transform;
    }

    public Transform PlayerUnitRoot { get { return GetRootTransform("@PlayerUnits"); } }
    public Transform MonsterRoot { get { return GetRootTransform("@Monsters"); } }
    #endregion

    public T Spawn<T>(Vector3Int cellPos, int templateID) where T : BaseController
    {
        Vector3 spawnPos = Managers.Map.CellToWorld(cellPos);
        return Spawn<T>(spawnPos, templateID);
    }

    public T Spawn<T>(Vector3 pos, int templateID) where T : BaseController
    {
        // TODO
        // 해당 위치가 움직일 수 있는 곳인지 확인
        if (Managers.Map.GetObject(pos) != null)
            return null;

        string prefabName = typeof(T).Name;
        if (prefabName.Contains("Controller"))
            prefabName = $"{prefabName.Replace("Controller", "")}";

        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = pos;

        BaseController obj = go.GetComponent<BaseController>();
        EObjectType objectType = obj.ObjectType;

        if (obj.ObjectType == EObjectType.Creature)
        {
            CreatureController creature = obj.GetComponent<CreatureController>();
            switch (creature.CreatureType)
            {
                case ECreatureType.PlayerUnit:
                    obj.transform.parent = PlayerUnitRoot;
                    PlayerUnitController playerUnit = go.GetComponent<PlayerUnitController>();
                    playerUnit.SetInfo(templateID);
                    PlayerUnits.Add(playerUnit);
                    break;
                case ECreatureType.Monster:
                    obj.transform.parent = MonsterRoot;
                    MonsterController monster = go.GetComponent<MonsterController>();
                    monster.SetInfo(templateID);
                    Monsters.Add(monster);
                    break;
            }
        }

        Managers.Map.AddObject(obj, Managers.Map.WorldToCell(pos));
        return obj as T;
    }

    public void Despawn<T>(T obj) where T : BaseController
    {
        if (obj.ObjectType == EObjectType.Creature)
        {
            CreatureController creature = obj.GetComponent<CreatureController>();
            switch (creature.CreatureType)
            {
                case ECreatureType.PlayerUnit:
                    PlayerUnitController playerUnit = obj as PlayerUnitController;
                    PlayerUnits.Remove(playerUnit);
                    break;
                case ECreatureType.Monster:
                    MonsterController monster = obj as MonsterController;
                    Monsters.Remove(monster);
                    break;
            }
        }

        Managers.Resource.Destroy(obj.gameObject);
    }
}
