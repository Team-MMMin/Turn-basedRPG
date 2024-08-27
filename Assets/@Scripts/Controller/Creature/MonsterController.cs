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
            if (Managers.Game.CurrentUnit == this)
            {
                ExecuteAI();
                return;
            }

            // 이동 범위 초기화
            if (MovementRange.Count > 0)
            {
                ClearMovementRange();
            }

            // 캐스팅 스킬 범위 초기화
            if (CastingSkill != null)
            {
                CastingSkill.ClearCastingRange();
                CastingSkill.ClearSizeRange();
                CastingSkill = null;
            }
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

    protected override void UpdateEndTurn()
    {
        if (CreatureState == ECreatureState.EndTurn)
        {
            //Debug.Log("UpdateEndTurn");
        }
    }

    #region AI
    public struct PQUnit : IComparable<PQUnit>
    {
        public float Distance;
        public float Hp;
        public float Def;

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
        // 이동 가능한 범위 설정
        SetMovementRange();

        // 타겟 우선 순위 설정
        PriorityQueue<PQUnit> units = new PriorityQueue<PQUnit>();
        foreach (PlayerUnitController unit in Managers.Object.PlayerUnits)
        {
            float distance = Mathf.Abs(transform.position.x - unit.transform.position.x) + Mathf.Abs(transform.position.y - unit.transform.position.y); // 맨해튼 거리 계산
            units.Push(new PQUnit() { Distance = distance, Hp = unit.Hp, Def = unit.Def });
        }

        // 현재 체력이 30%이하라면
        if (Hp <= MaxHp * 0.3)
        {
            // 어떤 스킬을 사용할 지 결정 or 스킬 사용x
            // 적으로부터 후퇴 or 가만히 있기
        }
        else
        {
            // 어떤 스킬을 사용할 지 결정: 다수의 적
        }
    }
    #endregion
}