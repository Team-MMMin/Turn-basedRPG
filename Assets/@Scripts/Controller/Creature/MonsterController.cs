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
    public struct PQTarget : IComparable<PQTarget>  // 타겟 우선순위
    {
        public float Hp;
        public float Def;
        public Vector3Int CellPos;
        public int Distance;

        public int CompareTo(PQTarget other)
        {
            float hpWeight = 0.5f;  // HP 가중치 (낮을수록 우선순위 높음)
            float defWeight = 0.5f; // 방어력 가중치 (낮을수록 우선순위 높음)
            float distanceWeight = 1.0f;

            float score = (Hp * hpWeight) + (Def * defWeight) + (Distance * distanceWeight);
            float otherScore = (other.Hp * hpWeight) + (other.Def * defWeight) + (other.Distance * distanceWeight);

            if (score == otherScore)
                return 0;
            return score < otherScore ? 1 : -1;
        }
    }

    void ExecuteAI()
    {
        EMonsterBehaviorPattern behaviorPattern = EMonsterBehaviorPattern.Aggressive;
        if (Hp <= MaxHp * 0.3 || CreatureData.ClassDataID == MAGE_ID)   // 현재 체력이 30%이하거나 마법사라면
            behaviorPattern = EMonsterBehaviorPattern.Defensive;

        switch (behaviorPattern)
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
        PriorityQueue<PQTarget> units = new PriorityQueue<PQTarget>();
        foreach (var unit in Managers.Object.PlayerUnits)
        {
            int distance = Mathf.Abs(CellPos.x - unit.CellPos.x) + Mathf.Abs(CellPos.y - unit.CellPos.y);   // 맨해튼 거리 계산
            units.Push(new PQTarget() { Hp = unit.Hp, Def = unit.Def, CellPos = unit.CellPos, Distance = distance});
        }

        // 유닛이 없는지 확인
        if (units.Count <= 0)
            return;

        PQTarget target = units.Pop();

        // 목적지 설정
        DestPos = Managers.Map.CellToWorld(target.CellPos); // 타겟 근처로 이동
        CreatureState = ECreatureState.Move;

        // TODO
        // 스킬 사용
    }

    void HandleDefensivePattern()
    {
        Debug.Log("DefensivePattern");

        // 타겟 우선순위 설정
        PriorityQueue<PQTarget> units = new PriorityQueue<PQTarget>();
        foreach (var unit in Managers.Object.PlayerUnits)
        {
            int distance = Mathf.Abs(CellPos.x - unit.CellPos.x) + Mathf.Abs(CellPos.y - unit.CellPos.y);   // 맨해튼 거리 계산
            units.Push(new PQTarget() { Hp = unit.Hp, Def = unit.Def, CellPos = unit.CellPos, Distance = distance });
        }

        // 타겟이 없는지 확인
        if (units.Count <= 0)
            return;

        PQTarget target = units.Pop();

        // TODO
        // 타겟에게 스킬 사용 or 자기 자신에게 힐

        Vector3Int destCellPos = CellPos;
        int maxTotalDistance = 0; 
        SetMovementRange();

        // 주변 유닛과 최대한 거리를 유지
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
    #endregion
}