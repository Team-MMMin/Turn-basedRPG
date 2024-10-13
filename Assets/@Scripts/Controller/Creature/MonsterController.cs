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

        Managers.Game.GameStateChanged -= OnGameStateChanged;
        Managers.Game.GameStateChanged += OnGameStateChanged;

        StartCoroutine(CoUpdate());
        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);
    }

    void OnGameStateChanged(EGameState gameState)
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
            if (CastingSkill != null)
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
    public struct PQTarget : IComparable<PQTarget>  // 타겟 우선순위
    {
        public float Hp;    // 체력
        public float Def;   // 방어력
        public Vector3Int CellPos;  // 위치
        public int Distance;    // 타겟과 몬스터(this)와의 거리

        public int CompareTo(PQTarget other)
        {
            // 각 가중치이다. 낮을수록 우선순위가 높다
            float hpWeight = 0.5f;
            float defWeight = 0.5f;
            float distanceWeight = 1.0f;

            float score = (Hp * hpWeight) + (Def * defWeight) + (Distance * distanceWeight);
            float otherScore = (other.Hp * hpWeight) + (other.Def * defWeight) + (other.Distance * distanceWeight);

            if (score == otherScore)
                return 0;
            return score < otherScore ? 1 : -1;
        }
    }

    EMonsterBehaviorPattern _behaviorPattern;

    void ExecuteAI()
    {
        _behaviorPattern = EMonsterBehaviorPattern.Aggressive;
        if (Hp <= MaxHp * 0.3 || CreatureData.ClassDataID == MAGE_ID)   // 현재 체력이 30%이하거나 마법사라면
            _behaviorPattern = EMonsterBehaviorPattern.Defensive;

        switch (_behaviorPattern)
        {
            case EMonsterBehaviorPattern.Aggressive:
                HandleAggressivePattern();
                break;
            case EMonsterBehaviorPattern.Defensive:
                HandleDefensivePattern();
                break;
        }

        IsMyTurn = false;
    }

    void HandleAggressivePattern()
    {
        Debug.Log("AggressivePattern");

        // 타겟 우선순위 설정
        PriorityQueue<PQTarget> targets = SetTargetPriority();

        // 타겟이 없는지 확인
        if (targets.Count <= 0)
            return;

        CastingSkill = FindSkill(targets);
        PQTarget target = targets.Pop();

        // 목적지 설정
        DestPos = Managers.Map.CellToWorld(target.CellPos); // 타겟 근처로 이동
        CreatureState = ECreatureState.Move;

        // 스킬 사용
        CreatureState = ECreatureState.Skill;
    }

    void HandleDefensivePattern()
    {
        Debug.Log("DefensivePattern");

        // 타겟 우선순위 설정
        PriorityQueue<PQTarget> targets = SetTargetPriority();

        // 타겟이 없는지 확인
        if (targets.Count <= 0)
            return;

        // 스킬 사용
        CastingSkill = FindSkill(targets);
        CreatureState = ECreatureState.Skill;

        Vector3Int destCellPos = CellPos;
        int maxTotalDistance = 0; 
        SetMovementRange();

        // 주변 타겟과 최대한 거리를 유지
        foreach (var pos in MovementRange)
        {
            Vector3Int cellPos = Managers.Map.WorldToCell(pos);
            int totalDistance = 0; // 모든 유닛과의 총 거리

            // 모든 유닛과의 거리 합산
            foreach (var unit in Managers.Object.PlayerUnits)
            {
                int distance = Mathf.Abs(cellPos.x - unit.CellPos.x) + Mathf.Abs(cellPos.y - unit.CellPos.y); // 맨해튼 거리 계산
                totalDistance += distance;
            }

            // 총 거리가 최대가 되는 위치를 찾기
            if (totalDistance > maxTotalDistance)
            {
                maxTotalDistance = totalDistance;
                destCellPos = cellPos;
            }
        }

        // 목적지 설정
        DestPos = Managers.Map.CellToWorld(destCellPos);
        CreatureState = ECreatureState.Move;
    }

    //SkillBase FindSkill()
    PriorityQueue<PQTarget> SetTargetPriority()
    {
        PriorityQueue<PQTarget> units = new PriorityQueue<PQTarget>();
        foreach (var unit in Managers.Object.PlayerUnits)
        {
            int distance = Mathf.Abs(CellPos.x - unit.CellPos.x) + Mathf.Abs(CellPos.y - unit.CellPos.y);   // 맨해튼 거리 계산
            units.Push(new PQTarget() { Hp = unit.Hp, Def = unit.Def, CellPos = unit.CellPos, Distance = distance });
        }

        return units;
    }

    #endregion
}