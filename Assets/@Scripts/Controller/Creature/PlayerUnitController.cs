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

        Managers.Game.OnActionStateChanged -= HandleOnActionStateChanged;
        Managers.Game.OnActionStateChanged += HandleOnActionStateChanged;
        
        OnDestPosChanged -= HandleOnDestPosChanged;
        OnDestPosChanged += HandleOnDestPosChanged;

        StartCoroutine(CoUpdate());
        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);
    }

    protected override void UpdateMove()
    {
        if (CreatureState == ECreatureState.Move)
        {
            Debug.Log("UpdateMove");
            // TODO
            // 플레이어 유닛의 무브 포인트만큼 이동 가능한 구역 표시
        }
    }

    void HandleOnActionStateChanged(EActionState actionState)
    {
        switch (actionState)
        {
            case EActionState.None:
                CreatureState = ECreatureState.Idle;
                break;
            case EActionState.Move:
                CreatureState = ECreatureState.Move;
                break;
            case EActionState.Skill:
                break;
        }
    }

    void HandleOnDestPosChanged()
    {
        FindPathAndMoveToCellPos(_destPos, Mov);
    }
}
