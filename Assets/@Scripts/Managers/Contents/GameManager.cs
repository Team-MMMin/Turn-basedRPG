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

    CreatureController _currentUnit;
    public CreatureController CurrentUnit
    {
        get { return _currentUnit; }
        set 
        {
            _currentUnit = value;
        }
    }

    public CursorController Cursor;

    public event Action<EActionState> OnActionStateChanged;
}
