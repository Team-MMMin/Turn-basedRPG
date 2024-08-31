using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    public bool IsMyTurn = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = ECreatureType.Monster;
        CreatureState = ECreatureState.Idle;

        Managers.Game.OnGameStateChanged -= HandleOnGameStateChanged;
        Managers.Game.OnGameStateChanged += HandleOnGameStateChanged;

        StartCoroutine(CoUpdate());
        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);
    }

    void HandleOnGameStateChanged(EGameState gameState)
    {
        if (gameState != EGameState.MonsterTurn)
            return;

        IsMove = false;
        IsSkill = false;
        CreatureState = ECreatureState.Idle;
    }

    protected override void UpdateIdle()
    {
        if (CreatureState == ECreatureState.Idle)
        {
            if (IsMyTurn)
                ExecuteAI();
        }
    }

    protected override void UpdateMove()
    {
        if (CreatureState == ECreatureState.Move)
        {
            Debug.Log("UpdateMove");
            FindPathAndMoveToCellPos(DestPos, Mov, findClosestPos: true);
            CreatureState = ECreatureState.Idle;
        }
    }

    protected override void UpdateSkill()
    {
        if (CreatureState == ECreatureState.Skill && CastingSkill != null)
        {
            Debug.Log("UpdateSkill");
            CastingSkill.DoSkill();
            CreatureState = ECreatureState.Idle;
        }
    }

    protected override void UpdateDead()
    {
        if (CreatureState == ECreatureState.Dead)
        {
            Debug.Log("UpdateDead");
            // TODO
            // 보상 제공
            Managers.Object.Despawn(this);
        }
    }

    #region AI
    EMonsterBehaviorPattern _behaviorPattern;

    public struct PQUnit : IComparable<PQUnit>
    {
        public float Hp;
        public float Def;
        public Vector3Int CellPos;
        public int Depth;

        public int CompareTo(PQUnit other)
        {
            float depthWeight = 1.0f;   // 거리의 가중치 (가까울수록 우선순위 높음)
            float hpWeight = 0.5f;         // HP의 가중치 (낮을수록 우선순위 높음)
            float defWeight = 0.3f;        // 방어력의 가중치 (낮을수록 우선순위 높음)

            float score = (Depth * depthWeight) + (Hp * hpWeight) + (Def * defWeight);
            float otherScore = (other.Depth * depthWeight) + (other.Hp * hpWeight) + (other.Def * defWeight);

            if (score == otherScore)
                return 0;
            return score < otherScore ? 1 : -1;
        }
    }

    void ExecuteAI()
    {
        Debug.Log("ExecuteAI");
        
        // 타겟 우선 순위 설정
        PriorityQueue<PQUnit> units = new PriorityQueue<PQUnit>();
        foreach (PlayerUnitController unit in Managers.Object.PlayerUnits)
        {
            int depth = Managers.Map.FindPath(this, CellPos, unit.CellPos, Mov).Count;
            units.Push(new PQUnit() { Depth = depth, Hp = unit.Hp, Def = unit.Def, CellPos = unit.CellPos });
        }

        // 타겟이 없는지 확인
        if (units.Count <= 0)
        {
            IsMyTurn = false;
            return;
        }

        _behaviorPattern = EMonsterBehaviorPattern.Aggressive;
        if (Hp <= MaxHp * 0.3)  // 현재 체력이 30%이하라면
            _behaviorPattern = EMonsterBehaviorPattern.Defensive;
        else if (CreatureData.ClassDataID == MAGE_ID)   // 마법사라면
            _behaviorPattern = EMonsterBehaviorPattern.Strategic;

        PQUnit target = units.Pop();
        switch (_behaviorPattern)
        {
            case EMonsterBehaviorPattern.Aggressive:
                HandleAggressivePattern(target);
                break;
            case EMonsterBehaviorPattern.Defensive:
                HandleDefensivePattern(target);
                break;
            case EMonsterBehaviorPattern.Strategic:
                HandleStrategicPattern(target);
                break;
        }

        IsMyTurn = false;
    }

    void HandleAggressivePattern(PQUnit target)
    {
        Debug.Log("AggressivePattern");

        // 목적지 설정
        DestPos = Managers.Map.CellToWorld(target.CellPos);
        CreatureState = ECreatureState.Move;

        // TODO
        // 스킬 사용
    }

    void HandleDefensivePattern(PQUnit target)
    {
        Debug.Log("DefensivePattern");

        // TODO
        // 가장 가까운 타겟에게 스킬 사용

        // 타겟과 최대한 거리를 유지
        if (Mov < target.Depth)
        {
            Dictionary<Vector3Int, bool> found = new Dictionary<Vector3Int, bool>();
            Queue<(Vector3Int pos, int distance)> q = new Queue<(Vector3Int, int)>();
            q.Enqueue((CellPos, 0));

            while (q.Count > 0)
            {
                var (nowPos, nowDist) = q.Dequeue();

                int distance = Mathf.Abs(CellPos.x - target.CellPos.x) + Mathf.Abs(CellPos.y - target.CellPos.y);
                if (Mov < nowDist)
                    continue;
            }
        }
    }

    void HandleStrategicPattern(PQUnit target)
    {
        Debug.Log("StrategicPattern");

        // 타겟과 최대한 거리를 유지

        // TODO
        // 가장 가까운 타겟에게 스킬 사용
    }

    #endregion
}