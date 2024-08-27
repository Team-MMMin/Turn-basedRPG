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

            // �̵� ���� �ʱ�ȭ
            if (MovementRange.Count > 0)
            {
                ClearMovementRange();
            }

            // ĳ���� ��ų ���� �ʱ�ȭ
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
            // �̵��� �Ϸ��� ������ �ִϸ��̼� ���

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
            // ���� ����
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
            float distanceWeight = 1.0f;   // �Ÿ��� ����ġ (�������� �켱���� ����)
            float hpWeight = 0.5f;         // HP�� ����ġ (�������� �켱���� ����)
            float defWeight = 0.3f;        // ������ ����ġ (�������� �켱���� ����)

            float score = (Distance * distanceWeight) + (Hp * hpWeight) + (Def * defWeight);
            float otherScore = (other.Distance * distanceWeight) + (other.Hp * hpWeight) + (other.Def * defWeight);

            if (score == otherScore)
                return 0;
            return score < otherScore ? 1 : -1;
        }
    }

    void ExecuteAI()
    {
        // �̵� ������ ���� ����
        SetMovementRange();

        // Ÿ�� �켱 ���� ����
        PriorityQueue<PQUnit> units = new PriorityQueue<PQUnit>();
        foreach (PlayerUnitController unit in Managers.Object.PlayerUnits)
        {
            float distance = Mathf.Abs(transform.position.x - unit.transform.position.x) + Mathf.Abs(transform.position.y - unit.transform.position.y); // ����ư �Ÿ� ���
            units.Push(new PQUnit() { Distance = distance, Hp = unit.Hp, Def = unit.Def });
        }

        // ���� ü���� 30%���϶��
        if (Hp <= MaxHp * 0.3)
        {
            // � ��ų�� ����� �� ���� or ��ų ���x
            // �����κ��� ���� or ������ �ֱ�
        }
        else
        {
            // � ��ų�� ����� �� ����: �ټ��� ��
        }
    }
    #endregion
}