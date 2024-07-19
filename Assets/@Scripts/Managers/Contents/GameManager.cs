using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager
{
    EGameState _gameState;
    public EGameState GameState
    {
        get { return _gameState; }
        set { _gameState = value; }
    }

    EActionState _actionState;
    public EActionState ActionState
    {
        get { return _actionState; }
        set 
        {
            _actionState = value;
            OnActionStateChanged?.Invoke(_actionState);
        }
    }

    public Vector3 _clickCellPos;
    public Vector3 ClickCellPos
    {
        get { return _clickCellPos; }
        set 
        {
            _clickCellPos = value;
            OnClickCellPosChanged?.Invoke(_clickCellPos);
        }
    }

    public event Action<EActionState> OnActionStateChanged;
    public event Action<Vector3> OnClickCellPosChanged;
}
