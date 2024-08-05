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

    Vector3 _cursorPos;
    public Vector3 CursorPos
    {
        get { return _cursorPos; }
        set 
        {
            _cursorPos = value;
            OnCursorPosChanged?.Invoke(_cursorPos);
        }
    }

    public event Action<EActionState> OnActionStateChanged;
    public event Action<Vector3> OnCursorPosChanged;
}
