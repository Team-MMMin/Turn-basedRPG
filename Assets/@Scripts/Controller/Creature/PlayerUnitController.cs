using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerUnitController : CreatureController
{
    Vector3 _destPos;
    public Vector3 DestPos
    {
        get { return _destPos; }
        set 
        {
            _destPos = value;
            OnDestPosChanged?.Invoke();
        }
    }

    public event Action OnDestPosChanged;

    public override bool Init()
    {
        if (base.Init() == false) 
            return false;

        CreatureType = ECreatureType.PlayerUnit;
        CreatureState = ECreatureState.Idle;

        OnDestPosChanged -= HandleOnDestPosChanged;
        OnDestPosChanged += HandleOnDestPosChanged;

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

            // 이동 범위 비시각화
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

    protected override void UpdateEndTurn()
    {
        if (CreatureState == ECreatureState.EndTurn)
        {
            //Debug.Log("UpdateEndTurn");
        }
    }

    void HandleOnDestPosChanged()
    {
        FindPathAndMoveToCellPos(_destPos, Mov);
    }

}
