using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public abstract class CreatureController : BaseController
{
    public ECreatureType CreatureType { get; protected set; } = ECreatureType.None;
    protected ECreatureState _creatureState = ECreatureState.None;
    public virtual ECreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            if (_creatureState != value) 
            {
                _creatureState = value;
            }
        }
    }

    [SerializeField]
    protected SpriteRenderer CreatureSprite;
    protected string SpriteName;
    
    public SkillComponent Skills { get; protected set; }
    protected SkillBase _castingSkill;
    public SkillBase CastingSkill
    {
        get { return _castingSkill; }
        set
        {
            _castingSkill = value;
        }
    }

    public List<Vector3> MovementRange = new List<Vector3>();

    public Vector3 TargetPos;

    public CreatureData CreatureData { get; protected set; }
    public ClassData ClassData { get; protected set; }

    #region Stats
    public float Hp { get; set; }
    public float Mp { get; set; }
    public float Atk { get; set; }
    public float Def { get; set; }
    public int Mov { get; set; }
    #endregion

    public bool IsMove { get; set; } = false;
    public bool IsSkill { get; set; } = false;

    void Awake()
    {
        Init();    
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Creature;
        CreatureState = ECreatureState.Idle;

        CreatureSprite = gameObject.GetOrAddComponent<SpriteRenderer>();

        Collider.isTrigger = true;
        
        RigidBody.mass = 0;
        RigidBody.simulated = true;
        RigidBody.gravityScale = 0;
        RigidBody.freezeRotation = true;

        return true;
    }

    public virtual void SetInfo(int templateID)
    {
        DataTemplateID = templateID;

        if (CreatureType == ECreatureType.PlayerUnit)
            CreatureData = Managers.Data.PlayerUnitDataDic[templateID];
        else if (CreatureType == ECreatureType.Monster)
            CreatureData = Managers.Data.MonsterDataDic[templateID];

        ClassData = Managers.Data.ClassDataDic[CreatureData.ClassDataID];
        gameObject.name = $"{CreatureData.DataID}_{CreatureData.Name}";

        SortingGroup sg = gameObject.GetOrAddComponent<SortingGroup>();
        sg.sortingOrder = SortingLayers.CREATURE;

        // ½ºÅ³
        Skills = gameObject.GetOrAddComponent<SkillComponent>();
        Skills.SetInfo(this);

        // Stat
        Hp = CreatureData.Hp;
        Mp = CreatureData.Mp;
        Atk = CreatureData.Atk;
        Def = CreatureData.Def;
        Mov = ClassData.Mov;

        // State
        CreatureState = ECreatureState.Idle;

        // Map
        StartCoroutine(CoLerpToCellPos());
    }

    protected IEnumerator CoUpdate()
    {
        while (true)
        {
            switch (CreatureState) 
            {
                case ECreatureState.Idle:
                    UpdateIdle();
                    break;
                case ECreatureState.Move:
                    UpdateMove();
                    break;
                case ECreatureState.Skill:
                    UpdateSkill();
                    break;
                case ECreatureState.Dead:
                    UpdateDead();
                    break;
                case ECreatureState.EndTurn:
                    UpdateEndTurn();
                    break;
            }

            yield return null;
        }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMove() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateDead() { }
    protected virtual void UpdateEndTurn() { }

    public void SetMovementRange()
    {
        ClearMovementRange();
        for (int dx = -Mov; dx <= Mov; dx++)
        {
            for (int dy = -Mov; dy <= Mov; dy++)
            {
                int distance = Math.Abs(dx) + Math.Abs(dy);
                if (distance <= Mov && distance > 0)
                {
                    Vector3 destPos = Managers.Map.GetTilePosition(transform.position, new Vector3Int(dx, dy));
                    if (FindPath(destPos, Mov) == EFindPathResult.Success)
                    {
                        MovementRange.Add(destPos);
                        if (CreatureType == ECreatureType.PlayerUnit)
                        {
                            GameObject go = Managers.Resource.Instantiate("RangeTile", pooling: true);
                            go.transform.position = destPos;
                        }
                    }
                }
            }
        }
    }

    public void ClearMovementRange()
    {
        MovementRange.Clear();
        GameObject go = GameObject.Find("RangeTile");
        if (go == null)
            return;

        foreach (Transform child in go.transform.parent)
        {
            if (child != null && child.gameObject.activeInHierarchy)
                Managers.Resource.Destroy(child.gameObject);
        }
    }

    #region Map
    public EFindPathResult FindPath(Vector3 destWorldPos, int maxDepth)
    {
        Vector3Int destCellPos = Managers.Map.WorldToCell(destWorldPos);
        return FindPath(destCellPos, maxDepth);
    }

    public EFindPathResult FindPath(Vector3Int destCellPos, int maxDepth)
    {
        // A*
        List<Vector3Int> path = Managers.Map.FindPath(this, CellPos, destCellPos, maxDepth);
        if (path.Count <= 0)
            return EFindPathResult.Fail_NoPath;

        int idx = 1;
        while (path.Count > idx)
        {
            Vector3Int dirCellPos = path[idx] - CellPos;
            Vector3Int nextPos = CellPos + dirCellPos;
            idx++;

            if (Managers.Map.CanGo(nextPos) == false)
                return EFindPathResult.Fail_MoveTo;
        }

        return EFindPathResult.Success;
    }

    public EFindPathResult FindPathAndMoveToCellPos(Vector3 destWorldPos, int maxDepth, bool forceMoveCloser = false)
    {
        Vector3Int destCellPos = Managers.Map.WorldToCell(destWorldPos);
        return FindPathAndMoveToCellPos(destCellPos, maxDepth, forceMoveCloser);
    }

    public EFindPathResult FindPathAndMoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
    {
        if (LerpCellPosCompleted == false)
            return EFindPathResult.Fail_LerpCell;

        // A*
        List<Vector3Int> path = Managers.Map.FindPath(this, CellPos, destCellPos, maxDepth);
        if (path.Count < 2)
            return EFindPathResult.Fail_NoPath;

        EFindPathResult findPathResult = EFindPathResult.None;
        StartCoroutine(CoMoveAlongPath(path, forceMoveCloser, callback => findPathResult = callback));
        return findPathResult;

        //int idx = 1;
        //while (path.Count > idx)
        //{
        //    if (forceMoveCloser)
        //    {
        //        Vector3Int diff1 = CellPos - destCellPos;
        //        Vector3Int diff2 = path[idx] - destCellPos;
        //        if (diff1.sqrMagnitude <= diff2.sqrMagnitude)
        //            return EFindPathResult.Fail_NoPath;
        //    }

        //    Vector3Int dirCellPos = path[idx] - CellPos;
        //    Vector3Int nextPos = CellPos + dirCellPos;
        //    idx++;

        //    if (Managers.Map.CanGo(nextPos) == false)
        //        return EFindPathResult.Fail_MoveTo;
        //}

        //return EFindPathResult.Success;
    }

    IEnumerator CoMoveAlongPath(List<Vector3Int> path, bool forceMoveCloser, Action<EFindPathResult> callback)
    {
        for (int idx = 1; idx < path.Count; idx++)
        {
            if (forceMoveCloser)
            {
                Vector3Int diff1 = CellPos - path[path.Count - 1];
                Vector3Int diff2 = path[idx] - path[path.Count - 1];
                if (diff1.sqrMagnitude <= diff2.sqrMagnitude)
                {
                    callback(EFindPathResult.Fail_NoPath);
                    yield break;
                }
            }

            Vector3Int dirCellPos = path[idx] - CellPos;
            Vector3Int nextPos = CellPos + dirCellPos;
            //Vector3Int nextPos = path[idx];

            if (Managers.Map.MoveTo(this, nextPos) == false)
            {
                callback(EFindPathResult.Fail_MoveTo);
                yield break;
            }
            else
                yield return new WaitUntil(() => LerpCellPosCompleted);
        }

        callback(EFindPathResult.Success);
    }

    protected IEnumerator CoLerpToCellPos()
    {
        while (true)
        {
            LerpToCellPos(5f);
            yield return null;
        }
    }
    #endregion
}
