using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = ECreatureType.Monster;
        CreatureState = ECreatureState.Idle;

        Managers.Game.OnGameStateChanged -= HandleOnGameStateChanged;
        Managers.Game.OnGameStateChanged += HandleOnGameStateChanged;

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
}