using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    public bool IsMyTurn = false;
    EMonsterBehaviorPattern _behaviorPattern;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = ECreatureType.Monster;
        CreatureState = ECreatureState.Idle;
        _behaviorPattern = EMonsterBehaviorPattern.None;

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
            FindPathAndMoveToCellPos(DestPos, Mov);
            // TODO
            // 이동을 완료할 때까지 애니메이션 재생

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
    public struct PQUnit : IComparable<PQUnit>
    {
        public float Distance;
        public float Hp;
        public float Def;
        public Vector3Int CellPos;

        public int CompareTo(PQUnit other)
        {
            float distanceWeight = 1.0f;   // 거리의 가중치 (가까울수록 우선순위 높음)
            float hpWeight = 0.5f;         // HP의 가중치 (낮을수록 우선순위 높음)
            float defWeight = 0.3f;        // 방어력의 가중치 (낮을수록 우선순위 높음)

            float score = (Distance * distanceWeight) + (Hp * hpWeight) + (Def * defWeight);
            float otherScore = (other.Distance * distanceWeight) + (other.Hp * hpWeight) + (other.Def * defWeight);

            if (score == otherScore)
                return 0;
            return score < otherScore ? 1 : -1;
        }
    }

    void ExecuteAI()
    {
        Debug.Log("ExecuteAI");
        _behaviorPattern = EMonsterBehaviorPattern.Aggressive;
        if (Hp <= MaxHp * 0.3)  // 현재 체력이 30%이하라면
            _behaviorPattern = EMonsterBehaviorPattern.Defensive;
        else if (CreatureData.ClassDataID == MAGE_ID)   // 마법사라면
            _behaviorPattern = EMonsterBehaviorPattern.Strategic;

        switch (_behaviorPattern)
        {
            case EMonsterBehaviorPattern.Aggressive:
                HandleAggressivePattern();
                break;
            case EMonsterBehaviorPattern.Defensive:
                HandleDefensivePattern();
                break;
            case EMonsterBehaviorPattern.Strategic:
                HandleStrategicPattern();
                break;
        }

        IsMyTurn = false;
    }

    void HandleAggressivePattern()
    {
        // 타겟 우선 순위 설정
        PriorityQueue<PQUnit> units = new PriorityQueue<PQUnit>();
        foreach (PlayerUnitController unit in Managers.Object.PlayerUnits)
        {
            float distance = Mathf.Abs(transform.position.x - unit.transform.position.x) + Mathf.Abs(transform.position.y - unit.transform.position.y); // 맨해튼 거리 계산
            units.Push(new PQUnit() { Distance = distance, Hp = unit.Hp, Def = unit.Def, CellPos = unit.CellPos });
        }

        // 타겟이 없는지 확인
        if (units.Count <= 0)
            return;

        PQUnit target = units.Pop();
        Vector3 pos = Managers.Map.CellToWorld(target.CellPos);
        FindPathAndMoveToCellPos(pos, Mov, findClosestPos: true);
    }

    void HandleDefensivePattern()
    {
        // TODO
        // 타겟과 최대한 거리를 유지
    }

    void HandleStrategicPattern()
    {
        // TODO
        // 타겟과 최대한 거리를 유지
    }

    #endregion
}