using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerUnitController : CreatureController
{
    public override bool Init()
    {
        if (base.Init() == false) 
            return false;

        CreatureType = ECreatureType.PlayerUnit;
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
        if (gameState != EGameState.PlayerTurn)
            return;

        IsMove = false;
        IsSkill = false;
        CreatureState = ECreatureState.Idle;
    }

    protected override void UpdateIdle()
    {
        if (CreatureState == ECreatureState.Idle)
        {
            Debug.Log("UpdateIdle");
            // 이동 범위 비시각화 및 초기화
            if (MovementRange.Count > 0)
            {
                ClearMovementRange();
            }

            // 캐스팅 스킬 범위 비시각화 및 초기화
            if (CastingSkill != null)
            {
                CastingSkill.ClearCastingRange();
                CastingSkill.ClearSizeRange();
                CastingSkill = null;
            }

            CreatureState = ECreatureState.None;
        }
    }

    protected override void UpdateMove()
    {
        if (CreatureState == ECreatureState.Move)
        {
            Debug.Log("UpdateMove");
            FindPathAndMoveToCellPos(DestPos, Mov);
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
}
